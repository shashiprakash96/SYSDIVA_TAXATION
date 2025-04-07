using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace SYSDIVA_TAXATION.Models
{
    public class Question
    {
        public int Id  { get; set; }
        public string QuestionText { get; set; }
        public string OptionA { get; set; }
        public string OptionB { get; set; }
        public string OptionC { get; set; }
        public string OptionD { get; set; }
        public string CorrectOption { get; set; }
        public string SelectedOption { get; internal set; }
    }
}
