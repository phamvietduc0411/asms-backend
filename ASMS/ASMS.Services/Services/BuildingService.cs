using ASMS.Repositories.Entities;
using ASMS.Repositories.Infrastructures;
using ASMS.Repositories.Interfaces;
using ASMS.Services.Interfaces;
using ASMS.Services.Model.Building;
using AutoMapper;

namespace ASMS.Services.Services
{
    public class BuildingService : IBuildingService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;
        public BuildingService(IUnitOfWork unitOfWork, IMapper mapper)
        {
            _unitOfWork = unitOfWork;
            _mapper = mapper;
        }

        public async Task<Building> AddBuildingAsync(CreateBuildingRequest request)
        {
            var newBuilding = _mapper.Map<Building>(request);
            await _unitOfWork.Building.AddAsync(newBuilding);
            await _unitOfWork.CompleteAsync();
            return newBuilding;
        }

        public async Task<Building> GetByIdAsync(int id)
        {
            var building = await _unitOfWork.Building.GetEntityByIdAsync(id);
            if (building == null)
            {
                return null;
            }
            return building;
        }

        public async Task<Building> UpdateBuildingAsync(Building building)
        {
            await _unitOfWork.Building.UpdateAsync(building);
            await _unitOfWork.CompleteAsync();
            return building;
        }

        public async Task<string> GetLastRecord()
        {
            var latRecord = await _unitOfWork.Building.GetLastRecord();

            int nextNumber = 1;

            if (latRecord != null && !string.IsNullOrWhiteSpace(latRecord.BuildingCode))
            {
                string code = latRecord.BuildingCode.Trim();

                if (code.StartsWith("BLD", StringComparison.OrdinalIgnoreCase))
                {
                    string numberPart = code.Substring(3);
                    if (int.TryParse(numberPart, out int currentNumber))
                    {
                        nextNumber = currentNumber + 1;
                    }
                }
            }
            string newCode = $"BLD{nextNumber:D3}";

            return newCode;
        }


    }
}
