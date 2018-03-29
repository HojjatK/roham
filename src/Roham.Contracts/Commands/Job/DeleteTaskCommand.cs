/* Copyright - Roham 
 * This software may be modified and distributed under the terms of the MIT license.  See the LICENSE file for details.*/
using Roham.Lib.Domain.CQS.Command;

namespace Roham.Contracts.Commands.Job
{
    public class DeleteTaskCommand : AbstractCommand
    {
        public long JobId { get; set; }

        public long TaskId { get; set; }
    }
}