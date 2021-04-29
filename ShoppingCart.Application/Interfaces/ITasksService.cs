using ShoppingCart.Application.ViewModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ShoppingCart.Application.Interfaces
{
    public interface ITasksService
    {
        AssignmentTaskViewModel GetTask(Guid id);

        void AddTask(AssignmentTaskViewModel task);

        //Gets tasks created by the teacher's email
        IQueryable<AssignmentTaskViewModel> GetTasks(string email);

        void AddSubmission(SubmissionViewModel submission);

        IQueryable<SubmissionViewModel> GetSubmissions(Guid taskId);

        //Gets a submission's info
        SubmissionViewModel GetSubmission(Guid id);

    }
}
