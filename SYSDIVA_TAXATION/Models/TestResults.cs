using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace SYSDIVA_TAXATION.Models
{
    public class TestResults
    {
        public int ResultId { get; set; }          // Corresponds to PRIMARY KEY IDENTITY
        public string UserName { get; set; }       // NVARCHAR(100)
        public decimal TotalMarks { get; set; }    // DECIMAL(5,2)
        public DateTime SubmittedOn { get; set; }
    }
}
