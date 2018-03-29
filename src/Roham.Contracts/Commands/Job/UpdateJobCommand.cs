/* Copyright - Roham 
 * This software may be modified and distributed under the terms of the MIT license.  See the LICENSE file for details.*/
using Roham.Lib.Domain;
using Roham.Lib.Domain.CQS.Command;
using System.ComponentModel.DataAnnotations;

namespace Roham.Contracts.Commands.Job
{
    public class UpdateJobCommand : AbstractCommand
    {
        public long JobId { get; set; }

        [MaxLength(Lengths.Description)]
        public string Description { get; set; }
    }
}