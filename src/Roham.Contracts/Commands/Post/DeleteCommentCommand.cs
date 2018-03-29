﻿using Roham.Lib.Domain.CQS.Command;

namespace Roham.Contracts.Commands.Post
{
    public class DeleteCommentCommand : AbstractCommand
    {
        public long PostId { get; set; }
        public long CommentId { get; set; }
    }
}
