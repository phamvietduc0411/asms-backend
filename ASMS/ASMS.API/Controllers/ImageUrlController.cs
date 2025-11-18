using ASMS.Services.Interfaces;
using ASMS.Services.Model.Image;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace ASMS.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ImageUrlController : ControllerBase
    {
        private readonly IImageUrlService _imageUrlService;

        public ImageUrlController(IImageUrlService imageUrlService)
        {
            _imageUrlService = imageUrlService;
        }

        /// <summary>
        /// Batch update ImageUrl cho nhiều entities trong cùng một bảng
        /// </summary>
        /// <param name="request">Request chứa tên bảng và danh sách items cần update Container|Storage|Shelf|Floor|ContainerType</param>
        /// <returns>Response chứa kết quả update</returns>
        /// <response code="200">Cập nhật thành công</response>
        /// <response code="400">Request không hợp lệ</response>
        [HttpPatch("batch-update")]

        public async Task<IActionResult> BatchUpdateImageUrl([FromBody] BatchUpdateImageUrlRequest request)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var result = await _imageUrlService.BatchUpdateImageUrlAsync(request);

            // Nếu tất cả đều fail hoặc bảng không được hỗ trợ
            if (result.FailedCount == result.TotalItems && result.SuccessCount == 0)
            {
                return BadRequest(result);
            }

            return Ok(result);
        }
    }
}
