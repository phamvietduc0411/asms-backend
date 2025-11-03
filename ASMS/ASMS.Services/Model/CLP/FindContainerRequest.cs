using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ASMS.Services.Model.CLP
{
    public class FindContainerRequest
    {
        public int ProductTypeID { get; set; }
        public decimal PackageLength { get; set; }
        public decimal PackageWidth { get; set; }
        public decimal PackageHeight { get; set; }
        public decimal PackageWeight { get; set; }
        public int StorageDays { get; set; } = 30;
        public bool IsFragile { get; set; }
    }
}
