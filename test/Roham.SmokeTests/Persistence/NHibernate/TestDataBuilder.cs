using System;
using System.Collections.Generic;
using Ploeh.AutoFixture;
using Ploeh.AutoFixture.Dsl;
using Roham.Domain.Entities.Posts;
using Roham.Domain.Entities.Parties;
using Roham.Domain.Entities.Security;
using Roham.Domain.Entities.Sites;
using Roham.Lib.Strings;
using Roham.Domain.Entities.Snippets;

namespace Roham.Persistence.NHibernate
{
    public static class TestDataBuilder
    {
        public static ICustomizationComposer<T> Build<T>()
        {
            var fixture = new Fixture();
            return fixture.Build<T>();
        }

        public static T Create<T>()
        {
            var fixture = new Fixture();
            return fixture.Build<T>().Create();
        }

        public static Portal NewPortal(User portalOwner)
        {
            var fixture = new Fixture();

            var portal = fixture.Build<Portal>()
                .With(p => p.Owner, portalOwner)
                .With(p => p.Name, new PageName("TestPortal"))
                .Without(p => p.Organisation)
                .Create();
            return portal;
        }

        public static Site NewSite(string siteName, Portal portal, User siteOwner)
        {
            var fixture = new Fixture();

            var site = fixture.Build<Site>()
                .With(e => e.Name, new PageName(siteName))
                .With(e => e.Owner, siteOwner)
                .With(e => e.Portal, portal)
                .Create();
            return site;
        }

        public static User NewUser(string userName, string email, Role role, UserStatus userStatus)
        {
            var fixture = new Fixture();

            UserLogin userLogin = null;
            var user = fixture.Build<User>()
                .Without(e => e.Party)
                .With(e => e.UserName, userName)
                .With(e => e.Email, email)
                .With(e => e.Status, userStatus)
                .Do(e => {
                    userLogin = fixture.Build<UserLogin>().Without(u => u.User).Create();                    
                    e.UserLogins.Add(userLogin);
                    e.Roles.Add(role);
                })
                .Create();
            userLogin.User = user;         
            return user;
        }

        public static Person NewPersonParty(string givenName, string surName, string address, string phone, IEnumerable<PartyRole> partyRoles)
        {
            var fixture = new Fixture();

            Address address1 = null;
            Telephone tel1 = null;
            var user = fixture.Build<Person>()
                .With(e => e.Title, "")
                .With(e => e.GivenName, givenName)
                .With(e => e.Surname, surName)
                .Do(e => {
                    address1 = fixture.Build<Address>()
                        .With(a => a.AddressType, AddressTypes.Residential)
                        .With(a => a.AddressLine1, address)
                        .Without(a => a.Party).Create();

                    tel1 = fixture.Build<Telephone>()
                        .With(a => a.Type, TelephoneTypes.Home)
                        .With(a => a.Number, phone)
                        .Without(a => a.Party).Create();

                    e.Addresses.Add(address1);
                    e.Telephones.Add(tel1);
                    foreach (var r in partyRoles)
                        e.PartyRoles.Add(r);
                })
                .Create();
            address1.Party = user;
            tel1.Party = user;

            return user;
        }

        public static Organisation NewOrganisationParty(string name, string address, string phone, IEnumerable<PartyRole> partyRoles)
        {
            var fixture = new Fixture();

            Address address1 = null;
            Telephone tel1 = null;
            var org = fixture.Build<Organisation>()
                .With(e => e.Name, name)
                .Do(e => {
                    address1 = fixture.Build<Address>()
                        .With(a => a.AddressType, AddressTypes.Residential)
                        .With(a => a.AddressLine1, address)
                        .Without(a => a.Party).Create();

                    tel1 = fixture.Build<Telephone>()
                        .With(a => a.Type, TelephoneTypes.Home)
                        .With(a => a.Number, phone)
                        .Without(a => a.Party).Create();

                    e.Addresses.Add(address1);
                    e.Telephones.Add(tel1);
                    foreach (var r in partyRoles)
                        e.PartyRoles.Add(r);
                })
                .Create();
            address1.Party = org;
            tel1.Party = org;
            return org;
        }

        public static Post NewPost(Site site, Zone postZone, User creator, int commentsCount = 0, int pingbackCounts = 0, int ratingsCount = 0)
        {
            var fixture = new Fixture();
            var post = fixture.Build<Post>()
                .With(e => e.Site, site)
                .With(e => e.Name, new PageName("test post " + Guid.NewGuid()))
                .With(e => e.Zone, postZone)                
                .With(e => e.Creator, creator)                
                .Without(e => e.Serie)
                .Do(e => {
                    var r = e.Revise();
                    r.Body = "<div>test post body</div>";
                    r.Author = "revision author";
                    r.Reviser = creator;
                    r.Format = ContentFormats.Html;                    
                })
                .Do(e => {
                    for (int i = 0; i < commentsCount; i++)
                        e.Comments.Add(fixture.Build<Comment>().Without(x => x.Post).Create());
                })
                .Do(e => {
                    for (int i = 0; i < pingbackCounts; i++)
                        e.Pingbacks.Add(fixture.Build<Pingback>().Without(x => x.Post).Without(x => x.Snippet).Create());
                })
                .Do(e => {
                    for (int i = 0; i < ratingsCount; i++)
                        e.Ratings.Add(fixture.Build<Rating>().Without(x => x.Post).Create());
                })
                .Create();
            post.Comments.ForEach(c => c.Post = post);
            post.Pingbacks.ForEach(p => p.Post = post);
            post.Ratings.ForEach(r => r.Post = post);
            return post;
        }

        public static Snippet NewSnippet(User creator, int commentsCount = 0, int ratingsCount = 0, int pingbackCounts = 0)
        {
            var fixture = new Fixture();
            var codeSnippet = fixture.Build<Snippet>()                
                .With(e => e.Name, new PageName("test snippet " + Guid.NewGuid()))                
                .With(e => e.Creator, creator)
                .Do(e => {
                    var r = e.Revise();
                    r.Body = "using System;\r\nfor(int i = 0; i < 10; i++) {\r\n   Console.WriteLine(\"Demo {0}\", i);\r\n }";
                    r.Author = "revision author";
                    r.Reviser = creator;
                    r.BodyEncoding = "UTF8";                    
                })
                .Do(e => {
                    for (int i = 0; i < commentsCount; i++)
                        e.Comments.Add(fixture.Build<SnippetComment>().Without(x => x.Snippet).Create());
                })
                .Do(e => {
                    for (int i = 0; i < pingbackCounts; i++)
                        e.Pingbacks.Add(fixture.Build<Pingback>().Without(x => x.Post).Without(x => x.Snippet).Create());
                })
                .Do(e => {
                    for (int i = 0; i < ratingsCount; i++)
                        e.Ratings.Add(fixture.Build<SnippetRating>().Without(x => x.Snippet).Create());
                })
                .Create();
            codeSnippet.Comments.ForEach(c => c.Snippet = codeSnippet);
            codeSnippet.Pingbacks.ForEach(p => p.Snippet = codeSnippet);
            codeSnippet.Ratings.ForEach(r => r.Snippet = codeSnippet);
            return codeSnippet;
        }
    }
}