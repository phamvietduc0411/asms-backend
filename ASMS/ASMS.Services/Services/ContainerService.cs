using ASMS.Repositories.Common;
using ASMS.Repositories.Entities;
using ASMS.Repositories.Infrastructures;
using ASMS.Services.Interfaces;
using ASMS.Services.Model.Container;
using AutoMapper;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;
using static Azure.Core.HttpHeader;

namespace ASMS.Services.Services
{
    public class ContainerService : IContainerService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;

        public ContainerService(IUnitOfWork unitOfWork, IMapper mapper)
        {
            _unitOfWork = unitOfWork;
            _mapper = mapper;
        }

        public async Task<PaginatedList<ContainerResponse>> GetAllAsync(int pageNumber, int pageSize)
        {
            var result = await _unitOfWork.Containers.GetAllAsync(pageNumber, pageSize);

            var mappedItems = _mapper.Map<List<ContainerResponse>>(result.Items);

            return new PaginatedList<ContainerResponse>(
                mappedItems,
                result.CurrentPage,
                result.PageSize,
                result.TotalRecords)
            {
                TotalPages = result.TotalPages
            };
        }

        public async Task<ContainerResponse?> GetByCodeAsync(string code)
        {
            var container = await _unitOfWork.Containers.GetByCodeAsync(code);
            return container == null ? null : _mapper.Map<ContainerResponse>(container);
        }

        public async Task<ContainerResponse> CreateAsync(CreateContainerRequest request)
        {
            var container = _mapper.Map<Container>(request);
            container.IsActive = true;

            await _unitOfWork.Containers.AddAsync(container);
            await _unitOfWork.CompleteAsync();

            return _mapper.Map<ContainerResponse>(container);
        }

        public async Task<ContainerResponse?> UpdateAsync(string code, UpdateContainerRequest request)
        {
            var container = await _unitOfWork.Containers.GetByCodeAsync(code);
            if (container == null) return null;

            _mapper.Map(request, container);
            await _unitOfWork.Containers.UpdateAsync(container);
            await _unitOfWork.CompleteAsync();

            return _mapper.Map<ContainerResponse>(container);
        }

        public async Task<bool> DeleteAsync(string code)
        {
            var container = await _unitOfWork.Containers.GetByCodeAsync(code);
            if (container == null) return false;

            await _unitOfWork.Containers.DeleteAsync(code);
            await _unitOfWork.CompleteAsync();
            return true;
        }

        public async Task<bool> UpdateContainerPositionAsync(UpdateContainerPositionRequest request)
        {

            var container = await _unitOfWork.Containers.GetByCodeAsync(request.ContainerCode);
            if (container == null)
            {
                return false;
            }

            container.PositionX = request.PositionX;
            container.PositionY = request.PositionY;
            container.PositionZ = request.PositionZ;

            await _unitOfWork.Containers.UpdateAsync(container);
            await _unitOfWork.CompleteAsync();

            return true;
        }
        public async Task<PlaceContainerResponse> PlaceContainerAsync(PlaceContainerRequest request)
        {
            try
            {
                var container = await _unitOfWork.Containers.GetByCodeAsync(request.ContainerCode);
                if(container == null)
                {
                    return new PlaceContainerResponse
                    {
                        Success = false,
                        Message = $"Container {request.ContainerCode} not found"
                    };
                }
                if(container.Status != "Available")
                {
                    return new PlaceContainerResponse
                    {
                        Success = true,
                        Message = $"Container {request.ContainerCode} is not available"
                    };
                }
                var floor = await _unitOfWork.Floors.GetByCodeAsync(request.FloorCode);
                if(floor == null)
                {
                    return new PlaceContainerResponse
                    {
                        Success = false,
                        Message = $"Floor {request.FloorCode} not found"
                    };
                }
                var oldFloor = container.FloorCode;
                if(request.RequiresRearrangement && !string.IsNullOrEmpty(request.RearrangeContainerCode))
                {
                    var rearrangeResult = await RearrangeFragileContainerAsync(request.RearrangeContainerCode, request.ContainerCode, request.OrderCode,
                        request.PerformedBy);
                    if(!rearrangeResult.Success)
                    {
                        return rearrangeResult;
                    }
                }
                container.FloorCode = request.FloorCode;
                container.Status = "Occupied";
                container.Layer =  request.Layer;
                container.SerialNumber = request.SerialNumber;
                if(request.Layer == 1)
                {
                    var belowContainerCode = await FindContainerBelowAsync(request.FloorCode, container.ContainerType);
                    if (string.IsNullOrEmpty(belowContainerCode))
                    {
                        return new PlaceContainerResponse
                        {
                            Success = false,
                            Message = "Cannot find suitable base container for Layer 1"
                        };
                    }
                    var belowContainer = await _unitOfWork.Containers.GetByCodeAsync(belowContainerCode);
                    if(belowContainer != null)
                    {
                        belowContainer.ContainerAboveCode = container.ContainerCode;
                        await _unitOfWork.Containers.UpdateAsync(belowContainer);
                    }
                }
                await _unitOfWork.Containers.UpdateAsync(container);
                //var logId = await GenerateContainerLocationLogIdAsync();
                var log = new ContainerLocationLog
                {
                    //ContainerLocationLogId = logId,
                    ContainerCode = container.ContainerCode,
                    OrderCode = request.OrderCode,
                    PerformedBy = request.PerformedBy,
                    UpdatedDate = DateOnly.FromDateTime(DateTime.Now),
                    OldFloor = oldFloor,
                    CurrentFloor = request.FloorCode,
                    Reason = "Container Placement",
                    Algorithm = "CLP",
                    Notes = $"Placed at Layer {request.Layer}, SerialNumber {request.SerialNumber}" +
                           (request.RequiresRearrangement ? " (with rearrangement)" : "")
                };
                await _unitOfWork.ContainerLocationLogs.AddAsync(log);
                await _unitOfWork.CompleteAsync();
                return new PlaceContainerResponse
                {
                    Success = true,
                    Message = "Container placed successfully",
                    ContainerCode = container.ContainerCode,
                    FloorCode = request.FloorCode,
                    Layer = request.Layer,
                    SerialNumber = request.SerialNumber,
                };

            }
            catch (Exception ex)
            {

                return new PlaceContainerResponse
                {
                    Success = false,
                    Message = ex.Message,
                };
            }
        }

        

        private async Task<PlaceContainerResponse> RearrangeFragileContainerAsync(string fragileContainerCode, string newContainerCode, string orderCode,
            string performedBy)
        {
            try
            {
                var fragileContainer = await _unitOfWork.Containers.GetByCodeAsync(fragileContainerCode);
                if (fragileContainer == null)
                {
                    return new PlaceContainerResponse
                    {
                        Success = false,
                        Message = $"Fragile container {fragileContainerCode} not found"
                    };
                }

                if (fragileContainer.Layer != 0)
                {
                    return new PlaceContainerResponse
                    {
                        Success = false,
                        Message = $"Fragile container {fragileContainerCode} is not at Layer 0"
                    };
                }

                var baseContainerCode = await FindSuitableBaseContainerAsync(
                    fragileContainer.FloorCode,
                    fragileContainer.ContainerType);

                if (string.IsNullOrEmpty(baseContainerCode))
                {
                    return new PlaceContainerResponse
                    {
                        Success = false,
                        Message = "Cannot find suitable base container for rearrangement"
                    };
                }

                // Di chuyển lên Layer 1
                fragileContainer.Layer = 1;

                var baseContainer = await _unitOfWork.Containers.GetByCodeAsync(baseContainerCode);
                if (baseContainer != null)
                {
                    baseContainer.ContainerAboveCode = fragileContainerCode;
                    await _unitOfWork.Containers.UpdateAsync(baseContainer);
                }

                await _unitOfWork.Containers.UpdateAsync(fragileContainer);

                // Log rearrangement
                //var logId = await GenerateContainerLocationLogIdAsync();
                var log = new ContainerLocationLog
                {
                    //ContainerLocationLogId = logId,
                    ContainerCode = fragileContainerCode,
                    OrderCode = orderCode,
                    PerformedBy = performedBy,
                    UpdatedDate = DateOnly.FromDateTime(DateTime.Now),
                    OldFloor = fragileContainer.FloorCode,
                    CurrentFloor = fragileContainer.FloorCode,
                    Reason = "Rearrangement",
                    Algorithm = "CLP",
                    Notes = $"Moved from Layer 0 to Layer 1 to make room for {newContainerCode}"
                };

                await _unitOfWork.ContainerLocationLogs.AddAsync(log);

                return new PlaceContainerResponse
                {
                    Success = true,
                    Message = "Rearrangement successful"
                };
            }
            catch (Exception ex)
            {
                return new PlaceContainerResponse
                {
                    Success = false,
                    Message = $"Error during rearrangement: {ex.Message}"
                };
            }
        }

        private async Task<string> FindSuitableBaseContainerAsync(string? floorCode, ContainerType? containerType)
        {
            var occupiedContainers = (await _unitOfWork.Containers.GetByFloorCodeAsync(floorCode)).ToList();
            var suitableBase = occupiedContainers
                .Where(c => c.Layer == 0
                       && c.ProductType?.IsFragile != true
                       && string.IsNullOrEmpty(c.ContainerAboveCode)
                       && c.ContainerType != null
                       && (c.ContainerType.Length * c.ContainerType.Width)
                          >= (containerType.Length * containerType.Width))
                .OrderByDescending(c => c.ContainerType.Length * c.ContainerType.Width)
                .FirstOrDefault();
            return suitableBase?.ContainerCode;
        }

        private async Task<string> FindContainerBelowAsync(
            string floorCode,
            ContainerType containerType)
        {
            var occupiedContainers = await _unitOfWork.Containers.GetByFloorCodeAsync(floorCode);
            var suitable = occupiedContainers
                .Where(c => c.Layer == 0
                       && string.IsNullOrEmpty(c.ContainerAboveCode)
                       && c.ProductType?.IsFragile != true
                       && c.ContainerType != null
                       && (c.ContainerType.Length * c.ContainerType.Width)
                          >= (containerType.Length * containerType.Width))
                .OrderByDescending(c => c.ContainerType.Length * c.ContainerType.Width)
                .FirstOrDefault();

            return suitable?.ContainerCode;
        }
        public async Task<RemoveContainerResponse> RemoveContainerAsync(string containerCode, string orderCode = null, string performedBy = null)
        {
            try
            {
                var container = await _unitOfWork.Containers.GetByCodeAsync(containerCode);
                if(container == null)
                {
                    return new RemoveContainerResponse
                    {
                        Success = false,
                        Message = $"Container {containerCode} not found"
                    };
                }
                if(container.Status != "Occupied")
                {
                    return new RemoveContainerResponse
                    {
                        Success = false,
                        Message = $"Container {containerCode} is not occupied"
                    };
                }
                if (!string.IsNullOrEmpty(container.ContainerAboveCode))
                {
                    return new RemoveContainerResponse
                    {
                        Success = false,
                        Message = $"Cannot remove container. Container {container.ContainerAboveCode} is stacked on top",
                        BlockingContainerCode = container.ContainerAboveCode
                    };
                }
                var oldFloor = container.FloorCode;
                var oldLayer = container.Layer;
                container.FloorCode = null;
                container.Status = "Available";
                container.Layer = null;
                container.SerialNumber = null;
                container.ContainerAboveCode = null;
                container.CurrentWeight = 0;
                await _unitOfWork.Containers.UpdateAsync(container);
                //var logId = await GenerateContainerLocationLogIdAsync();
                var log = new ContainerLocationLog
                {
                    //ContainerLocationLogId = logId,
                    ContainerCode = containerCode,
                    OrderCode = orderCode,
                    PerformedBy = performedBy,
                    UpdatedDate = DateOnly.FromDateTime(DateTime.Now),
                    OldFloor = oldFloor,
                    CurrentFloor = null,
                    Reason = "Container Removal",
                    Algorithm = null,
                    Notes = $"Removed from Layer {oldLayer}, returned to Available status"
                };

                await _unitOfWork.ContainerLocationLogs.AddAsync(log);
                await _unitOfWork.CompleteAsync();
                return new RemoveContainerResponse
                {
                    Success = true,
                    Message = "Container remove successfully",
                    ContainerCode = containerCode
                };
            }
            catch (Exception ex)
            {

                return new RemoveContainerResponse
                {
                    Success = false,
                    Message = $"Error removing container: {ex.Message}"
                };
            }
        }
        private async Task<int> GenerateContainerLocationLogIdAsync()
        {
            var lastLog = await _unitOfWork.ContainerLocationLogs.GetLastAsync();
            return (lastLog?.ContainerLocationLogId ?? 0) + 1;
        }
    }
}
