using System;
using System.Collections.Generic;
using System.Linq;
using Roham.Lib.Domain.CQS.Command;
using Roham.Lib.Domain.Exceptions;
using Roham.Lib.Domain.Persistence;
using Roham.Domain.Entities.Posts;
using Roham.Contracts.Commands.Post;
using System.ComponentModel.DataAnnotations;

namespace Roham.Domain.Commands.Post
{
    public abstract class PersistPostCommandHandler<TCommand> : AbstractCommandHandler<TCommand>
        where TCommand : SavePostCommand
    {
        protected PersistPostCommandHandler(Func<IPersistenceUnitOfWorkFactory> uowFactoryResolver) : base(uowFactoryResolver) { }

        protected abstract bool NewRevisionRequired { get; }

        protected override void OnHandle(TCommand command)
        {
            string title = command.Title;
            var authorIdentity = command.UserName;
            using (var uow = UowFactory.Create())
            {
                Validate(command, uow);

                var revisor = uow.Context
                   .Query<Entities.Security.User>()
                   .SingleOrDefault(u => u.UserName == authorIdentity);
                var revisorName = revisor.Person != null ? revisor.Person.GetFullName() : revisor.UserName;

                var postStatus = Entities.Posts.PostStatus.Saved; // TODO: figure out the next status, based on user and workflow config
                var postFormat = Entities.Posts.ContentFormats.Html;
                var postTags = (command.TagsCommaSeparated ?? "").Split(new[] { ',' }, StringSplitOptions.RemoveEmptyEntries)
                    .Select(t => t.Trim())
                    .ToList();

                var site = uow.Context.FindById<Entities.Sites.Site>(command.SiteId);
                var tagEntities = new HashSet<Tag>();
                foreach (var tagName in postTags)
                {
                    var tagEntity = uow.Context.Query<Tag>().FirstOrDefault(t => t.Name == tagName);
                    if (tagEntity == null)
                    {
                        // TODO: send AddTag command instead
                        tagEntity = new Tag { Name = tagName, Site = site };
                        uow.Context.Add(tagEntity);
                    }

                    tagEntities.Add(tagEntity);
                }

                var post = uow.Context.FindById<Entities.Posts.Post>(command.PostId);
                if (post == null)
                {
                    throw new EntityNotFoundException(string.Format("Post with Id: {0} not found", command.PostId));
                }
                if (post.Site.Id != command.SiteId || post.Zone.Id != command.ZoneId)
                {
                    throw new Exception("Site/Zone are not valid");
                }

                DateTime? nowDate = DateTime.Now;
                post.Name = command.Title;
                post.Title = command.Title;
                post.MetaTitle = command.MetaTitle ?? title;
                post.MetaDescription = command.MetaDescription;
                post.PublishDate = postStatus == PostStatus.Published ? nowDate : null;
                post.EffectiveDate = postStatus == PostStatus.Published ? nowDate : null;
                post.Status = postStatus;
                post.Format = postFormat;
                post.IsPrivate = command.IsPrivate;
                post.IsDiscussionEnabled = command.IsRatingEnabled;
                post.IsAnonymousCommentAllowed = command.IsAnonymousCommentAllowed;
                post.IsRatingEnabled = command.IsRatingEnabled;

                post.Tags.Clear();
                tagEntities
                    .ForEach(tag => post.Tags.Add(tag));

                var newRevisionRequired = true; // TODO: 
                if (newRevisionRequired)
                {
                    var postRevision = post.Revise();
                    command.Links
                        .ForEach(l => postRevision.Post.Links.Add(new PostLink { Type = l.Key, Ref = l.Value, Post = postRevision.Post }));
                }
                else
                {
                    post.Links.Clear();
                    command.Links
                        .ForEach(l => post.Links.Add(new PostLink { Type = l.Key, Ref = l.Value, Post = post }));
                }

                post.LatestRevision.Summary = command.ContentSummary;
                post.LatestRevision.Body = command.Content;
                post.LatestRevision.Reviser = revisor;
                post.LatestRevision.Author = revisorName;

                uow.Context.Update(post);

                uow.Complete();
            }
        }

        private void Validate(SavePostCommand command, IPersistenceUnitOfWork uow)
        {
            // check duplicate name
            var postId = command.PostId;
            string newTitle = command.Title;
            if (uow.Context.Query<Entities.Posts.Post>().Any(p => p.Title == newTitle && p.Id != postId))
            {
                throw new ValidationException($"Another post with '{newTitle}' title already exist");
            }
            string newName = command.Name;
            if (uow.Context.Query<Entities.Posts.Post>().Any(p => p.Name == newName && p.Id != postId))
            {
                throw new ValidationException($"Another post with '{newName}' url already exist");
            }
        }
    }
}
