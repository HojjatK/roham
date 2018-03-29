/* Copyright - Roham 
 * This software may be modified and distributed under the terms of the MIT license.  See the LICENSE file for details.*/
using Roham.Contracts.Dtos;
using Roham.Domain.Entities.Posts;
using Roham.Lib.Domain;
using Roham.Lib.Ioc;
using System;
using System.Text;

namespace Roham.Domain.Mappers
{
    public abstract class PostItemMapper<T>
    {
        protected void Fill(PostItemDto<T> postItemDto, Post post)
        {
            postItemDto.Uid = post.Uid.ToString();
            postItemDto.Id = post.Id;
            postItemDto.RevisionNumber = post.LatestRevision.RevisionNumber;
            postItemDto.SiteId = post.Site.Id;
            postItemDto.SiteName = post.Site.Name;
            postItemDto.SiteTitle = post.Site.Title;
            postItemDto.ZoneId = post.Zone.Id;
            postItemDto.ZoneName = post.Zone.Name;
            postItemDto.ZoneTitle = post.Zone.Title;
            postItemDto.SerieId = post.Serie != null ? post.Serie.Id : 0;
            postItemDto.SerieTitle = post.Serie != null ? post.Serie.Title : null;
            postItemDto.Name = post.Name;            
            postItemDto.Title = post.Title;
            postItemDto.Author = post.Author;
            postItemDto.IsPrivate = post.IsPrivate;
            postItemDto.CommentsCount = post.CommentsCount;
            postItemDto.Rating = post.Rating;
            postItemDto.Status = post.Status.ToString();
            postItemDto.WorkflowStatus = ""; // TODO:
            postItemDto.Created = post.Created;
            postItemDto.Published = post.PublishDate;
            postItemDto.DisplayDate = post.EffectiveDate;
            postItemDto.TagsCommaSeparated = post.TagsCommaSeparated;

            postItemDto.Uri = $"{postItemDto.SiteName}/{postItemDto.ZoneName}/{postItemDto.Name}";
        }

        protected string GetBodyContent(Post post)
        {
            if (post.IsContentBinary)
            {
                Encoding e = null;
                string encodingStr = (post.LatestRevision.BodyEncoding ?? "").Trim().ToUpper();
                switch (encodingStr)
                {
                    case "UTF8":
                        e = Encoding.UTF8;
                        break;
                    case "UNICODE":
                        e = Encoding.Unicode;
                        break;
                    case "ASCII":
                        e = Encoding.ASCII;
                        break;
                    case "UTF32":
                        e = Encoding.UTF32;
                        break;
                    case "UTF7":
                        e = Encoding.UTF7;
                        break;
                    default:
                        throw new NotSupportedException(string.Format("{0} encoding is not supported", encodingStr));
                }
                return e.GetString(post.LatestRevision.BodyImage);
            }
            else
            {
                return post.LatestRevision.Body;
            }
        }
    }

    [AutoRegister(LifetimeScope = LifetimeScopeType.SingleInstance)]
    public class PostItemMapper : PostItemMapper<PostItemDto>, IEntityMapper<PostItemDto, Post>
    {
        public PostItemDto Map(Post post)
        {
            var result = new PostItemDto();
            Fill(result, post);
            return result;
        }
    }
}