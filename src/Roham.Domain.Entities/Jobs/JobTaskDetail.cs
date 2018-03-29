/* Copyright - Roham 
 * This software may be modified and distributed under the terms of the MIT license.  See the LICENSE file for details.*/

using System;
using System.ComponentModel.DataAnnotations;
using System.Text;
using Roham.Lib.Domain;

namespace Roham.Domain.Entities.Jobs
{
    public class JobTaskDetail : Identifiable
    {
        [Required]
        public virtual int TryNo { get; set; }

        [MaxLength(Lengths.Arguments)]
        public virtual string Arguments { get; set; }        

        [Required]
        public virtual TaskStatus Status { get; set; }        

        [Required]
        public virtual DateTime Started { get; set; }

        public virtual DateTime? Updated { get; set; }

        public virtual DateTime? Finished { get; set; }

        [Required]
        public virtual JobTask OwnerTask { get; set; }

        private StringBuilder outputLogBuilder;
        [MaxLength(8000)]
        public virtual string OutputLog
        {
            get
            {
                if (outputLogBuilder == null)
                {
                    return null;
                }
                return outputLogBuilder.ToString();
            }
            set { outputLogBuilder = new StringBuilder(value); }
        }

        public virtual void Append(string logText, params object[] args)
        {
            Updated = DateTime.UtcNow;
            outputLogBuilder.AppendFormat("[{0}] ", Updated.Value.ToString("yyyy-MM-dd HH:mm:ss"));
            outputLogBuilder.AppendFormat(logText, args);
            outputLogBuilder.AppendLine();
        }
    }
}