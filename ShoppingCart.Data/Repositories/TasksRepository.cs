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

        //Get user Submissions
        public IQueryable<Submission> GetUserSubmissions(string email) {
            return _context.Submissions.Where(x => x.Owner == email);
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

        public IQueryable<Comment> GetSubmissionComments(Guid submissionId) {
            return _context.Comments.Where(x => x.SubmissionId == submissionId).OrderBy(x => x.Posted);
        }

        public void AddComment(Comment comment) {
            _context.Comments.Add(comment);
            _context.SaveChanges();
        }

        public bool SubmissionCopied(string hash) {
            IQueryable<Submission> submissions = _context.Submissions.Where(x => x.Hash == hash);
            int count = submissions.Count();
            if (count == 1)
            {
                return false;
            }
            else {
                return true;
            }
        }
    }
}
