using ASMS.Repositories.Entities;
using ASMS.Repositories.Infrastructures;
using ASMS.Services.Interfaces;
using ASMS.Services.Model.PayOS;
using Microsoft.Extensions.Configuration;
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

        public PayOSService(PayOS payOS, IConfiguration config, IUnitOfWork unitOfWork)
        {
            _payOS = payOS;
            _config = config;
            _unitOfWork = unitOfWork;
        }

        public async Task<object> CreatePaymentLink(string orderCode)
        {
            var order = await _unitOfWork.Orders.GetByCodeAsync(orderCode);

            if (order == null)
                throw new Exception("Order not found");

            if (order.UnpaidAmount == null || order.UnpaidAmount <= 0)
                throw new Exception("Order is already paid or has no unpaid amount.");

            // Tạo mã thanh toán duy nhất
            long paymentCode = long.Parse(DateTimeOffset.Now.ToString("yyyyMMddHHmmssfff"));

            ItemData item = new ItemData(
                name: "Thanh toán đơn hàng",
                quantity: 1,
                price: (int)order.UnpaidAmount.Value
            );

            var baseUrl = _config["Frontend:BaseUrl"];
            string successUrl = $"{baseUrl}/payment-success?orderCode={orderCode}";
            string cancelUrl = $"{baseUrl}/payment-cancel?orderCode={orderCode}";

            PaymentData paymentData = new PaymentData(
                orderCode: paymentCode,
                amount: (int)order.UnpaidAmount.Value,
                description: $"Payment for the order {orderCode}",
                items: new List<ItemData> { item },
                cancelUrl,
                successUrl
            );

          
            CreatePaymentResult result = await _payOS.createPaymentLink(paymentData);

            return new
            {
                paymentCode,
                checkoutUrl = result.checkoutUrl
            };
        }

        public async Task HandlePaymentWebhook(WebhookType webhookData)
        {
            WebhookData data = _payOS.verifyPaymentWebhookData(webhookData);

            string orderCode = data.orderCode.ToString();

            var order = await _unitOfWork.Orders.GetByCodeAsync(orderCode);

            if (order == null)
                return;

            bool isSuccess = data.code == "00";

            // Cập nhật Order
            order.PaymentStatus = isSuccess ? "Paid" : "Failed";

            if (isSuccess)
            {
                order.UnpaidAmount = 0;

                // Lưu vào PaymentHistory
                var payment = new PaymentHistory
                {
                    PaymentHistoryCode = Guid.NewGuid().ToString(),
                    OrderCode = order.OrderCode,
                    Amount = order.TotalPrice,
                    PaymentMethod = "PayOS",
                    PaymentPlatform = "PayOS",
                };

                await _unitOfWork.PaymentHistories.AddAsync(payment);

                // Tracking
                var tracking = new TrackingHistory
                {
                    OrderDetailCode = null,
                    OldStatus = order.Status,
                    NewStatus = "Paid",
                    ActionType = "Payment",
                    CreateAt = DateOnly.FromDateTime(DateTime.Now),
                };
                await _unitOfWork.TrackingHistories.AddAsync(tracking);

                order.Status = "Paid";
            }

            await _unitOfWork.Orders.UpdateAsync(order);
            await _unitOfWork.CompleteAsync();
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
