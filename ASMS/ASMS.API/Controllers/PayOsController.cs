using Microsoft.AspNetCore.Mvc;
using ASMS.Services.Interfaces;
using ASMS.Services.Model.PayOS;
using Net.payOS.Types;


namespace ASMS.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class PayOsController : ControllerBase
    {
        private readonly IPayOSService _payOSService;

        public PayOsController(IPayOSService payOSService)
        {
            _payOSService = payOSService;
        }

        
        [HttpPost("create-link/{orderCode}")]
        public async Task<IActionResult> CreatePaymentLink(string orderCode)
        {
            var result = await _payOSService.CreatePaymentLink(orderCode);
            return Ok(result);
        }

       
        [HttpPost("confirm-webhook")]
        public async Task<IActionResult> ConfirmWebhook(WebhookURL body)
        {
            var result = await _payOSService.ConfirmWebhook(body);
            return Ok(result);
        }

     
        [HttpPost("webhook")]
        public async Task<IActionResult> HandleWebhook([FromBody] WebhookType webhookData)
        {
            await _payOSService.HandlePaymentWebhook(webhookData);
            return Ok();
        }


        [HttpGet("result/{paymentCode}")]
        public async Task<IActionResult> GetPaymentResult(string paymentCode)
        {
            var result = await _payOSService.GetPaymentResult(paymentCode);

            if (result == null)
            {
                return NotFound(new { message = "Payment result not found" });
            }

            return Ok(result);
        }
    }

}
