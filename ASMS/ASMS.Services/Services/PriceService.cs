using ASMS.Repositories.Entities;
using ASMS.Repositories.Infrastructures;

namespace ASMS.Services.Services
{
    public class PriceService
    {
        private readonly IUnitOfWork _unitOfWork;
        public PriceService(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;

        }

        public async Task CalculateOrderDetailsPriceAsync(List<OrderDetail> details)
        {

            var serviceIds = details
                .Select(d => d.ServiceId)
                .Distinct()
                .ToList();

            var services = await _unitOfWork.Services.GetByIdsAsync(serviceIds);

            var serviceMap = services.ToDictionary(x => x.ServiceId);
            decimal totalPrice = 0m;

            //foreach (var d in details)
            //{
            //    var service = serviceMap[d.ServiceId];

            //    decimal servicePrice = service.Price.GetValueOrDefault();

            //    totalPrice += servicePrice;
            //}
        }
    }
}
