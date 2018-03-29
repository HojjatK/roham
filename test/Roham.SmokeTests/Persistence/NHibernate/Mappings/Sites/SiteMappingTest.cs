using System;
using System.Collections.Generic;
using FluentNHibernate;
using FluentNHibernate.Testing;
using NUnit.Framework;
using Roham.Domain.Entities.Posts;
using Roham.Domain.Entities.Jobs;
using Roham.Domain.Entities.Security;
using Roham.Domain.Entities.Sites;
using Roham.Lib.Strings;

namespace Roham.Persistence.NHibernate.Mappings.Sites
{
    public class SiteMappingTest
    {
        [TestFixture]
        [Category("SmokeTests.NHibernate.Mapping")]
        internal class GivenSiteEntity : NHibernateEntityMappingFixture
        {
            [Test]
            public void TestSiteMapping()
            {
                // arrange
                var portal = GetOrCreatePortal();
                User user1 = GetOrCreateUser("testsitemapping.u1"),
                    user2 = GetOrCreateUser("testsitemapping.u2"),
                    user3 = GetOrCreateUser("testsitemapping.u3");
                var role1 = GetOrCreateAdminRole();
                var zonetype1 = ZoneTypeCodes.Blog;

                var z1 = new Zone { Name = "test.site.mapping.zone1", Title = "test zone 1", ZoneType = zonetype1 };
                var z2 = new Zone { Name = "test.site.mapping.zone2", Title = "test zone 2", ZoneType = zonetype1 };
                var post1 = new Post { Name = "test.site.mapping.post1", Title = "post1", Created = DateTime.Now, Zone = z1, Creator = user2 };
                post1.Revise();
                post1.LatestRevision.Reviser = user2;
                post1.LatestRevision.Body = "test";

                var post2 = new Post { Name = "test.site.mapping.post2", Title = "post2", Created = DateTime.Now, Zone = z2, Creator = user2 };
                post2.Revise();
                post2.LatestRevision.Reviser = user2;
                post2.LatestRevision.Body = "test";

                new PersistenceSpecification<Site>(Session, new CustomEqualityComparer())
                    .CheckProperty(s => s.Uid, Guid.NewGuid())
                    .CheckProperty(s => s.Name, new PageName("Test site mapping Site"))
                    .CheckProperty(s => s.Title, "Test site mapping Site")
                    .CheckProperty(s => s.Description, "Site description")
                    .CheckProperty(s => s.IsDefault, true)
                    .CheckProperty(s => s.IsActive, true)
                    .CheckProperty(s => s.IsPrivate, false)
                    .CheckProperty(s => s.Portal, portal)
                    .CheckProperty(s => s.Owner, user1)
                    .CheckInverseList(s => s.Settings, new List<Setting> {
                        new Setting { Section = "test.site.mapping", Name = "setting1", Title = "setting display name", Value = "setting1 value" } },
                        (si, se) => { se.Site = si; si.Settings.Add(se); })
                    .CheckList(s => s.Users, new List<User> {
                        user1, user2, user3 },
                        (s, u) => { s.Users.Add(u); })
                    .CheckInverseList(s => s.WorkflowRules, new List<PostWorkflowRule> {
                        new PostWorkflowRule { Name = "work.flow.1", IsActive = false, ApproverRole = role1 },
                        new PostWorkflowRule { Name = "work.flow.2", IsActive = true, ApproverRole = role1 } },
                        (s, w) => { w.Site = s; s.WorkflowRules.Add(w); })
                    .CheckInverseList(Reveal.Member<Site, IEnumerable<Zone>>(Site.NameOfZones),
                        new List<Zone> { z1, z2 },
                        (s, z) => { z.Site = s; Reveal.Member<Site, ICollection<Zone>>(Site.NameOfZones).Compile()(s).Add(z); })
                    .CheckInverseList(Reveal.Member<Site, IEnumerable<Tag>>(Site.NameOfTags),
                        new List<Tag> {
                                new Tag { Name = "test.site.mapping.tag1"} ,
                                new Category { Name = "test.site.mapping.cat2", IsPrivate = true }
                        },
                        (s, t) => { t.Site = s; Reveal.Member<Site, ICollection<Tag>>(Site.NameOfTags).Compile()(s).Add(t); })
                    .CheckInverseList(Reveal.Member<Site, IEnumerable<Job>>(Site.NameOfJobs),
                        new List<Job> {
                                new Job { Name = "test.site.mapping.job1", Created = DateTime.Now, Owner = user2},
                                new Job { Name = "test.site.mapping.job2", Created = DateTime.Now, Owner = user2}
                        },
                        (s, j) => { j.Site = s; Reveal.Member<Site, ICollection<Job>>(Site.NameOfJobs).Compile()(s).Add(j); })
                    .CheckInverseList(Reveal.Member<Site, IEnumerable<PostSerie>>(Site.NameOfPostSeries),
                        new List<PostSerie> {
                                new PostSerie { Name = "test.site.mapping.serie1", Title = "test serie1"}
                        },
                        (s, es) => { es.Site = s; Reveal.Member<Site, ICollection<PostSerie>>(Site.NameOfPostSeries).Compile()(s).Add(es); })
                    .CheckInverseList(Reveal.Member<Site, IEnumerable<Post>>(Site.NameOfEntries),
                        new List<Post> { post1, post2 },
                        (s, e) => { e.Site = s; Reveal.Member<Site, ICollection<Post>>(Site.NameOfEntries).Compile()(s).Add(e); })
                    .VerifyTheMappings();
            }
        }
    }
}
