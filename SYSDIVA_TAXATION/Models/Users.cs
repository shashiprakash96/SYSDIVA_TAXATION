using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace SYSDIVA_TAXATION.Models
{
    public class Users
    {
        public int Id { get; set; }
        [Required(ErrorMessage = "Name is required")]
        public string Name { get; set; }
        [Required(ErrorMessage = "Email is required")]
        public string Email { get; set; }
        [Required(ErrorMessage = "Age is required")]
        public int Age { get; set; }
        [Required(ErrorMessage = "Salary is required")]
        public decimal Salary { get; set; }
        [Required(ErrorMessage = "Gender is required")]
        public string Gender { get; set; }
       // [Required(ErrorMessage = "Name is required")]
        public DateTime CreatedOn { get; set; }
        public bool IsActive { get; set; }
    }
}
