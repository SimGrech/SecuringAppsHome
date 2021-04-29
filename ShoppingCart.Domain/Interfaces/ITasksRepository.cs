using ShoppingCart.Domain.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ShoppingCart.Domain.Interfaces
{
    public interface ITasksRepository
    {
        AssignmentTask GetTask(Guid id);

        void AddTask(AssignmentTask task);

        //Gets tasks created by the teacher's email
        IQueryable<AssignmentTask> GetTasks(string email);

        void AddSubmission(Submission submission);

        //Gets a submission's info
        Submission GetSubmission(Guid id);

        //Gets all submissions under the specified task id
        IQueryable<Submission> GetSubmissions(Guid taskId);


    }
}
