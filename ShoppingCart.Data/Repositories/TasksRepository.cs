using ShoppingCart.Data.Context;
using ShoppingCart.Domain.Interfaces;
using ShoppingCart.Domain.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ShoppingCart.Data.Repositories
{
    public class TasksRepository : ITasksRepository
    {
        ShoppingCartDbContext _context;
        public TasksRepository(ShoppingCartDbContext context)
        {

            _context = context;
        }

        public void AddSubmission(Submission submission)
        {
            _context.Submissions.Add(submission);
            _context.SaveChanges();
        }

        public void AddTask(AssignmentTask task)
        {
            _context.AssignmentTasks.Add(task);
            _context.SaveChanges();
        }

        public Submission GetSubmission(Guid id)
        {
            return _context.Submissions.SingleOrDefault(x => x.Id == id);
        }

        //Get submissions based on taskId
        public IQueryable<Submission> GetSubmissions(Guid taskId)
        {
            return _context.Submissions.Where(x => x.TaskId == taskId);
        }

        public AssignmentTask GetTask(Guid id)
        {
            return _context.AssignmentTasks.SingleOrDefault(x => x.Id == id);
        }

        //Gets tasks according to teacher
        public IQueryable<AssignmentTask> GetTasks(string email)
        {
            return _context.AssignmentTasks.Where(x => x.Teacher == email);
        }
    }
}
