using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ASMS.Services.Model.CLP
{
    public class CreateOrderWithCLPRequest
    {
        public string CustomerCode { get; set; } = null!;
        public DateOnly? DepositeDate { get; set; }
        public DateOnly? ReturnDate { get; set; }
        public string? PickupAddress { get; set; }
        public string EmployeeCode { get; set; }
        public int ProductTypeID { get; set; }
        public decimal PackageLength { get; set; }
        public decimal PackageWidth { get; set; }
        public decimal PackageHeight { get; set; }
        public decimal PackageWeight { get; set; }
        public List<ItemRequestDto> Items { get; set; } = new();

    }
}
