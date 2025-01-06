using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Data.SqlClient;

namespace Autokauppa.model
{
    public class Auto
    {
        public int autoId { get; set; }
        public decimal hinta { get; set; }
        public DateTime rekisteripaivays { get; set; }
        public decimal moottoritila { get; set; }
        public int mittariluku { get; set; }
        public int MerkkiId { get; set; }
        public int MalliId { get; set; }
        public int VariId { get; set; }
        public int PolttoaineId { get; set; }

    }
}
