using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ASMS.Repositories.Entities;
using ASMS.Repositories.Infrastructures;
using ASMS.Services.Interfaces;

namespace ASMS.Services.Services
{
    public class ShortLinkService : IShortLinkService
    {
        private readonly IUnitOfWork _unitOfWork;
        private const string Characters = "abcdefghijklmnopqrstuvwxyzABCDEFGHIJKLMNOPQRSTUVWXYZ0123456789";

        public ShortLinkService(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        public async Task<string> GenerateShortCodeAsync(
            string originalUrl,
            string? orderCode = null,
            int? orderDetailId = null,
            int? expiresInDays = null)
        {
            // Check if URL already has a short code
            var existing = await _unitOfWork.ShortLinks.GetByOriginalUrlAsync(originalUrl);
            if (existing != null)
            {
                // Update Order/OrderDetail nếu cần
                await UpdateOrderShortCodeAsync(orderCode, orderDetailId, existing.ShortCode);
                return existing.ShortCode;
            }

            // Generate unique short code
            string shortCode;
            do
            {
                shortCode = GenerateRandomString(6);
            }
            while (await _unitOfWork.ShortLinks.ShortCodeExistsAsync(shortCode));

            var shortLink = new ShortLink
            {
                ShortCode = shortCode,
                OriginalUrl = originalUrl,
                CreatedAt = DateTime.UtcNow,
                ExpiresAt = expiresInDays.HasValue
                    ? DateTime.UtcNow.AddDays(expiresInDays.Value)
                    : null,
                ClickCount = 0
            };

            await _unitOfWork.ShortLinks.AddAsync(shortLink);

            // Cập nhật ShortCode vào Order hoặc OrderDetail
            await UpdateOrderShortCodeAsync(orderCode, orderDetailId, shortCode);

            await _unitOfWork.CompleteAsync();

            return shortCode;
        }
        private async Task UpdateOrderShortCodeAsync(string? orderCode, int? orderDetailId, string shortCode)
        {
            // Update Order
            if (!string.IsNullOrEmpty(orderCode))
            {
                var order = await _unitOfWork.Orders
                    .GetByCodeAsync(orderCode);

                if (order != null)
                {
                    order.ShortCode = shortCode;
                    await _unitOfWork.Orders.UpdateAsync(order);
                }
            }

            // Update OrderDetail
            if (orderDetailId.HasValue)
            {
                var orderDetail = await _unitOfWork.OrderDetails
                    .GetEntityByIdAsync(orderDetailId.Value);

                if (orderDetail != null)
                {
                    orderDetail.ShortCode = shortCode;
                    await _unitOfWork.OrderDetails.UpdateAsync(orderDetail);
                }
            }
        }

        public async Task<ShortLink?> GetByOrderCodeAsync(string orderCode)
        {
            var order = await _unitOfWork.Orders
                .GetByCodeAsync(orderCode);

            if (order == null || string.IsNullOrEmpty(order.ShortCode))
                return null;

            return await _unitOfWork.ShortLinks.GetByShortCodeAsync(order.ShortCode);
        }

        public async Task<ShortLink?> GetByOrderDetailIdAsync(int orderDetailId)
        {
            var orderDetail = await _unitOfWork.OrderDetails
                .GetEntityByIdAsync(orderDetailId);

            if (orderDetail == null || string.IsNullOrEmpty(orderDetail.ShortCode))
                return null;

            return await _unitOfWork.ShortLinks.GetByShortCodeAsync(orderDetail.ShortCode);
        }

        public async Task<ShortLink?> GetByShortCodeAsync(string shortCode)
        {
            var shortLink = await _unitOfWork.ShortLinks.GetByShortCodeAsync(shortCode);

            if (shortLink != null && shortLink.ExpiresAt.HasValue && shortLink.ExpiresAt < DateTime.UtcNow)
            {
                return null; 
            }

            return shortLink;
        }
        public async Task<ShortLink?> GetByOriginalUrlAsync(string originalUrl)
        {
            var shortLink = await _unitOfWork.ShortLinks.GetByOriginalUrlAsync(originalUrl);


            if (shortLink != null && shortLink.ExpiresAt.HasValue && shortLink.ExpiresAt < DateTime.UtcNow)
            {
                return null; 
            }

            return shortLink;
        }

        public async Task IncrementClickCountAsync(string shortCode)
        {
            await _unitOfWork.ShortLinks.IncrementClickCountAsync(shortCode);
            await _unitOfWork.CompleteAsync();
        }

        public async Task DeleteExpiredLinksAsync()
        {
            var expiredLinks = await _unitOfWork.ShortLinks.GetExpiredLinksAsync();

            foreach (var link in expiredLinks)
            {
                await _unitOfWork.ShortLinks.DeleteAsync(link);
            }

            await _unitOfWork.CompleteAsync();
        }

        private string GenerateRandomString(int length)
        {
            var random = new Random();
            return new string(Enumerable.Range(0, length)
                .Select(_ => Characters[random.Next(Characters.Length)])
                .ToArray());
        }
    }
}
