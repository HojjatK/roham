using System;
using System.Collections.Generic;
using System.Linq;
using Roham.Lib.Domain.CQS.Command;
using Roham.Lib.Domain.Persistence;
using Roham.Domain.Entities.Posts;
using Roham.Contracts.Commands.Post;
using Roham.Lib.Ioc;
using System.ComponentModel.DataAnnotations;

namespace Roham.Domain.Commands.Post
{
    [AutoRegister]
    public class NewPostCommandHandler : AbstractCommandHandler<NewPostCommand>
    {
        public NewPostCommandHandler(Func<IPersistenceUnitOfWorkFactory> uowFactoryResolver) : base(uowFactoryResolver) { }

        protected override void OnHandle(NewPostCommand command)
        {
            string title = command.Title;
            var authorIdentity = command.UserName;
            using (var uow = UowFactory.Create())
            {
                Validate(command, uow);

                var user = uow.Context
                    .Query<Entities.Security.User>()
                    .SingleOrDefault(u => u.UserName == authorIdentity);
                var author = user.Person != null ? user.Person.GetFullName() : user.UserName;

                var site = uow.Context.FindById<Entities.Sites.Site>(command.SiteId);
                var zone = uow.Context.FindById<Entities.Sites.Zone>(command.ZoneId);
                PostSerie serie = null;
                if (command.SerieId > 0)
                {
                    serie = uow.Context.FindById<PostSerie>(command.SerieId);
                }

                var postStatus = PostStatus.Saved; // TODO: figure out the next status, based on user and workflow config
                var postFormat = ContentFormats.Html;
                var postTags = (command.TagsCommaSeparated ?? "").Split(new[] { ',' }, StringSplitOptions.RemoveEmptyEntries)
                    .Select(t => t.Trim())
                    .ToList();

                var tagEntities = new HashSet<Tag>();
                foreach (var tagName in postTags)
                {
                    var tagEntity = uow.Context.Query<Tag>().FirstOrDefault(t => t.Name == tagName);
                    if (tagEntity == null)
                    {
                        // TODO: send AddTag command instead
                        tagEntity = new Tag {  Name = tagName, Site = site };
                        uow.Context.Add(tagEntity);
                    }

                    tagEntities.Add(tagEntity);
                }

                var nowDate = DateTime.Today.ToUniversalTime();
                var newPost = new Entities.Posts.Post
                {
                    Site = site,
                    Zone = zone,
                    Name = title,
                    Title = title,
                    MetaTitle = command.MetaTitle ?? title,
                    MetaDescription = command.MetaDescription,
                    Author = author,
                    Serie = serie,
                    Created = nowDate,
                    Creator = user,
                    PublishDate = postStatus == PostStatus.Published ? (DateTime?)nowDate : null,
                    EffectiveDate = postStatus == PostStatus.Published ? (DateTime?)nowDate : null,
                    Status = postStatus,
                    Format = postFormat,
                    IsPrivate = command.IsPrivate,
                    IsDiscussionEnabled = command.IsDiscussionEnabled,
                    IsAnonymousCommentAllowed = command.IsAnonymousCommentAllowed,
                    IsRatingEnabled = command.IsRatingEnabled,
                    IsContentBinary = false,
                    IsPingbackEnabled = true,
                    IsTrackbackEnabled = true,
                    IsChromeHidden = false,
                };

                command.Links
                    .ForEach(l => newPost.Links.Add(new PostLink { Type = l.Key, Ref = l.Value, Post = newPost }));
                tagEntities
                    .ForEach(tag => newPost.Tags.Add(tag));

                newPost.Revise();
                newPost.LatestRevision.Summary = command.ContentSummary;
                newPost.LatestRevision.Body = command.Content;
                newPost.LatestRevision.Reviser = user;
                newPost.LatestRevision.Author = author;

                uow.Context.Add(newPost);

                uow.Complete();
            }
        }

        private void Validate(NewPostCommand command, IPersistenceUnitOfWork uow)
        {
            // check duplicate name
            string newTitle = command.Title;
            if (uow.Context.Query<Entities.Posts.Post>().Any(p => p.Title == newTitle))
            {
                throw new ValidationException($"Post with '{newTitle}' title already exist");
            }
            string newName = command.Name;
            if (uow.Context.Query<Entities.Posts.Post>().Any(p => p.Name == newName))
            {
                throw new ValidationException($"Post with '{newName}' url already exist");
            }
        }
    }
}
