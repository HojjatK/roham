/* Copyright - Roham 
 * This software may be modified and distributed under the terms of the MIT license.  See the LICENSE file for details.*/
using System;
using System.Linq;
using Roham.Contracts.Commands.Job;
using Roham.Domain.Entities.Jobs;
using Roham.Lib.Domain.CQS.Command;
using Roham.Lib.Domain.Persistence;
using Roham.Lib.Ioc;

namespace Roham.Domain.Commands.Job
{
    [AutoRegister]
    public class AddJobCommandHandler : AbstractCommandHandler<AddJobCommand>
    {
        public AddJobCommandHandler(Func<IPersistenceUnitOfWorkFactory> uowFactoryResolver) : base(uowFactoryResolver) { }

        protected override void OnHandle(AddJobCommand command)
        {
            using (var uow = UowFactory.Create())
            {
                Validate(uow, command);

                var jobType = (JobType)Enum.Parse(typeof(JobType), command.Type);

                var ownerId = command.OwnerUserId;
                var siteId = command.SiteId;
                var owner = uow.Context.Query<Entities.Security.User>().SingleOrDefault(u => u.Id == ownerId);
                var site = uow.Context.Query<Entities.Sites.Site>().SingleOrDefault(s => s.Id == siteId);

                var newJob = new Entities.Jobs.Job
                {
                    Name = command.Name,
                    Type = jobType,
                    IsSystemJob = command.IsSystemJob,
                    Description = command.Description,
                    Created = command.Created,
                    Owner = owner,
                    Site = site,
                };
                uow.Context.Add(newJob);

                uow.Complete();
            }
        }

        private void Validate(IPersistenceUnitOfWork uow, AddJobCommand command)
        {
            // TODO:
        }
    }
}