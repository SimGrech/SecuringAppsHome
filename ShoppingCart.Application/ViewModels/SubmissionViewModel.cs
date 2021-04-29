using System;
using System.Collections.Generic;
using System.Text;

namespace ShoppingCart.Application.ViewModels
{
    public class SubmissionViewModel
    {
        public Guid Id { get; set; }
        
        public string FileName { get; set; }
        
        public string Description { get; set; }
        
        public string Path { get; set; }

        public string Signature { get; set; }

        public DateTime TimeSubmitted { get; set; }

        public AssignmentTaskViewModel AssignmentTask { get; set; }

        public string Owner { get; set; } //User Email
    }
}
