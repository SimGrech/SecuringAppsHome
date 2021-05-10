using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace ShoppingCart.Application.ViewModels
{
    public class SubmissionViewModel
    {
        public Guid Id { get; set; }
        
        [Required]
        public string FileName { get; set; }
        
        [Required]
        public string Description { get; set; }
        
        public string Path { get; set; }

        public string Signature { get; set; }

        public string Hash { get; set; }

        [Required]
        public DateTime TimeSubmitted { get; set; }

        public IList<CommentViewModel> Comments { get; set; }

        public AssignmentTaskViewModel AssignmentTask { get; set; }

        public string Owner { get; set; } //User Email
    }
}
