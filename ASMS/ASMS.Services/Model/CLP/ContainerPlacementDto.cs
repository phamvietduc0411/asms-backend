using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ASMS.Services.Model.CLP
{
    public class ContainerPlacementDto
    {
        public string ContainerCode { get; set; }
        public string ContainerType { get; set; }
        public decimal Length { get; set; }
        public decimal Width { get; set; }
        public decimal Height { get; set; }

        // === Thông tin Building (MỚI) ===
        public int BuildingId { get; set; }
        public string BuildingName { get; set; }

        /// <summary>
        /// True nếu package quá cỡ, cần kho riêng
        /// </summary>
        public bool IsOversized { get; set; }

        // === Thông tin vị trí ===
        public string FloorCode { get; set; }
        public int FloorNumber { get; set; }
        public string ShelfCode { get; set; }
        public string StorageCode { get; set; }


        public decimal PositionX { get; set; }
        public decimal PositionY { get; set; }
        public decimal PositionZ { get; set; }

        public int Layer { get; set; }

        public int SerialNumber { get; set; }
        public string ContainerAboveCode { get; set; }

        public bool IsFragile { get; set; }

        public double Score { get; set; }



        public bool RequiresRearrangement { get; set; }

        public string RearrangeContainerCode { get; set; }
    }
}
