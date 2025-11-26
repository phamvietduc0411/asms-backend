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

        //public async Task CalculateOrderDetailsPriceAsync(List<int> details)
        //{
        //    var services = await _unitOfWork.OrderDetailServices.GetByIdsAsync(details);

        //    //var serviceMap = services.ToDictionary(x => x.ServiceId);
        //    decimal totalPrice = 0m;

        //    foreach (var id in details)
        //    {
        //        var service = serviceMap[id];
        //        decimal servicePrice = service.Price.GetValueOrDefault();
        //        totalPrice += servicePrice;
        //    }
        //}

        //public async Task StoragePriceAsync(List<int> details)
        //{
        //    //var services = await _unitOfWork.OrderDetailServices.GetByIdsAsync(details);

        //    //var serviceMap = services.ToDictionary(x => x.ServiceId);
        //    //decimal totalPrice = 0m;

        //    //foreach (var id in details)
        //    //{
        //    //    var service = serviceMap[id];
        //    //    decimal servicePrice = service.Price.GetValueOrDefault();
        //    //    totalPrice += servicePrice;
        //    //}
        //}
        #region Self Storage
        #endregion
        public async Task<decimal> ShelfPriceAsync(string storageCode)
        {
            if (string.IsNullOrEmpty(storageCode)) return 0;

            // get all list shelf of selfStorage
            var listshelf = await _unitOfWork.Shelves.GetAllShelvesByStorageCodeAsync(storageCode);

            decimal total = 0;

            foreach (var item in listshelf)
            {
                var price = item.ShelfType?.Price;
                if (price != null)
                {
                    total += price.Value;
                }
            }

            return total;
        }
    }
}
