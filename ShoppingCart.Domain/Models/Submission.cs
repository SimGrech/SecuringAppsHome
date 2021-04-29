using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text;

namespace ShoppingCart.Domain.Models
{
    public class Submission
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public Guid Id { get; set; }
        public string FileName { get; set; } //optional
        public string Description { get; set; } //optional

        public string Path { get; set; }
        public string Signature { get; set; }

        public DateTime TimeSubmitted { get; set; }

        [Required]
        public virtual AssignmentTask Task { get; set; }

        [ForeignKey("Task")]
        public Guid TaskId { get; set; }

        public string Owner { get; set; } //student email

    }
}
