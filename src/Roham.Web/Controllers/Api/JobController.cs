using System;
using System.Collections.Generic;
using System.Web.Http;
using Roham.Contracts.Dtos;
using Roham.Contracts.Queries;
using Roham.Domain.Entities.Jobs;
using Roham.Domain.Entities.Security;
using Roham.Lib.Domain.CQS.Command;
using Roham.Lib.Domain.CQS.Query;
using Roham.Contracts.Commands.Job;

namespace Roham.Web.Controllers.Api
{
    [RoutePrefix("api/job")]
    [Authorize(Roles = SecurityRoleNames.SysAdmin_Admin)]
    public class JobController : ApiControllerBase
    {
        public JobController(
            IQueryExecutor queryExecutor,
            ICommandDispatcher commandDispatcher) : base(queryExecutor, commandDispatcher)
        {   
        }

        [HttpGet]
        [Route("")]
        public List<JobDto> GetJobs()
        {
            return QueryExecutor.Execute(new FindAllQuery<JobDto, Job>());
        }

        [HttpGet]
        [Route("{id:long}")]
        public JobDto GetJob(long id)
        {
            return QueryExecutor.Execute(new FindByIdQuery<JobDto, Job>(id));
        }

        [HttpPost]
        [Route("")]        
        public ResultDto NewJob(JobDto jobDto)
        {
            return Result(() =>
            {   
                long? ownerId = GetCurrentSessionUser()?.Id;

                var command = new AddJobCommand
                {   
                    Name = jobDto.Name,
                    Type = jobDto.Type,
                    IsSystemJob = jobDto.IsSystemJob,
                    Description = jobDto.Description,
                    OwnerUserId = ownerId, 
                    Created = DateTime.Now,
                    SiteId = jobDto.SiteId,
                };
                CommandDispatcher.Send(command);
            });
        }

        [HttpPut]
        [Route("{id:long}")]        
        public ResultDto UpdateJob(long id, JobDto jobDto)
        {
            return Result(() => {
                var command = new UpdateJobCommand
                {
                    JobId = jobDto.Id,
                    Description = jobDto.Description,
                };
                CommandDispatcher.Send(command);
            });
        }

        [HttpDelete]
        [Route("{id:long}")]        
        public ResultDto DeleteJob(long id)
        {
            return Result(() =>
            {
                var command = new DeleteJobCommand
                {
                    JobId = id
                };
                CommandDispatcher.Send(command);
            });
        }

        [HttpGet]
        [Route("{jobId:long}/task")]
        public List<TaskDto> GetTasks(long jobId)
        {
            return QueryExecutor.Execute(new FindAllQuery<TaskDto, Job>());
        }

        [HttpGet]
        [Route("{jobId:long}/task/{id:long}")]
        public TaskDto GetTask(long jobId, long id)
        {
            return QueryExecutor.Execute(new FindByIdQuery<TaskDto, JobTask>(id));
        }

        [HttpPost]
        [Route("{jobId:long}/task")]
        public ResultDto NewTask(long jobId, TaskDto taskDto)
        {
            return Result(() =>
            {
                var command = new ExecuteTaskCommand
                {
                    JobId = jobId,
                    Name = taskDto.Name,
                    OwnerUserName = taskDto.OwnerUserName,
                };
                CommandDispatcher.Send(command);
            });
        }

        [HttpDelete]
        [Route("{jobId:long}/task/{id:long}")]
        public ResultDto DeleteTask(long jobId, long id)
        {
            return Result(() =>
            {
                var command = new DeleteTaskCommand
                {
                    JobId = jobId,
                    TaskId = id,
                };
                CommandDispatcher.Send(command);
            });
        }
    }
}