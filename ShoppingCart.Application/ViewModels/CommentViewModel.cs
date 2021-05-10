using System;
using System.Collections.Generic;
using System.Text;

namespace ShoppingCart.Application.ViewModels
{
    public class CommentViewModel
    {
        public Guid Id { get; set; }

        public string Content { get; set; }

        public string Author { get; set; } //Author email

        public DateTime Posted { get; set; }

        public SubmissionViewModel Submission { get; set; }

    }
}
