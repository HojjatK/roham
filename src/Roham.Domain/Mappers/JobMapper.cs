/* Copyright - Roham 
 * This software may be modified and distributed under the terms of the MIT license.  See the LICENSE file for details.*/
using System;
using System.Collections.Generic;
using Roham.Contracts.Dtos;
using Roham.Domain.Entities.Jobs;
using Roham.Lib.Domain;
using Roham.Lib.Ioc;


namespace Roham.Domain.Mappers
{
    [AutoRegister(LifetimeScope = LifetimeScopeType.SingleInstance)]
    public class JobMapper : IEntityMapper<JobDto, Job>
    {
        public JobDto Map(Job job)
        {
            var result = Convert(job);
            return result;
        }

        private JobDto Convert(Job job)
        {
            return new JobDto
            {
                Uid = job.Uid.ToString(),
                Id = job.Id,                
                Name = job.Name,
                Type = job.Type.ToString(),
                TypeDescription = JobDescription(job.Type),
                IsSystemJob = job.IsSystemJob,
                Description = job.Description,
                OwnerUserId = job.Owner?.Id,
                OwnerUser = job.Owner?.UserName,
                SiteId = job.Site?.Id,
                SiteTitle = job.Site?.Title
            };
        }

        private string JobDescription(JobType jobType)
        {
            switch(jobType)
            {
                case JobType.ImportEntries:
                    return "Import Blogs";                    
                case JobType.ExportEntries:
                    return "Export Blogs";
                default:
                    return jobType.ToString();
            }
        }
    }
}