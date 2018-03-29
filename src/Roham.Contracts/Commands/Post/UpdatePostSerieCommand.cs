/* Copyright - Roham 
 * This software may be modified and distributed under the terms of the MIT license.  See the LICENSE file for details.*/
using Roham.Lib.Domain.CQS.Command;

namespace Roham.Contracts.Commands.Post
{
    public class UpdatePostSerieCommand : AbstractCommand
    {
        public long Id { get; set; }
        public string Name { get; set; }
        public string Title { get; set; }
        public string Description { get; set; }
        public bool IsPrivate { get; set; }
    }
}