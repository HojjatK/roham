using FluentNHibernate.Testing;
using NUnit.Framework;
using Roham.Domain.Entities.Entries;
using Roham.Domain.Entities.Snippets;
using Roham.Lib.Strings;
using System;
using System.Collections.Generic;

namespace Roham.Persistence.NHibernate.Mappings.Snippets
{
    public class SnippetMappingTest
    {
        [TestFixture]
        [Category("SmokeTests.NHibernate.Mapping")]
        internal class GivenSnippetEntity : NHibernateEntityMappingFixture
        {
            [Test]
            public void TestSnippetMapping()
            {
                // arrange
                var user1 = GetOrCreateUser("test.moderator1");

                // assert
                new PersistenceSpecification<Snippet>(Session, new CustomEqualityComparer())
                    .CheckProperty(e => e.Name, new PageName("An awesome test code snippet"))
                    .CheckProperty(e => e.Title, "An awesome test code snippet")                    
                    .CheckProperty(e => e.Author, "test author")
                    .CheckProperty(e => e.CommentsCount, 3)
                    .CheckProperty(e => e.Rating, 0.5m)
                    .CheckProperty(e => e.DisableDiscussionDays, 20)
                    .CheckProperty(e => e.IsDiscussionEnabled, true)
                    .CheckProperty(e => e.IsRatingEnabled, true)
                    .CheckProperty(e => e.IsAnonymousCommentAllowed, false)
                    .CheckProperty(e => e.Created, DateTime.Now)                                                                                
                    .CheckProperty(e => e.IsPrivate, false)                    
                    .CheckProperty(e => e.IsContentBinary, false)                    
                    .CheckProperty(e => e.LatestRevision, new SnippetRevision
                    {
                        RevisionNumber = 2,
                        Body = "for(int i = 1; i < 10; i++){\r\nConsole.WriteLine(\"Test\");}\r\n",
                        Summary = "test summary",                         
                        RevisedDate = DateTime.Now,                        
                        Reviser = user1
                    })                    
                    .CheckReference(e => e.Creator, user1)                    
                    .CheckInverseList(e => e.Ratings, new List<SnippetRating> {
                        new SnippetRating { Rate=0.56m, RatedDate=DateTime.Now.Subtract(TimeSpan.FromDays(2)), UserEmail = "test@email.com", UserIdentity="hkh" },
                        new SnippetRating { Rate=0.78m, RatedDate=DateTime.Now, UserIdentity = "scott" }
                    }, (e, rh) => { rh.Snippet = e; e.Ratings.Add(rh); })
                    .CheckInverseList(e => e.Revisions, new List<SnippetRevision> {
                        new SnippetRevision { RevisionNumber = 1,  Body= "for(int i = 1; i < 10; i++)\r\nConsole.WriteLine(\"Test\");\r\n", Summary ="test summary 1", RevisedDate = DateTime.Now, Reviser = user1 },
                        new SnippetRevision { RevisionNumber = 2,  Body= "for(int i = 1; i < 10; i++){\r\nConsole.WriteLine(\"Test\");}\r\n}", Summary ="test summary", RevisedDate = DateTime.Now, Reviser = user1  }
                    }, (e, rv) => { rv.Snippet = e; e.Revisions.Add(rv); })
                    .CheckInverseList(e => e.Comments, new List<SnippetComment> {
                        new SnippetComment { AuthorName ="author1", AuthorEmail="a@email.com", Body ="test comment 1", Posted = DateTime.Now, Status = CommentStatus.NotSpam, RevisionNumber = 1},
                        new SnippetComment { AuthorName ="author2", AuthorEmail="b@email.com", Body ="test comment 2", Posted = DateTime.Now, Status = CommentStatus.Spam, RevisionNumber = 1},
                        new SnippetComment { AuthorName ="author1", AuthorEmail="a@email.com", Body ="test comment 3", Posted = DateTime.Now, Status = CommentStatus.NotSpam, RevisionNumber = 2},
                    }, (e, c) => { c.Snippet = e; e.Comments.Add(c); })
                    .VerifyTheMappings();
            }
        }
    }
}
