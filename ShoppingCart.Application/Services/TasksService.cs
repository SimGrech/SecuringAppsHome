using AutoMapper;
using AutoMapper.QueryableExtensions;
using ShoppingCart.Application.Interfaces;
using ShoppingCart.Application.ViewModels;
using ShoppingCart.Domain.Interfaces;
using ShoppingCart.Domain.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ShoppingCart.Application.Services
{
    public class TasksService : ITasksService
    {
        private ITasksRepository _tasksRepo;
        private IMapper _mapper;

        public TasksService(ITasksRepository tasksRepository, IMapper mapper) {
            _tasksRepo = tasksRepository;
            _mapper = mapper;
        }

        public void AddSubmission(SubmissionViewModel submission)
        {
            var userSubmission = _mapper.Map<Submission>(submission);
            userSubmission.Task = null;
            _tasksRepo.AddSubmission(userSubmission);
            //throw new NotImplementedException();
        }

        public void AddTask(AssignmentTaskViewModel task)
        {
            var myTask = _mapper.Map<AssignmentTask>(task);
            _tasksRepo.AddTask(myTask);
            //throw new NotImplementedException();
        }

        public SubmissionViewModel GetSubmission(Guid id)
        {
            var mySubmission = _tasksRepo.GetSubmission(id);
            SubmissionViewModel myModel = _mapper.Map<SubmissionViewModel>(mySubmission);

            return myModel;
        }

        public IQueryable<SubmissionViewModel> GetSubmissions(Guid taskId)
        {
            var assignmentTasks = _tasksRepo.GetSubmissions(taskId).ProjectTo<SubmissionViewModel>(_mapper.ConfigurationProvider);
            return assignmentTasks;
            //throw new NotImplementedException();
        }

        public AssignmentTaskViewModel GetTask(Guid id)
        {
            var myTask = _tasksRepo.GetTask(id);
            AssignmentTaskViewModel myModel = _mapper.Map<AssignmentTaskViewModel>(myTask);
            return myModel;
            //throw new NotImplementedException();
        }

        public IQueryable<AssignmentTaskViewModel> GetTasks(string email)
        {
            var myTasks = _tasksRepo.GetTasks(email).ProjectTo<AssignmentTaskViewModel>(_mapper.ConfigurationProvider);
            return myTasks;
            //throw new NotImplementedException();
        }
    }
}
