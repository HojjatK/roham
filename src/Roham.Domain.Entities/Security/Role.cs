using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using Roham.Lib.Domain;
using Roham.Lib.Domain.DataAnnotation;

namespace Roham.Domain.Entities.Security
{
    public class SecurityRoleNames
    {
        public const string SysAdmin = "SysAdmin";
        public const string Admin = "Admin";
        public const string User = "User";
        public const string SysAdmin_Admin = "SysAdmin, Admin";
        public const string SysAdmin_Admin_User = "SysAdmin, Admin, User";

        public static string GetSecurityRoleName(RoleTypeCodes typeCode)
        {
            switch (typeCode)
            {
                case RoleTypeCodes.SystemAdmin:
                    return SysAdmin;
                case RoleTypeCodes.Administrator:
                    return Admin;
                case RoleTypeCodes.User:
                    return User;
                default:
                    return null;
            }
        }
    }
   
    public enum RoleTypeCodes
    {
        SystemAdmin = 1,
        Administrator = 2,
        User = 3,
    }

    /// <summary>
    /// Role domain entity.
    /// </summary>
    public class Role : AggregateRoot
    {
        [Required(AllowEmptyStrings = false)]
        [Unique("UQ_Role_Name")]
        [MaxLength(Lengths.Name)]
        public virtual string Name { get; set; }

        [MaxLength(Lengths.Description)]
        [Required(AllowEmptyStrings = false)]
        [Unique("UQ_Role_Description")]
        public virtual string Description { get; set; }

        [Required]
        public virtual bool IsSystemRole { get; set; }

        [Required]
        public virtual RoleTypeCodes RoleType { get; set; }

        public virtual IEnumerable<User> GetUsers()
        {
            return Users;
        }

        public virtual IEnumerable<AppFunction> GetFunctions()
        {
            return AppFunctions;
        }

        // ManyToMany relation is controlled by user entity, so keep Users property as protected
        private ICollection<User> _users;
        protected virtual ICollection<User> Users        
        {
            get { return this.LazySet(ref _users); }
            set { _users = value.AsSet(); }
        }        
                
        private ICollection<AppFunction> _appFunctions;
        public virtual ICollection<AppFunction> AppFunctions
        {
            get { return this.LazySet(ref _appFunctions); }
            set { _appFunctions = value.AsSet(); }
        }

        public static string NameOfUsers => nameof(Users);
    }
}
