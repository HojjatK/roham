using System;
using System.Collections.Generic;
using System.Linq;
using NUnit.Framework;
using Ploeh.AutoFixture;
using NHibernate.Linq;
using Roham.Lib.Strings;
using Roham.Domain.Entities.Parties;
using Roham.Domain.Entities.Security;
using Roham.Domain.Entities.Sites;
using Roham.Domain.Entities.Posts;
using Roham.Domain.Entities.Jobs;
using Roham.Domain.Entities.Snippets;

namespace Roham.Persistence.NHibernate.Ghostbusters
{
    public class GhostbustersTest : NHibernateFixture
    {
        public GhostbustersTest() : base(false) { }

        [TestFixture]
        [Category("SmokeTests.NHibernate.Ghostbuster")]
        internal class GivenMappedEntities : GhostbustersTest
        {
            [Test]
            public void AssertGhostbusters()
            {
                using (var tx = Session.BeginTransaction())
                {
                    // Within the following scenario all entities are utilised
                    // 1. create an admin user 
                    CreateAdminUser();

                    // 2. Create a new portal and a site with some settings 
                    var site = CreatePortalAndSite();

                    // 2. Create site zones 
                    var zone = CreateZones(site);

                    // 3. User logins
                    CreateUserLogin(site);

                    // 3. Create some posts associated with the zones
                    CreatePosts(site, zone);

                    // 4. Create some code snippet 
                    CreateCodeSnippets();

                    // 5. Create job
                    CreateJobs(site);

                    // 6. Create workflow      
                    CreateWorkflows(site, zone);

                    tx.Commit();
                }

                new Ghostbuster(
                    Configuration,
                    SessionFactory,
                    new Action<string>(msg => Assert.Fail(msg)),
                    new Action<string>(msg => 
                    Assert.Inconclusive(msg)))
                    .Test();
            }

            private void CreateAdminUser()
            {
                var adminUser = GetOrCreateAdminUser();

                var userPartyRole = TestDataBuilder.Build<PartyRole>()
                    .With(e => e.Name, "User")
                    .Create();
                Session.Save(userPartyRole);

                var adminPerson = TestDataBuilder.NewPersonParty("user", "test", "no 3, oak avenue", "33445566", new List<PartyRole> { userPartyRole });
                Session.Save(adminPerson);

                adminUser.Party = adminPerson;
                Session.SaveOrUpdate(adminUser);
                Session.Flush();
            }

            private Site CreatePortalAndSite()
            {
                var portal = GetOrCreatePortal();
                var userRole = GetOrCreateUserRole();
                var adminUser = GetOrCreateAdminUser();

                var user1 = TestDataBuilder.NewUser("user1", "user1@site1.com", userRole, UserStatus.Active);
                var user2 = TestDataBuilder.NewUser("user2", "user2@site1.com", userRole, UserStatus.Active);
                var user3 = TestDataBuilder.NewUser("user3", "user3@site1.com", userRole, UserStatus.Active);

                Session.Save(user1);
                Session.Save(user2);
                Session.Save(user3);

                var site1 = TestDataBuilder.NewSite("site1", portal, adminUser);
                site1.Users.Add(user1);
                site1.Users.Add(user2);
                site1.Users.Add(user3);
                Session.Save(site1);
                Session.Flush();

                var setting1 = TestDataBuilder.Build<Setting>()
                    .With(s => s.Section, new PageName("section1"))
                    .With(s => s.Name, new PageName("setting1"))
                    .With(s => s.Site, null)
                    .Create();
                Session.Save(setting1);

                var setting2 = TestDataBuilder.Build<Setting>()
                    .With(s => s.Section, new PageName("section2"))
                    .With(s => s.Name, new PageName("setting1"))
                    .With(s => s.Site, site1)
                    .Create();
                Session.Save(setting2);

                var redirect = TestDataBuilder.Create<Redirect>();
                Session.Save(redirect);
                Session.Flush();

                // security
                var adminRole = GetOrCreateAdminRole();
                var user = TestDataBuilder.NewUser("user4", "user4@email.com", userRole, UserStatus.Active);
                user.Roles.Add(userRole);
                Session.Save(user);

                var systemFunction1 = TestDataBuilder.Build<AppFunction>().Without(e => e.Parent).Create();                
                adminRole.AppFunctions.Add(systemFunction1);
                userRole.AppFunctions.Add(systemFunction1);                
                Session.Save(systemFunction1);

                Session.Flush();

                return site1;
            }

            private Zone CreateZones(Site site1)
            {
                var zoneType1 = ZoneTypeCodes.Blog;
                var zoneType2 = ZoneTypeCodes.WebContent;

                var zone1 = TestDataBuilder.Build<Zone>()
                    .With(z => z.Site, site1)
                    .With(x => x.ZoneType, zoneType1)
                    .With(z => z.IsActive, true)
                    .With(z => z.IsPrivate, false)
                    .With(z => z.Name, new PageName("zone1"))
                    .With(z => z.ZoneType, zoneType1)
                    .Create();
                Session.Save(zone1);

                var zone2 = TestDataBuilder.Build<Zone>()
                    .With(z => z.Site, site1)
                    .With(x => x.ZoneType, zoneType2)
                    .With(z => z.IsActive, true)
                    .With(z => z.IsPrivate, true)
                    .With(z => z.Name, new PageName("zone2"))
                    .With(z => z.ZoneType, zoneType2)
                    .Create();
                Session.Save(zone2);
                Session.Flush();

                return zone1;
            }

            private void CreateUserLogin(Site site)
            {
                var userRole = GetOrCreateUserRole();

                var user1 = TestDataBuilder.NewUser("demo-user", "demo-user@email.com", userRole, UserStatus.Active);
                Session.Save(user1);

                user1.UserLogins.Add(new UserLogin { LoginProvider = "test-provider", ProviderKey = "12345678", User = user1 });
                user1.UserClaims.Add(new UserClaim { ClaimType = "test-claim", ClaimValue = "1234", User = user1 });
                Session.Save(user1);


                var session = new UserSession { StartTimestamp = DateTime.Now, Status = SessionStatus.Active, User = user1 };
                Session.Save(session);

                session.EndTimestamp = DateTime.Now;
                Session.Save(session);
                
                var logEntry1 = new LogEntry { Timestamp = DateTime.Now, Level = LogEntryLevel.Debug, Message = "Test debug message", LoggerName = "TestLogger", Thread = 323.ToString(), ProcessUser = "NT User", SessionUser = user1.UserName, SessionToken = session.Uid.ToString() };
                var logEntry2 = new LogEntry { Timestamp = DateTime.Now, Level = LogEntryLevel.Error, Message = "Test error message", LoggerName = "TestLogger", Thread = 134.ToString(), ProcessUser = "NT User", SessionUser = user1.UserName, SessionToken = session.Uid.ToString(), StackTrace = Environment.StackTrace, Exception = new ArgumentNullException().ToString()};
                Session.Save(logEntry1);
                Session.Save(logEntry2);

                Session.Flush();
            }

            private void CreatePosts(Site site1, Zone zone1)
            {
                var userRole = GetOrCreateUserRole();

                var postCreator = TestDataBuilder.NewUser("postOwner", "post-owner@email.com", userRole, UserStatus.Active);
                Session.Save(postCreator);

                var post = TestDataBuilder.NewPost(site1, zone1, postCreator, commentsCount: 5, pingbackCounts: 3, ratingsCount: 10);
                post.Creator = postCreator;
                post.LatestRevision.Reviser = postCreator;
                Session.Save(post);
                Session.Flush();

                var revision = post.Revise();
                revision.Reviser = postCreator;
                revision.Body = "modified " + revision.Body;
                Session.SaveOrUpdate(post);

                var comment = post.Comment();
                comment.AuthorName = "commenter";
                comment.Body = "this is a test comment";
                Session.SaveOrUpdate(post);
                Session.Flush();

                var rate = post.Rate();
                rate.Rate = (decimal)4.5;
                rate.UserIdentity = "user.identity123";
                Session.SaveOrUpdate(post);
                Session.Flush();

                var link = TestDataBuilder.Build<PostLink>().Without(c => c.Post).Create();
                link.Post = post;
                post.Links.Add(link);

                var pingback = TestDataBuilder.Build<Pingback>().Without(c => c.Post).Without(x => x.Snippet).Create();
                pingback.Post = post;
                post.Pingbacks.Add(pingback);
                Session.SaveOrUpdate(post);
                Session.Flush();

                var tag = TestDataBuilder.Build<Tag>()
                    .With(t => t.Site, site1)
                    .Create();
                Session.Save(tag);

                var category = TestDataBuilder.Build<Category>()
                    .Without(t => t.Parent)
                    .With(t => t.Site, site1)
                    .Create();
                Session.Save(category);

                post.Tags.Add(tag);
                post.Tags.Add(category);
                Session.SaveOrUpdate(post);
                Session.Flush();

                var serie = TestDataBuilder.Build<PostSerie>()
                    .With(s => s.Site, site1)
                    .Create();
                serie.Posts.Add(post);
                Session.Save(serie);
                Session.Flush();

                var postPermission = TestDataBuilder.Build<PostPermission>()
                    .With(x => x.User, postCreator)
                    .With(x => x.Post, post)
                    .Create();
                Session.Save(postPermission);
                Session.Flush();
            }

            private void CreateCodeSnippets()
            {
                var userRole = GetOrCreateUserRole();

                var snippetOwner = TestDataBuilder.NewUser("snippetOwner", "snippet-owner@email.com", userRole, UserStatus.Active);
                Session.Save(snippetOwner);

                var snippet = TestDataBuilder.NewSnippet(snippetOwner, commentsCount: 3, ratingsCount: 2, pingbackCounts: 100);
                snippet.Creator = snippetOwner;
                snippet.LatestRevision.Reviser = snippetOwner;

                var link = TestDataBuilder.Build<SnippetLink>().Without(c => c.Snippet).Create();
                link.Snippet = snippet;
                snippet.Links.Add(link);

                Session.Save(snippet);
                Session.Flush();
            }

            private void CreateJobs(Site site1)
            {
                var adminRole = GetOrCreateAdminRole();

                var jobOwner = TestDataBuilder.NewUser("jobCreator", "job-creator@email.com", adminRole, UserStatus.Active);
                Session.Save(jobOwner);

                var job1 = TestDataBuilder.Build<Job>()
                    .With(j => j.Owner, jobOwner)
                    .With(j => j.Site, site1)
                    .Create();
                var task1 = TestDataBuilder.Build<JobTask>().With(jd => jd.Job, job1).Create();
                var task1Detail = TestDataBuilder.Build<JobTaskDetail>().With(jd => jd.OwnerTask, task1).Create();                

                task1.Details.Add(task1Detail);

                job1.Tasks.Add(task1);

                job1.Tasks.Add(TestDataBuilder.Build<JobTask>().Without(jd => jd.Job).Create());
                job1.Tasks.Add(TestDataBuilder.Build<JobTask>().Without(jd => jd.Job).Create());
                job1.Tasks.ForEach(jd => jd.Job = job1);

                Session.Save(job1);

                var job2 = TestDataBuilder.Build<Job>()
                    .With(j => j.Owner, jobOwner)
                    .With(j => j.Site, site1)
                    .Create();
                job2.Tasks.Add(TestDataBuilder.Build<JobTask>().Without(jh => jh.Job).Create());
                job2.Tasks.ForEach(jd => jd.Job = job2);

                Session.Save(job2);
            }

            private void CreateWorkflows(Site site1, Zone zone1)
            {
                var adminRole = GetOrCreateAdminRole();
                var userRole = GetOrCreateUserRole();

                var workflow = TestDataBuilder.Build<PostWorkflowRule>()
                    .With(w => w.Site, site1)
                    .With(w => w.ReturnToAuthorForPublish, false)
                    .With(w => w.IsActive, true)
                    .With(w => w.ApproverRole, adminRole)
                    .With(w => w.PublisherRole, userRole)
                    .Create();
                Session.Save(workflow);

                var postCreator2 = TestDataBuilder.NewUser("postOwner2", "post-owner2@email.com", userRole, UserStatus.Active);
                Session.Save(postCreator2);

                var postApprover = TestDataBuilder.NewUser("postApprover2", "post-approver2@email.com", adminRole, UserStatus.Active);
                Session.Save(postApprover);

                var postPublisher = TestDataBuilder.NewUser("postPublisher2", "post-publisher2@email.com", userRole, UserStatus.Active);
                Session.Save(postPublisher);

                var post = TestDataBuilder.NewPost(site1, zone1, postCreator2, commentsCount: 5, pingbackCounts: 3, ratingsCount: 10);                
                post.LatestRevision.Reviser = postCreator2;
                post.Status = PostStatus.Saved;
                Session.Save(post);

                
                post.LatestRevision.Approver = postApprover;
                post.LatestRevision.ApprovedDate = DateTime.Now;
                post.LatestRevision.ApproverRoleName = adminRole.Name;
                post.Status = PostStatus.Approved;
                Session.SaveOrUpdate(post);

                post.LatestRevision.Publisher = postPublisher;
                post.LatestRevision.PublishedDate = DateTime.Now;
                post.LatestRevision.PublisherRoleName = userRole.Name;
                post.Status = PostStatus.Published;
                Session.SaveOrUpdate(post);
            }

            private Role GetOrCreateAdminRole()
            {
                Role adminRole = Session
                    .Query<Role>()
                    .Where(r => r.RoleType == RoleTypeCodes.SystemAdmin && r.IsSystemRole)
                    .FirstOrDefault();

                if (adminRole == null)
                {
                    adminRole = TestDataBuilder.Build<Role>()
                    .Without(e => e.AppFunctions)
                    .With(e => e.IsSystemRole, true)
                    .With(e => e.RoleType, RoleTypeCodes.SystemAdmin)
                    .With(e => e.Name, "administrator")                    
                    .Create();
                    Session.Save(adminRole);
                    Session.Flush();
                }
                return adminRole;
            }

            private Role GetOrCreateUserRole()
            {
                Role userRole = Session
                    .Query<Role>()
                    .Where(r => r.RoleType != RoleTypeCodes.SystemAdmin)
                    .FirstOrDefault();

                if (userRole == null)
                {
                    userRole = TestDataBuilder.Build<Role>()
                        .Without(e => e.AppFunctions)
                        .With(e => e.RoleType, RoleTypeCodes.User)
                        .With(e => e.Name, "user")
                        .Create();

                    Session.Save(userRole);
                    Session.Flush();
                }
                return userRole;
            }

            private User GetOrCreateAdminUser()
            {
                var adminRole = GetOrCreateAdminRole();
                User adminUser = Session
                    .Query<User>()
                    .Where(u => u.Roles.Any(r => r.RoleType == RoleTypeCodes.SystemAdmin && r.IsSystemRole))
                    .FirstOrDefault();
                if (adminUser == null)
                {
                    adminUser = TestDataBuilder.NewUser("portal-admin", "portal-admin@mail.com", adminRole, UserStatus.Active);
                    Session.Save(adminUser);
                    Session.Flush();
                }
                return adminUser;
            }

            private Portal GetOrCreatePortal()
            {
                Portal portal = Session
                    .Query<Portal>()
                    .SingleOrDefault();

                if (portal == null)
                {
                    var adminUser = GetOrCreateAdminUser();

                    var orgPartyRole = TestDataBuilder.Build<PartyRole>()
                        .With(e => e.Name, "Portal Orgnisation")
                        .Create();
                    Session.Save(orgPartyRole);

                    var portalOrg = TestDataBuilder.NewOrganisationParty("my portal", "no 3, oak avenue", "484848", new List<PartyRole> { orgPartyRole });
                    Session.Save(portalOrg);

                    portal = TestDataBuilder.NewPortal(adminUser);
                    portal.Organisation = portalOrg;
                    Session.Save(portal);

                    Session.Flush();
                }
                return portal;
            }
        }
    }
}
