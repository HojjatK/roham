using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using Roham.Lib.Domain;
using Roham.Lib.Domain.DataAnnotation;
using Roham.Domain.Entities.Posts;
using Roham.Domain.Entities.Jobs;
using Roham.Domain.Entities.Parties;
using Roham.Domain.Entities.Sites;

namespace Roham.Domain.Entities.Security
{
    public enum UserStatus
    {
        Active = 1,
        InActive = 2,
        Locked = 3,
    }

    /// <summary>
    /// User domain entity.
    /// </summary>
    public class User : AggregateRoot, IUserNamed
    {
        [Required]
        [MaxLength(Lengths.Email)]
        [Unique("UQ_User_UserName")]
        public virtual string UserName { get; set; }

        [MaxLength(Lengths.Email)]
        [Unique("UQ_User_Email")]
        public virtual string Email { get; set; }

        [Required]
        public virtual bool EmailConfirm { get; set; }

        [MaxLength(Lengths.ShortName)]
        [Required]
        public virtual string PasswordHashAlgorithm { get; set; }

        [MaxLength(Lengths.Description)]
        public virtual string PasswordHash { get; set; }

        [MaxLength(Lengths.Description)]
        public virtual string SecurityStamp { get; set; }

        [MaxLength(Lengths.ShortName)]
        public virtual string PhoneNumber { get; set; }

        [Required]
        public virtual bool PhoneNumberConfirmed { get; set; }

        [Required]
        public virtual bool TwoFactorEnabled { get; set; }

        public virtual DateTime? LockoutEndDateUtc { get; set; }

        [Required]
        public virtual bool LockoutEnabled { get; set; }

        [Required]
        public virtual int AccessFailedCount { get; set; }

        [Required]
        public virtual bool IsSystemUser { get; set; }

        [Required]
        public virtual UserStatus Status { get; set; }

        [MaxLength(Lengths.LongName)]
        public virtual string StatusReason { get; set; }

        public virtual Party Party { get; set; }

        public virtual Person Person
        {
            get
            {
                if (Party != null && Party.Is<Person>())
                {
                    return Party.Unproxy<Person>();
                }
                return null;
            }
        }

        public virtual Organisation Organisation
        {
            get
            {
                if (Party != null && Party.Is<Organisation>())
                {
                    return Party.Unproxy<Organisation>();
                }
                return null;
            }
        }

        public virtual IEnumerable<Site> GetUserSites()
        {
            return UserSites;
        }

        private ICollection<PostPermission> _permissions;
        public virtual ICollection<PostPermission> Permissions
        {
            get { return this.LazySet(ref _permissions); }
            protected set { _permissions = value.AsSet(); }
        }

        private ICollection<UserSession> _userSessions;
        protected virtual ICollection<UserSession> UserSessions
        {
            get { return this.LazySet(ref _userSessions); }
            set { _userSessions = value.AsSet(); }
        }

        private ICollection<UserClaim> _userClaims;
        public virtual ICollection<UserClaim> UserClaims
        {
            get { return this.LazySet(ref _userClaims); }
            protected set { _userClaims = value.AsSet(); }
        }

        private ICollection<UserLogin> _userLogins;
        public virtual ICollection<UserLogin> UserLogins
        {
            get { return this.LazySet(ref _userLogins); }
            protected set { _userLogins = value.AsSet(); }
        }

        private ICollection<Role> _roles;
        public virtual ICollection<Role> Roles
        {
            get { return this.LazySet(ref _roles); }
            protected set { _roles = value.AsSet(); }
        }

        private ICollection<Job> _jobs;
        protected internal virtual ICollection<Job> Jobs
        {
            get { return this.LazySet(ref _jobs); }
            set { _jobs = value.AsSet(); }
        }

        // many-to-many
        private ICollection<Site> _userSites;
        protected internal virtual ICollection<Site> UserSites
        {
            get { return this.LazySet(ref _userSites); }
            set { _userSites = value.AsSet(); }
        }

        private ICollection<PostRevision> _postRevisions;
        protected internal virtual ICollection<PostRevision> PostRevisions
        {
            get { return this.LazySet(ref _postRevisions); }
            set { _postRevisions = value.AsSet(); }
        }

        public static string NameOfUserSessions => nameof(UserSessions);
        public static string NameOfJobs => nameof(Jobs);
        public static string NameOfUserSites => nameof(UserSites);
        public static string NameOfPostRevisions => nameof(PostRevisions);
    }
}