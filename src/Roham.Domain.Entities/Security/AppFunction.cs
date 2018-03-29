using Roham.Lib.Domain;
using Roham.Lib.Domain.DataAnnotation;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace Roham.Domain.Entities.Security
{
    /// <summary>
    /// You can change the enum names, but don't change the digits without updating database (since they are used in database)
    /// </summary>
    public enum FunctionKeys : int
    {
        Dashboard = 1,
        Configs = 2,
                            Portal = 21,
                            PortalSettings = 22,
        Sites = 3,
                            CreateSite = 31,
                            UpdateSite = 32,
                            DeleteSite = 33,
                            SiteSettings = 34,
                            Zones = 35,
                                            CreateZone = 351,
                                            UpdateZone = 352,
                                            DeleteZone = 353,
                           Categories = 36,
                                            CreateCategory = 361,
                                            UpdateCategory = 362,
                                            DeleteCategory = 363,
        Roles = 4,
                            CreateRole = 41,
                            UpdateRole = 42,
                            DeleteRole = 43,
        Users = 5,
                            CreateUser = 51,
                            UpdateUser = 52,
                            DeleteUser = 53,
                            UserPermissions = 54,
                                             UpdateUserPermission = 541,
                                             DeleteUserPermission = 542,        
        Posts = 6,
                            CreatePost = 61,
                            SavePost = 62,
                            RevisePost = 63,
                            PublishPost = 64,
                            DeletePost = 65,
                            PostSeries = 66,
                                            CreatePostSerie = 661,
                                            UpdatePostSerie = 662,
                                            DeletePostSerie = 663,
                            Comments = 67,
                                            DeclareCommentAsSpam = 671,
                                            DeleteComment = 672,
                                            DisableComment = 673,
                            Tags = 68,
                                            ReviseTag = 681,
                                            DeleteTag = 682,                                            
                            Ratings = 69,
                                            ResetRating = 691,
                                            DisableRating = 692,
        Jobs = 7,
                            CreateJob = 71,
                            UpdateJob = 72,
                            DeleteJob = 73,
                            ExecuteJob = 74,
        Reports = 8,
    }

    /// <summary>
    /// Application function domain entity.
    /// </summary>
    public class AppFunction : AggregateRoot
    {
        [Required]
        [Unique("UQ_FunctionKey_Key")]
        public virtual FunctionKeys Key { get; set; }

        [MaxLength(Lengths.Name)]
        [Unique("UQ_FunctionKey_Name")]
        public virtual string Name { get; set; }

        [MaxLength(Lengths.Name)]
        public virtual string Title { get; set; }

        [MaxLength(Lengths.Description)]
        public virtual string Description { get; set; }

        public virtual AppFunction Parent { get; set; }

        public virtual IEnumerable<Role> GetRoles()
        {
            return Roles;
        }

        // ManyToMany relation is controlled by role entity, so keep Roles property as protected
        private ICollection<Role> _roles;
        protected virtual ICollection<Role> Roles
        {
            get { return this.LazySet(ref _roles); }
            set { _roles = value.AsSet(); }
        }

        public static string NameOfRoles => nameof(Roles);
    }
}
