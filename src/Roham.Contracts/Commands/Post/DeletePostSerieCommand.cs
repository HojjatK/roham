/* Copyright - Roham 
 * This software may be modified and distributed under the terms of the MIT license.  See the LICENSE file for details.*/
using Roham.Lib.Domain.CQS.Command;

namespace Roham.Contracts.Commands.Post
{   
    public class DeletePostSerieCommand : AbstractCommand
    {
        public long Id { get; set; }
    }
}