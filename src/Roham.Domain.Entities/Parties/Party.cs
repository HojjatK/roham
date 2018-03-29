using System.Collections.Generic;
using Roham.Lib.Domain;
using Roham.Domain.Entities.Security;

namespace Roham.Domain.Entities.Parties
{
    /// <summary>
    /// Party domain entity.
    /// </summary>
    public abstract class Party : AggregateRoot
    {
        private ICollection<PartyRole> _partyRoles;
        public virtual ICollection<PartyRole> PartyRoles
        {
            get { return this.LazySet(ref _partyRoles); }
            protected set { _partyRoles = value.AsSet(); }
        }

        private ICollection<Address> _addresses;
        public virtual ICollection<Address> Addresses
        {
            get { return this.LazySet(ref _addresses); }
            protected set { _addresses = value.AsSet(); }
        }

        private ICollection<Telephone> _phones;
        public virtual ICollection<Telephone> Telephones
        {
            get { return this.LazySet(ref _phones); }
            protected set { _phones = value.AsSet(); }
        }

        private ICollection<User> _users;
        public virtual ICollection<User> Users
        {
            get { return this.LazySet(ref _users); }
            protected set { _users = value.AsSet(); }
        }
    }
}