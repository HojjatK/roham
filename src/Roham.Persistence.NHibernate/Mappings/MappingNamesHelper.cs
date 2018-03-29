using Roham.Lib.Domain;

namespace Roham.Domain.Entities
{
    public static class MappingNames
    {
        public static string ParentId = "ParentId";
        
        public static string OwnerId = "OwnerId";
        public static string CreatorId = "CreatorId";
        public static string ReviserId = "ReviserId";
        public static string ApproverId = "ApproverId";
        public static string PublisherId = "PublisherId";

        public static string ApproverRoleId = "ApproverRoleId";
        public static string PublisherRoleId = "PublisherRoleId";

        public static string LastRevisionId = "LastRevisionId";
        public static string RevisionNumber = "RevisionNumber";
        public static string RevisionAuthor = "RevisionAuthor";
        public static string LastRevised = "LastRevised";
        public static string LatestRevisionEncoding = "LatestRevisionEncoding";
        public static string LatestRevisionFormat = "LatestRevisionFormat";

        public static string JobTaskTableName = "JobTask";
        public static string JobTaskHistoryTableName = "JobTaskHistory";
        public static string PartyRoleMapTableName = "PartyRoleMap";
        public static string TagPostMapTableName = "TagPostMap";
        public static string AppFunctionRoleMapTableName = "AppFunctionRoleMap";
        public static string UserRoleMapTableName = "UserRoleMap";
        public static string SiteUserMapTableName = "SiteUserMap";

        public static string RefId<TEntity>() where TEntity : Identifiable 
        {
            return $"{typeof(TEntity).Name}Id";
        }
    }
}
