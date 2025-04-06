using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace SYSDIVA_TAXATION.Models
{
    public class BillItem
    {
        public int FoodItemId { get; set; }
        public string FoodName { get; set; }
        public int Quantity { get; set; }
        public decimal SubTotal { get; set; }
    }
}
