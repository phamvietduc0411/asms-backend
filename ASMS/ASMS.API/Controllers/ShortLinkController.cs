using ASMS.Services.Interfaces;
using ASMS.Services.Model.ShortLink;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace ASMS.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ShortLinkController : ControllerBase
    {
        private readonly IShortLinkService _shortLinkService;

        public ShortLinkController(IShortLinkService shortLinkService)
        {
            _shortLinkService = shortLinkService;
        }

        /// <summary>
        /// Tạo short link với filter theo OrderCode hoặc OrderDetailId
        /// </summary>
        [HttpPost("create")]
        public async Task<IActionResult> CreateShortLink([FromBody] CreateShortLinkRequest request)
        {
            if (string.IsNullOrWhiteSpace(request.OriginalUrl))
                return BadRequest(new { message = "Original URL is required." });

            var shortCode = await _shortLinkService.GenerateShortCodeAsync(
                request.OriginalUrl,
                request.OrderCode,
                request.OrderDetailId,
                request.ExpiresInDays
            );

            var shortUrl = $"{Request.Scheme}://{Request.Host}/s/{shortCode}";

            return Ok(new ShortLinkDto
            {
                ShortUrl = shortUrl,
                ShortCode = shortCode,
                OriginalUrl = request.OriginalUrl,
                CreatedAt = DateTime.UtcNow,
                ExpiresAt = request.ExpiresInDays.HasValue
                    ? DateTime.UtcNow.AddDays(request.ExpiresInDays.Value)
                    : null
            });
        }
        /// <summary>
        /// Lấy short link theo OrderCode
        /// </summary>
        [HttpGet("order/{orderCode}")]
        public async Task<IActionResult> GetByOrderCode(string orderCode)
        {
            var shortLink = await _shortLinkService.GetByOrderCodeAsync(orderCode);

            if (shortLink == null)
                return NotFound(new { message = $"No short link found for order: {orderCode}" });

            var shortUrl = $"{Request.Scheme}://{Request.Host}/s/{shortLink.ShortCode}";

            return Ok(new ShortLinkDto
            {
                ShortUrl = shortUrl,
                ShortCode = shortLink.ShortCode,
                OriginalUrl = shortLink.OriginalUrl,
                CreatedAt = shortLink.CreatedAt,
                ExpiresAt = shortLink.ExpiresAt
            });
        }

        /// <summary>
        /// Lấy short link theo OrderDetailId
        /// </summary>
        [HttpGet("order-detail/{orderDetailId}")]
        public async Task<IActionResult> GetByOrderDetailId(int orderDetailId)
        {
            var shortLink = await _shortLinkService.GetByOrderDetailIdAsync(orderDetailId);

            if (shortLink == null)
                return NotFound(new { message = $"No short link found for order detail: {orderDetailId}" });

            var shortUrl = $"{Request.Scheme}://{Request.Host}/s/{shortLink.ShortCode}";

            return Ok(new ShortLinkDto
            {
                ShortUrl = shortUrl,
                ShortCode = shortLink.ShortCode,
                OriginalUrl = shortLink.OriginalUrl,
                CreatedAt = shortLink.CreatedAt,
                ExpiresAt = shortLink.ExpiresAt
            });
        }

        /// <summary>
        /// Redirect từ short code về URL gốc
        /// </summary>
        [HttpGet("/s/{shortCode}")]
        public async Task<IActionResult> RedirectToOriginal(string shortCode)
        {
            var shortLink = await _shortLinkService.GetByShortCodeAsync(shortCode);

            if (shortLink == null)
                return NotFound(new { message = "Short link not found or expired." });

            // Increment click count (fire and forget)
            _ = _shortLinkService.IncrementClickCountAsync(shortCode);

            return Redirect(shortLink.OriginalUrl);
        }

        /// <summary>
        /// Lấy thông tin short link 
        /// </summary>
        [HttpGet("{shortCode}/info")]
        public async Task<IActionResult> GetShortLinkInfo(string shortCode)
        {
            var shortLink = await _shortLinkService.GetByShortCodeAsync(shortCode);

            if (shortLink == null)
                return NotFound(new { message = "Short link not found." });

            return Ok(new
            {
                shortCode = shortLink.ShortCode,
                originalUrl = shortLink.OriginalUrl,
                createdAt = shortLink.CreatedAt,
                expiresAt = shortLink.ExpiresAt,
                clickCount = shortLink.ClickCount
            });
        }
        /// <summary>
        /// Tìm short link từ original URL
        /// </summary>
        [HttpGet("find")]
        public async Task<IActionResult> FindByOriginalUrl([FromBody] FindShortLinkRequest request)
        {
            if (string.IsNullOrWhiteSpace(request.OriginalUrl))
                return BadRequest(new { message = "Original URL is required." });

            var shortLink = await _shortLinkService.GetByOriginalUrlAsync(request.OriginalUrl);

            if (shortLink == null)
                return NotFound(new { message = "No short link found for this URL." });

            var shortUrl = $"{Request.Scheme}://{Request.Host}/s/{shortLink.ShortCode}";

            return Ok(new ShortLinkDto
            {
                ShortUrl = shortUrl,
                ShortCode = shortLink.ShortCode,
                OriginalUrl = shortLink.OriginalUrl,
                CreatedAt = shortLink.CreatedAt,
                ExpiresAt = shortLink.ExpiresAt
            });
        }
        /// <summary>
        /// Lấy short URL từ short code
        /// </summary>
        [HttpGet("{shortCode}")]
        public async Task<IActionResult> GetShortUrl(string shortCode)
        {
            var shortLink = await _shortLinkService.GetByShortCodeAsync(shortCode);

            if (shortLink == null)
                return NotFound(new { message = "Short link not found." });

            var shortUrl = $"{Request.Scheme}://{Request.Host}/s/{shortCode}";

            return Ok(new
            {
                shortUrl,
                shortCode = shortLink.ShortCode,
                originalUrl = shortLink.OriginalUrl
            });
        }
    }
}
