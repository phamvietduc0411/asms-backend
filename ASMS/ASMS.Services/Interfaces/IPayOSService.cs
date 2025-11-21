using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ASMS.Services.Model.PayOS;
using Net.payOS.Types;


namespace ASMS.Services.Interfaces
{
    public interface IPayOSService
    {
        Task<object> CreatePaymentLink(string orderCode);
        Task HandlePaymentWebhook(WebhookType webhookData);
        Task<string> ConfirmWebhook(WebhookURL body);
    }
}
