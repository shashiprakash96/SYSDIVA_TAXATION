using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace SYSDIVA_TAXATION.Models
{
    public class BillViewModel
    {
        public string CustomerName { get; set; }
        public List<FoodItem> FoodItems { get; set; }
        public List<BillItem> SelectedItems { get; set; } 
        public decimal TotalAmount { get; set; }
    }
}
