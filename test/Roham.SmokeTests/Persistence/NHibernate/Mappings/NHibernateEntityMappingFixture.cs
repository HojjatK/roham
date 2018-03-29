using System.Linq;
using NHibernate.Linq;
using Ploeh.AutoFixture;
using Roham.Domain.Entities.Posts;
using Roham.Domain.Entities.Parties;
using Roham.Domain.Entities.Security;
using Roham.Domain.Entities.Sites;
using Roham.Lib.Strings;

namespace Roham.Persistence.NHibernate.Mappings {
    internal abstract class NHibernateEntityMappingFixture : NHibernateFixture {
        protected NHibernateEntityMappingFixture()
            : base(true) {
        }

        protected NHibernateEntityMappingFixture(bool canDbDirty)
            : base(canDbDirty) {
        }

        protected Portal GetOrCreatePortal() {
            var portal = Session.Query<Portal>()
                .SingleOrDefault();
            if (portal == null) {
                portal = TestDataBuilder.NewPortal(GetOrCreateUser("admin"));
                Session.Save(portal);
                Session.Flush();
            }
            return portal;
        }

        protected Role GetOrCreateAdminRole() {
            var role = Session.Query<Role>()
                .Where(r => r.RoleType == RoleTypeCodes.SystemAdmin && r.Name == "Administrator")
                .FirstOrDefault();
            if (role == null) {
                role = TestDataBuilder.Build<Role>()
                    .Without(r => r.AppFunctions)
                    .With(r => r.Name, "Administrator")
                    .With(r => r.IsSystemRole, true)
                    .With(r => r.RoleType, RoleTypeCodes.SystemAdmin)
                    .Create();
                Session.Save(role);
                Session.Flush();
            }
            return role;
        }

        protected User GetOrCreateUser(string userName) {
            var user = Session.Query<User>()
                .Where(u => u.UserName == userName)
                .SingleOrDefault();
            if (user == null) {
                user = TestDataBuilder.NewUser(userName, userName + "@email.com", GetOrCreateAdminRole(), UserStatus.Active);
                Session.Save(user);
                Session.Flush();
            }
            return user;
        }

        protected Person GetOrCreatePerson(string firstName, string lastName, string address, string phoneNumber) {
            var person = Session.Query<Person>()
                .Where(e => e.GivenName == firstName && e.Surname == lastName)
                .FirstOrDefault();

            if (person == null) {
                person = TestDataBuilder.Build<Person>()
                    .With(p => p.GivenName, firstName)
                    .With(p => p.Surname, lastName)
                    .Do(p => {
                        var addr1 = TestDataBuilder.Build<Address>()
                            .With(a => a.AddressLine1, address)
                            .With(a => a.AddressType, AddressTypes.Residential)
                            .Without(a => a.Party)
                            .Create();
                        p.Addresses.Add(addr1);
                    })
                    .Do(p => {
                        p.Telephones.Add(new Telephone { Area = "02", Number = phoneNumber, Type = TelephoneTypes.Home });
                    })
                    .Create();
                
                foreach(var adr in person.Addresses)
                {
                    adr.Party = person;
                }

                foreach (var tel in person.Telephones)
                {
                    tel.Party = person;
                }

                Session.Save(person);
                Session.Flush();
            }

            return person;
        }

        protected Organisation GetOrCreateOrganisation(string name, string address, string phoneNumber) {
            var person = Session.Query<Organisation>()
                .Where(e => e.Name == name)
                .FirstOrDefault();

            if (person == null) {
                person = TestDataBuilder.Build<Organisation>()
                    .With(p => p.Name, name)
                    .Do(p => {
                        var addr1 = TestDataBuilder.Build<Address>()
                            .With(a => a.AddressLine1, address)
                            .With(a => a.AddressType, AddressTypes.Residential)
                            .Without(a => a.Party)
                            .Create();
                        p.Addresses.Add(addr1);
                    })
                    .Do(p => {
                        p.Telephones.Add(new Telephone { Area = "02", Number = phoneNumber, Type = TelephoneTypes.Business });
                    })
                    .Create();
            }

            return person;
        }

        protected Site GetOrCreateSite(string siteName) {
            var site = Session.Query<Site>()
                .Where(s => s.Name == siteName)
                .FirstOrDefault();
            if (site == null) {
                var portal = GetOrCreatePortal();

                site = TestDataBuilder.NewSite(siteName, portal, GetOrCreateUser(siteName + "admin"));
                Session.Save(site);
                Session.Flush();
            }
            return site;
        }

        //protected ZoneType GetOrCreateZoneType() {
        //    var zoneType = Session.Query<ZoneType>()
        //        .Where(z => z.Code == ZoneTypeCodes.Blog)
        //        .SingleOrDefault();
        //    if (zoneType == null) {
        //        zoneType = TestDataBuilder.Build<ZoneType>()
        //            .With(z => z.Code, ZoneTypeCodes.Blog)
        //            .With(z => z.Name, "Blog")
        //            .Create();
        //        Session.Save(zoneType);
        //        Session.Flush();
        //    }
        //    return zoneType;
        //}

        protected Zone GetOrCreateZone(Site site, string zoneName) {
            var zone = Session.Query<Zone>()
                .Where(z => z.ZoneType == ZoneTypeCodes.Blog &&
                    z.Site.Id == site.Id &&
                    z.Name == zoneName)
                .FirstOrDefault();
            if (zone == null) {
                zone = TestDataBuilder.Build<Zone>()
                    .With(z => z.Site, site)
                    .With(z => z.Name, new PageName(zoneName))
                    .With(z => z.ZoneType, ZoneTypeCodes.Blog)
                    .Create();
                Session.Save(zone);
                Session.Flush();
            }
            return zone;
        }

        protected Category GetOrCreateCategory(Site site, string name, Category parent = null) {
            var cat = Session.Query<Category>()
                .Where(c => c.Name == name)
                .SingleOrDefault();
            if (cat == null) {
                cat = TestDataBuilder.Build<Category>()
                    .With(c => c.Site, site)
                    .With(c => c.Parent, parent)
                    .With(c => c.IsPrivate, true)
                    .With(c => c.Parent, null)
                    .With(c => c.Name, name)
                    .Create();
                Session.Save(cat);
                Session.Flush();
            }
            return cat;
        }

        protected Tag GetOrCreateTag(Site site, string name) {
            var tag = Session.Query<Tag>()
                .Where(c => c.Name == name)
                .SingleOrDefault();
            if (tag == null) {
                tag = TestDataBuilder.Build<Tag>()
                    .With(c => c.Site, site)
                    .With(c => c.Name, name)
                    .Create();
                Session.Save(tag);
                Session.Flush();
            }
            return tag;
        }

        protected PostSerie GetOrCreateSerie(Site site, string name) {
            var serie = Session.Query<PostSerie>()
                .Where(s => s.Name == name && s.Site.Id == site.Id)
                .SingleOrDefault();
            if (serie == null) {
                serie = TestDataBuilder.Build<PostSerie>()
                    .With(s => s.Site, site)
                    .With(s => s.Name, new PageName(name))
                    .With(s => s.IsPrivate, false)
                    .Create();
                Session.Save(serie);
                Session.Flush();
            }
            return serie;
        }
    }
}
