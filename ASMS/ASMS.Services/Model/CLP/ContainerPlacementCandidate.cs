using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ASMS.Services.Model.CLP
{
    public class ContainerPlacementCandidate
    {
        public required Repositories.Entities.Container Container { get; set; }

        public required Repositories.Entities.Floor Floor { get; set; }
        public required Repositories.Entities.Shelf Shelf { get; set; }
        public required Repositories.Entities.Storage Storage { get; set; }
        public required Repositories.Entities.Building Building { get; set; }

        public decimal Score { get; set; }
    }
}
