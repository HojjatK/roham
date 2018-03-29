using System;
using System.Collections.Generic;
using FluentNHibernate.Testing;
using NUnit.Framework;
using Roham.Domain.Entities.Posts;
using Roham.Domain.Entities.Sites;
using Roham.Lib.Strings;
using Roham.Domain.Entities.Entries;

namespace Roham.Persistence.NHibernate.Mappings.Posts
{
    public class PostMappingTest
    {
        [TestFixture]
        [Category("SmokeTests.NHibernate.Mapping")]
        internal class GivenPostEntity : NHibernateEntityMappingFixture
        {
            [Test]
            public void TestPostMapping()
            {
                // arrange
                var user1 = GetOrCreateUser("test.moderator1");
                var site1 = GetOrCreateSite("site1");
                var zone1 = GetOrCreateZone(site1, "blog zone");
                var category1 = GetOrCreateCategory(site1, "test category");
                var tag1 = GetOrCreateTag(site1, "tag1");
                var tag2 = GetOrCreateTag(site1, "tag2");
                var serie1 = GetOrCreateSerie(site1, "test serie1");

                // assert
                new PersistenceSpecification<Post>(Session, new CustomEqualityComparer())
                    .CheckProperty(e => e.Name, new PageName("An awesome test blog post"))
                    .CheckProperty(e => e.Title, "An awesome test blog post")
                    .CheckProperty(e => e.MetaTitle, "meta title")
                    .CheckProperty(e => e.MetaDescription, "meta description")
                    .CheckProperty(e => e.Format, ContentFormats.Html)
                    .CheckProperty(e => e.Author, "test author")
                    .CheckProperty(e => e.CommentsCount, 3)
                    .CheckProperty(e => e.ViewsCount, (long)2768282)
                    .CheckProperty(e => e.PageTemplate, "default")                    
                    .CheckProperty(e => e.Status, PostStatus.Saved)
                    .CheckProperty(e => e.Created, DateTime.Now)
                    .CheckProperty(e => e.PublishDate, null)
                    .CheckProperty(e => e.EffectiveDate, DateTime.Now.AddDays(1))
                    .CheckProperty(e => e.DisableDiscussionDays, 30)
                    .CheckProperty(e => e.Rating, (decimal)0.53m)
                    .CheckProperty(e => e.Popularity, (decimal)2.45m)
                    .CheckProperty(e => e.IsDiscussionEnabled, true)
                    .CheckProperty(e => e.IsRatingEnabled, true)
                    .CheckProperty(e => e.IsPrivate, false)
                    .CheckProperty(e => e.IsChromeHidden, true)
                    .CheckProperty(e => e.IsContentBinary, false)
                    .CheckProperty(e => e.IsPingbackEnabled, true)
                    .CheckProperty(e => e.IsTrackbackEnabled, true)
                    .CheckProperty(e => e.IsAnonymousCommentAllowed, false)
                    .CheckProperty(e => e.LatestRevision, new PostRevision
                    {
                        RevisionNumber = 2,
                        Body = "<div><p>test body for <span>an awesome test blog</span></p></div>",
                        TagsCommaSeperated = "test tag",
                        Summary = "test summary",                        
                        Format = ContentFormats.Html,
                        RevisedDate = DateTime.Now,
                        ViewsCount = 13,
                        Reviser = user1
                    })
                    .CheckReference(e => e.Zone, zone1)
                    .CheckReference(e => e.Serie, serie1)
                    .CheckReference(e => e.Creator, user1)
                    .CheckReference(e => e.Site, site1)
                    .CheckInverseList(e => e.Ratings, new List<Rating> {
                        new Rating { Rate=0.56m, RatedDate=DateTime.Now.Subtract(TimeSpan.FromDays(2)), UserEmail = "test@email.com", UserIdentity="hkh" },
                        new Rating { Rate=0.78m, RatedDate=DateTime.Now, UserIdentity = "scott" }
                    }, (e, rh) => { rh.Post = e; e.Ratings.Add(rh); })
                    .CheckInverseList(e => e.Revisions, new List<PostRevision> {
                        new PostRevision { RevisionNumber = 1,  Body= "<div><p>test body 1</p></div>", TagsCommaSeperated = "tag1, test category", Summary ="test summary 1", Format = ContentFormats.Html, RevisedDate = DateTime.Now, ViewsCount = 2, Reviser = user1 },
                        new PostRevision { RevisionNumber = 2,  Body= "<div><p>test body for <span>an awesome test blog</span></p></div>", TagsCommaSeperated = "test tag", Summary ="test summary", Format = ContentFormats.Html, RevisedDate = DateTime.Now, ViewsCount = 13, Reviser = user1  }
                    }, (e, rv) => { rv.Post = e; e.Revisions.Add(rv); })
                    .CheckInverseList(e => e.Comments, new List<Comment> {
                        new Comment { AuthorName ="author1", AuthorEmail="a@email.com", Body ="test comment 1", Posted = DateTime.Now, Status = CommentStatus.NotSpam, RevisionNumber = 1},
                        new Comment { AuthorName ="author2", AuthorEmail="b@email.com", Body ="test comment 2", Posted = DateTime.Now, Status = CommentStatus.Spam, RevisionNumber = 1},
                        new Comment { AuthorName ="author1", AuthorEmail="a@email.com", Body ="test comment 3", Posted = DateTime.Now, Status = CommentStatus.NotSpam, RevisionNumber = 2},
                    }, (e, c) => { c.Post = e; e.Comments.Add(c); })
                    .CheckList(e => e.Tags, new List<Tag> {
                        category1, tag1, tag2
                    }, (e, t) => e.Tags.Add(t))
                    .CheckInverseList(e => e.Pingbacks, new List<Pingback>
                    {
                        new Pingback { TargetUri = "uri1", TargetTitle="pingback title1", IsSpam = false, Received = DateTime.Now.Subtract(TimeSpan.FromHours(1))},
                        new Pingback { TargetUri = "uri2", TargetTitle="pingback title2", IsSpam = true, Received = DateTime.Now}
                    }, (e, p) => { p.Post = e; e.Pingbacks.Add(p); })
                    .VerifyTheMappings();
            }
        }
    }
}
