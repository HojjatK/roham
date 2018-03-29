/* Copyright - Roham 
 * This software may be modified and distributed under the terms of the MIT license.  See the LICENSE file for details.*/
using Roham.Contracts.Dtos;
using Roham.Domain.Entities.Jobs;
using Roham.Lib.Domain;
using Roham.Lib.Ioc;
using System.Collections.Generic;

namespace Roham.Domain.Mappers
{
    [AutoRegister(LifetimeScope = LifetimeScopeType.SingleInstance)]
    public class JobTaskMapper : IEntityMapper<TaskDto, JobTask>
    {
        public TaskDto Map(JobTask task)
        {
            var result = Convert(task);
            return result;
        }

        private TaskDto Convert(JobTask task)
        {
            var taskDto = new TaskDto
            {
                Uid = task.Uid.ToString(),
                Id = task.Id,
                JobId = task.Job.Id,
                JobName = task.Job.Name,
                Name = task.Name,
                Status = task.Status.ToString(),
                ProgressEstimate = task.ProgressEstimate,
                Created= task.Created,
                Completed = task.Completed,
                FailedMessage = task.FailedMessage,
                OwnerUserName = task.OwnerUserName,
            };
            taskDto.Details = ConvertDetails(task);
            return taskDto;
        }

        private string JobDescription(JobType jobType)
        {
            switch (jobType)
            {
                case JobType.ImportEntries:
                    return "Import Blogs";
                case JobType.ExportEntries:
                    return "Export Blogs";
                default:
                    return jobType.ToString();
            }
        }

        private List<TaskDetailDto> ConvertDetails(JobTask task)
        {
            var result = new List<TaskDetailDto>();
            if (task.Details != null)
            {
                foreach(var detail in task.Details)
                {
                    result.Add(new TaskDetailDto
                    {   
                        Id = detail.Id,
                        TaskId = task.Id,
                        TryNo = detail.TryNo,
                        Arguments = detail.Arguments,
                        Status = detail.Status.ToString(),
                        Started = detail.Started,
                        Updated = detail.Updated,
                        Finished = detail.Finished,
                        OutputLog = detail.OutputLog,
                    });
                }
            }
            return result;
        }
    }
}