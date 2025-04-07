using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace SYSDIVA_TAXATION.Models
{
    public class TestViewModel
    {
        public string UserName { get; set; }
        public List<Question> Questions { get; set; }
        public List<UserAnswer> Answers { get; set; }
    }
}
