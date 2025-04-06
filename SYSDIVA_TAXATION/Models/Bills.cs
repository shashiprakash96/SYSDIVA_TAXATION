using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace SYSDIVA_TAXATION.Models
{
    public class Bills
    {
        public int BillId { get; set; }

        public string CustomerName { get; set; }

        public decimal TotalAmount { get; set; }

        public DateTime CreatedOn { get; set; }
    }
}
