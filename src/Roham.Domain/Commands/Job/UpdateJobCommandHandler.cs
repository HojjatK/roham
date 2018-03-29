/* Copyright - Roham 
 * This software may be modified and distributed under the terms of the MIT license.  See the LICENSE file for details.*/
using System;
using System.Linq;
using Roham.Contracts.Commands.Job;
using Roham.Lib.Domain.CQS.Command;
using Roham.Lib.Domain.Persistence;
using Roham.Lib.Ioc;

namespace Roham.Domain.Commands.Job
{   
    [AutoRegister]
    public class UpdateJobCommandHandler : AbstractCommandHandler<UpdateJobCommand>
    {
        public UpdateJobCommandHandler(Func<IPersistenceUnitOfWorkFactory> uowFactoryResolver) : base(uowFactoryResolver) { }

        protected override void OnHandle(UpdateJobCommand command)
        {
            using (var uow = UowFactory.Create())
            {
                Validate(uow, command);
                var job = uow.Context.Query<Entities.Jobs.Job>().SingleOrDefault(j => j.Id == command.JobId);
                job.Description = command.Description;
                uow.Context.Update(job);

                uow.Complete();
            }
        }

        private void Validate(IPersistenceUnitOfWork uow, UpdateJobCommand command)
        {
            // TODO:
        }
    }
}