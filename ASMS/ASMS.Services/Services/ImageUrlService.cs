using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ASMS.Repositories.Infrastructures;
using ASMS.Services.Interfaces;
using ASMS.Services.Model.Image;
using Microsoft.Extensions.Logging;

namespace ASMS.Services.Services
{
    public class ImageUrlService : IImageUrlService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly ILogger<ImageUrlService> _logger;

        public ImageUrlService(IUnitOfWork unitOfWork, ILogger<ImageUrlService> logger)
        {
            _unitOfWork = unitOfWork;
            _logger = logger;
        }
        public async Task<BatchUpdateImageUrlResponse> BatchUpdateImageUrlAsync(BatchUpdateImageUrlRequest request)
        {
            var response = new BatchUpdateImageUrlResponse
            {
                TableName = request.TableName,
                TotalItems = request.Items.Count
            };

            try
            {
                _logger.LogInformation($"Starting batch update ImageUrl for table: {request.TableName}, Items: {request.Items.Count}");

                switch (request.TableName.ToLower())
                {
                    case "container":
                        await UpdateContainerImagesAsync(request.Items, response);
                        break;

                    case "storage":
                        await UpdateStorageImagesAsync(request.Items, response);
                        break;

                    case "shelf":
                        await UpdateShelfImagesAsync(request.Items, response);
                        break;

                    case "floor":
                        await UpdateFloorImagesAsync(request.Items, response);
                        break;
                    case "containertype":
                        await UpdateContainerTypeImagesAsync(request.Items, response);
                        break;

                    default:
                        response.Message = $"Bảng '{request.TableName}' không được hỗ trợ";
                        response.FailedCount = request.Items.Count;
                        return response;
                }

                // Commit changes
                await _unitOfWork.CompleteAsync();

                response.Message = $"Cập nhật thành công {response.SuccessCount}/{response.TotalItems} items";
                _logger.LogInformation($"Batch update completed. Success: {response.SuccessCount}, Failed: {response.FailedCount}");

                return response;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Error occurred while batch updating ImageUrl for table: {request.TableName}");
                response.Message = $"Lỗi khi cập nhật: {ex.Message}";
                response.FailedCount = request.Items.Count;
                return response;
            }
        }

        #region Private Methods

        private async Task UpdateContainerImagesAsync(List<ImageUrlUpdateItem> items, BatchUpdateImageUrlResponse response)
        {
            foreach (var item in items)
            {
                try
                {
                    var entity = await _unitOfWork.Containers.GetByCodeAsync(item.Code);

                    if (entity == null)
                    {
                        response.FailedCount++;
                        response.Errors.Add(new UpdateErrorDetail
                        {
                            Code = item.Code,
                            ErrorMessage = $"Container với code '{item.Code}' không tồn tại"
                        });
                        continue;
                    }

                    entity.ImageUrl = item.ImageUrl;
                    await _unitOfWork.Containers.UpdateAsync(entity);

                    response.SuccessCount++;
                    response.SuccessCodes.Add(item.Code);

                    _logger.LogInformation($"Updated ImageUrl for Container: {item.Code}");
                }
                catch (Exception ex)
                {
                    response.FailedCount++;
                    response.Errors.Add(new UpdateErrorDetail
                    {
                        Code = item.Code,
                        ErrorMessage = ex.Message
                    });
                    _logger.LogError(ex, $"Error updating Container: {item.Code}");
                }
            }
        }

        private async Task UpdateStorageImagesAsync(List<ImageUrlUpdateItem> items, BatchUpdateImageUrlResponse response)
        {
            foreach (var item in items)
            {
                try
                {
                    var entity = await _unitOfWork.Storages.GetByCodeAsync(item.Code);

                    if (entity == null)
                    {
                        response.FailedCount++;
                        response.Errors.Add(new UpdateErrorDetail
                        {
                            Code = item.Code,
                            ErrorMessage = $"Storage với code '{item.Code}' không tồn tại"
                        });
                        continue;
                    }

                    entity.ImageUrl = item.ImageUrl;
                    await _unitOfWork.Storages.UpdateAsync(entity);

                    response.SuccessCount++;
                    response.SuccessCodes.Add(item.Code);

                    _logger.LogInformation($"Updated ImageUrl for Storage: {item.Code}");
                }
                catch (Exception ex)
                {
                    response.FailedCount++;
                    response.Errors.Add(new UpdateErrorDetail
                    {
                        Code = item.Code,
                        ErrorMessage = ex.Message
                    });
                    _logger.LogError(ex, $"Error updating Storage: {item.Code}");
                }
            }
        }

        private async Task UpdateShelfImagesAsync(List<ImageUrlUpdateItem> items, BatchUpdateImageUrlResponse response)
        {
            foreach (var item in items)
            {
                try
                {
                    var entity = await _unitOfWork.Shelves.GetByCodeAsync(item.Code);

                    if (entity == null)
                    {
                        response.FailedCount++;
                        response.Errors.Add(new UpdateErrorDetail
                        {
                            Code = item.Code,
                            ErrorMessage = $"Shelf với code '{item.Code}' không tồn tại"
                        });
                        continue;
                    }

                    entity.ImageUrl = item.ImageUrl;
                    await _unitOfWork.Shelves.UpdateAsync(entity);

                    response.SuccessCount++;
                    response.SuccessCodes.Add(item.Code);

                    _logger.LogInformation($"Updated ImageUrl for Shelf: {item.Code}");
                }
                catch (Exception ex)
                {
                    response.FailedCount++;
                    response.Errors.Add(new UpdateErrorDetail
                    {
                        Code = item.Code,
                        ErrorMessage = ex.Message
                    });
                    _logger.LogError(ex, $"Error updating Shelf: {item.Code}");
                }
            }
        }

        private async Task UpdateFloorImagesAsync(List<ImageUrlUpdateItem> items, BatchUpdateImageUrlResponse response)
        {
            foreach (var item in items)
            {
                try
                {
                    var entity = await _unitOfWork.Floors.GetByCodeAsync(item.Code);

                    if (entity == null)
                    {
                        response.FailedCount++;
                        response.Errors.Add(new UpdateErrorDetail
                        {
                            Code = item.Code,
                            ErrorMessage = $"Floor với code '{item.Code}' không tồn tại"
                        });
                        continue;
                    }

                    entity.ImageUrl = item.ImageUrl;
                    await _unitOfWork.Floors.UpdateAsync(entity);

                    response.SuccessCount++;
                    response.SuccessCodes.Add(item.Code);

                    _logger.LogInformation($"Updated ImageUrl for Floor: {item.Code}");
                }
                catch (Exception ex)
                {
                    response.FailedCount++;
                    response.Errors.Add(new UpdateErrorDetail
                    {
                        Code = item.Code,
                        ErrorMessage = ex.Message
                    });
                    _logger.LogError(ex, $"Error updating Floor: {item.Code}");
                }
            }
        }

        private async Task UpdateContainerTypeImagesAsync(List<ImageUrlUpdateItem> items, BatchUpdateImageUrlResponse response)
        {
            foreach (var item in items)
            {
                try
                {
                    var itemId = int.Parse(item.Code);
                    var entity = await _unitOfWork.ContainerType.GetByIdAsync(itemId);

                    if (entity == null)
                    {
                        response.FailedCount++;
                        response.Errors.Add(new UpdateErrorDetail
                        {
                            Code = item.Code,
                            ErrorMessage = $"Container Type với code '{item.Code}' không tồn tại"
                        });
                        continue;
                    }

                    entity.ImageUrl = item.ImageUrl;
                    await _unitOfWork.ContainerType.UpdateAsync(entity);

                    response.SuccessCount++;
                    response.SuccessCodes.Add(item.Code);

                    _logger.LogInformation($"Updated ImageUrl for Container Type: {item.Code}");
                }
                catch (Exception ex)
                {
                    response.FailedCount++;
                    response.Errors.Add(new UpdateErrorDetail
                    {
                        Code = item.Code,
                        ErrorMessage = ex.Message
                    });
                    _logger.LogError(ex, $"Error updating Container Type: {item.Code}");
                }
            }
        }

        #endregion

    }
}
