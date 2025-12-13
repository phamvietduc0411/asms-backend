using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ASMS.Services.Model.Dashboard
{
    public class OrderStatisticsResponse
    {
        public string Date { get; set; }
        public string Type { get; set; }
        public DateOnly StartDate { get; set; }
        public DateOnly EndDate { get; set; }
        public string? Status { get; set; }
        public int TotalOrders { get; set; }
    }
}
