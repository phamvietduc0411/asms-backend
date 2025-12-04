using ASMS.Repositories.Entities;
using ASMS.Repositories.Infrastructures;
using ASMS.Services.Interfaces;
using ASMS.Services.Model.PayOS;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Net.payOS;
using Net.payOS.Types;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ASMS.Services.Services
{
    public class PayOSService : IPayOSService
    {
        private readonly PayOS _payOS;
        private readonly IConfiguration _config;
        private readonly IUnitOfWork _unitOfWork;
        private readonly ILogger<PayOSService> _logger;

        public PayOSService(PayOS payOS, IConfiguration config, IUnitOfWork unitOfWork, ILogger<PayOSService> logger)
        {
            _payOS = payOS;
            _config = config;
            _unitOfWork = unitOfWork;
            _logger = logger;
        }

        public async Task<object> CreatePaymentLink(string orderCode)
        {
            var order = await _unitOfWork.Orders.GetByCodeAsync(orderCode);
            if (order == null) throw new Exception("Order not found");
            if (order.UnpaidAmount == null || order.UnpaidAmount <= 0) throw new Exception("Order is already paid or has no unpaid amount.");

            //  paymentCode 
            long paymentCode = long.Parse($"{DateTime.UtcNow:yyMMddHHmmss}");

            
            var descToSend = orderCode;
            if (descToSend.Length > 25) descToSend = descToSend.Substring(0, 25);

            ItemData item = new ItemData(name: "Payment for the order", quantity: 1, price: (int)order.UnpaidAmount.Value);

            var backendBaseUrl = _config["Backend:BaseUrl"];
            string successUrl = $"{_config["Backend:BaseUrl"]}/api/PayOs/result/{paymentCode}";
            string cancelUrl = $"{_config["Backend:BaseUrl"]}/api/PayOs/result/{paymentCode}";


            PaymentData paymentData = new PaymentData(
                orderCode: paymentCode,
                amount: (int)order.UnpaidAmount.Value,
                description: descToSend, 
                items: new List<ItemData> { item },
                cancelUrl,
                successUrl
            );

            CreatePaymentResult result = await _payOS.createPaymentLink(paymentData);

            var pendingPayment = new PaymentResult
            {
                PaymentCode = paymentCode.ToString(),   
                OrderCode = orderCode,
                Status = "Pending",
                Message = "Pending",
                Url = result.checkoutUrl,
                Amount = order.UnpaidAmount.Value,
                CreatedAt = DateTime.UtcNow
            };

            await _unitOfWork.PaymentResults.AddAsync(pendingPayment);
            await _unitOfWork.CompleteAsync();
            

            return new
            {
                paymentCode,
                checkoutUrl = result.checkoutUrl
            };
        }


        public async Task HandlePaymentWebhook(WebhookType webhookData)
        {
            WebhookData data = _payOS.verifyPaymentWebhookData(webhookData);

            
            string paymentCode = data.orderCode.ToString();

           
            var existingPaymentResult = await _unitOfWork.PaymentResults.GetByPaymentCodeAsync(paymentCode);

            string orderCode = existingPaymentResult?.OrderCode;

           
            if (string.IsNullOrEmpty(orderCode))
            {
                orderCode = data.description ?? data.desc ?? data.reference ?? data.virtualAccountNumber ?? data.accountNumber;
            }

            //  debug
            _logger.LogInformation("Webhook received. paymentCode={PaymentCode}, description={Description}, desc={Desc}, reference={Reference}, virtualAccountNumber={VAN}", paymentCode, data.description, data.desc, data.reference, data.virtualAccountNumber);

            if (string.IsNullOrEmpty(orderCode))
            {
                _logger.LogWarning("Cannot determine orderCode from webhook payload. paymentCode={PaymentCode}", paymentCode);
                return;
            }

            var order = await _unitOfWork.Orders.GetByCodeAsync(orderCode);
            if (order == null)
            {
                _logger.LogWarning("Order {OrderCode} not found while handling webhook for paymentCode {PaymentCode}", orderCode, paymentCode);
                
            }

            bool isSuccess = data.code == "00";

            
            if (existingPaymentResult != null)
            {
                existingPaymentResult.Status = isSuccess ? "Success" : "Failed";
                existingPaymentResult.Message = isSuccess ? "Payment successful" : data.desc ?? data.code;
                existingPaymentResult.Amount = data.amount;
                existingPaymentResult.Url = isSuccess ? $"{_config["Frontend:BaseUrl"]}/payment-success?orderCode={orderCode}&paymentCode={paymentCode}" : $"{_config["Frontend:BaseUrl"]}/payment-failed?orderCode={orderCode}&paymentCode={paymentCode}";
                await _unitOfWork.PaymentResults.UpdateAsync(existingPaymentResult);
            }
            else
            {
                
                var paymentResult = new PaymentResult
                {
                    PaymentCode = paymentCode,
                    OrderCode = orderCode,
                    Status = isSuccess ? "Success" : "Failed",
                    Message = isSuccess ? "Payment successful! Your order has been confirmed." : "Payment failed. Please try again.",
                    Url = isSuccess ? $"{_config["Frontend:BaseUrl"]}/payment-success?orderCode={orderCode}&paymentCode={paymentCode}" : $"{_config["Frontend:BaseUrl"]}/payment-failed?orderCode={orderCode}&paymentCode={paymentCode}",
                    Amount = data.amount,
                    CreatedAt = DateTime.UtcNow
                };

                await _unitOfWork.PaymentResults.AddAsync(paymentResult);
            }

            if (order != null)
            {
                order.PaymentStatus = isSuccess ? "PAID" : "Failed";
                if (isSuccess)
                {
                    order.UnpaidAmount = 0;

                    // Lưu Payment History
                    var payment = new PaymentHistory
                    {
                        PaymentHistoryCode = Guid.NewGuid().ToString(),
                        OrderCode = order.OrderCode,
                        Amount = data.amount,
                        PaymentMethod = "PayOS",
                        PaymentPlatform = "PayOS",
                    };
                    await _unitOfWork.PaymentHistories.AddAsync(payment);

                    // Tracking
                    //var tracking = new TrackingHistory
                    //{
                    //    OrderDetailCode = null,
                    //    OldStatus = order.Status,
                    //    NewStatus = "Paid",
                    //    ActionType = "Payment",
                    //    CreateAt = DateOnly.FromDateTime(DateTime.Now),
                    //};
                    //await _unitOfWork.TrackingHistories.AddAsync(tracking);

                    //order.Status = "Paid";
                }

                await _unitOfWork.Orders.UpdateAsync(order);
            }

            await _unitOfWork.CompleteAsync();
        }

        public async Task<PaymentResultDto?> GetPaymentResult(string paymentCode)
        {
            var result = await _unitOfWork.PaymentResults.GetByPaymentCodeAsync(paymentCode);

            if (result == null)
                return null;

            return new PaymentResultDto
            {
                PaymentCode = result.PaymentCode,
                OrderCode = result.OrderCode,
                Status = result.Status,
                Message = result.Message,
                Url = result.Url,
                Amount = result.Amount,
                CreatedAt = result.CreatedAt
            };
        }
        public async Task<string> ConfirmWebhook(WebhookURL body)
        {
            try
            {
                return await _payOS.confirmWebhook(body.webhook_url);
            }
            catch (Exception ex)
            {
                return ex.Message;
            }
        }
    }
}
