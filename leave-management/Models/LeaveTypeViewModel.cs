using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace leave_management.Models
{
    public class DetailsLeaveTypeViewModel
    {
        public int Id { get; set; }
        [Required]
        public string Name { get; set; }
        [Required]
        [Display(Name= "Number Of Days")]
        [Range(1,25, ErrorMessage = "Please, Enter a number between 1 and 25.")]
        public int DefaultNumberOfDays { get; set; }
        [Display(Name="Date Created")]
        public DateTime? DateCreated { get; set; }
    }
    public class CreateLeaveTypeViewModel
    {
        [Required]
        public string Name { get; set; }
    }
}
