using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text;

namespace ShoppingCart.Domain.Models
{
    public class Comment
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public Guid Id { get; set; }

        public string Content { get; set; }

        public string Author { get; set; } //Author email

        public DateTime Posted { get; set; }

        public virtual Submission Submission { get; set; }

        [ForeignKey("Submission")]
        public Guid SubmissionId { get; set; }
    }
}
