/* Copyright - Roham 
 * This software may be modified and distributed under the terms of the MIT license.  See the LICENSE file for details.*/
using System;
using System.ComponentModel.DataAnnotations;
using Roham.Lib.Domain;

namespace Roham.Contracts.Dtos
{
    public class TaskDetailDto
    {
        public long Id { get; set; }

        public long TaskId { get; set; }
                
        public int TryNo { get; set; }

        [MaxLength(Lengths.Arguments)]
        public string Arguments { get; set; }

        public string Status { get; set; }

        public DateTime Started { get; set; }

        public DateTime? Updated { get; set; }

        public DateTime? Finished { get; set; }
                
        public string OutputLog { get; set; }
    }
}