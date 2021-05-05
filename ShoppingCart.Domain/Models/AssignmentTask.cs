using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text;

namespace ShoppingCart.Domain.Models
{
    public class AssignmentTask
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public Guid Id { get; set; }

        public string Title { get; set; }

        public string Description { get; set; }

        [Required]
        public DateTime Deadline { get; set; }

        public string Teacher { get; set; } //Teacher Email
    }
}
