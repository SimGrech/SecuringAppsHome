using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace ShoppingCart.Application.ViewModels
{
    public class AssignmentTaskViewModel
    {
        public Guid Id { get; set; }

        
        public DateTime Deadline { get; set; }

        [Required]
        public string Title { get; set; }

        [Required]
        public string Description { get; set; }

        public string Teacher { get; set; }

    }
}
