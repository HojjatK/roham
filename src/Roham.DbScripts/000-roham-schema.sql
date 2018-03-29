create table [Job] (
	Id BIGINT IDENTITY NOT NULL,
   [Uid] UNIQUEIDENTIFIER not null,
   [Name] nvarchar(64) not null,
   [Type] INT not null,
   [IsSystemJob] BIT null,
   [Description] nvarchar(255) null,
   [CronPattern] nvarchar(1024) null,
   [Created] DATETIME not null,
   OwnerId BIGINT not null,
   SiteId BIGINT null,
   primary key (Id),
  unique ([Uid])
)
create table [Address] (
	Id BIGINT IDENTITY NOT NULL,
   [AddressType] INT not null,
   [AddressLine1] nvarchar(64) not null,
   [AddressLine2] nvarchar(64) null,
   [AddressLine3] nvarchar(64) null,
   [PostCode] nvarchar(32) null,
   [Suburb] nvarchar(64) null,
   [State] nvarchar(64) null,
   [Country] nvarchar(64) not null,
   PartyId BIGINT not null,
   primary key (Id)
)
create table [Party] (
	Id BIGINT IDENTITY NOT NULL,
   [Uid] UNIQUEIDENTIFIER not null,
   primary key (Id),
  unique ([Uid])
)
create table PartyRoleMap (
	PartyId BIGINT not null,
   PartyRoleId BIGINT not null,
   primary key (PartyRoleId, PartyId)
)
create table [Organisation] (
	Id BIGINT not null,
   [Name] nvarchar(64) not null,
   primary key (Id),
  unique ([Name])
)
create table [Person] (
	Id BIGINT not null,
   [Title] nvarchar(10) null,
   [GivenName] nvarchar(64) not null,
   [MiddleName] nvarchar(64) null,
   [Surname] nvarchar(64) not null,
   primary key (Id)
)
create table [PartyRole] (
	Id BIGINT IDENTITY NOT NULL,
   [Uid] UNIQUEIDENTIFIER not null,
   [Name] nvarchar(64) not null,
   [Description] nvarchar(255) not null,
   primary key (Id),
  unique ([Uid]),
  unique ([Name])
)
create table [Telephone] (
	Id BIGINT IDENTITY NOT NULL,
   [Type] INT not null,
   [Area] nvarchar(10) null,
   [Number] nvarchar(20) not null,
   PartyId BIGINT not null,
   primary key (Id)
)
create table [Comment] (
	Id BIGINT IDENTITY NOT NULL,
   [Uid] UNIQUEIDENTIFIER not null,
   [AuthorName] nvarchar(64) not null,
   [AuthorUrl] nvarchar(1024) null,
   [AuthorEmail] nvarchar(100) null,
   [AuthorIp] nvarchar(32) null,
   [Body] ntext not null,
   [Posted] DATETIME not null,
   [Status] INT not null,
   [RevisionNumber] INT not null,
   PostId BIGINT not null,
   primary key (Id),
  unique ([Uid])
)
create table [Post] (
	Id BIGINT IDENTITY NOT NULL,
   [Uid] UNIQUEIDENTIFIER not null,
   [Name] nvarchar(64) not null,
   [Title] nvarchar(64) not null,
   [MetaTitle] nvarchar(64) null,
   [MetaDescription] nvarchar(1024) null,
   [MimeType] nvarchar(150) null,
   [Format] INT not null,
   [Author] nvarchar(64) null,
   [CommentsCount] INT not null,
   [ViewsCount] BIGINT not null,
   [PageTemplate] nvarchar(64) null,
   [Status] INT not null,
   [Created] DATETIME not null,
   [PublishDate] DATETIME null,
   [EffectiveDate] DATETIME null,
   [DisableDiscussionDays] INT null,
   [Rating] DECIMAL(19,5) not null,
   [Popularity] DECIMAL(19,5) not null,
   [IsDiscussionEnabled] BIT not null,
   [IsRatingEnabled] BIT not null,
   [IsPrivate] BIT not null,
   [IsChromeHidden] BIT not null,
   [IsContentBinary] BIT not null,
   [IsPingbackEnabled] BIT not null,
   [IsTrackbackEnabled] BIT not null,
   [IsAnonymousCommentAllowed] BIT not null,
   ZoneId BIGINT not null,
   PostSerieId BIGINT null,
   CreatorId BIGINT not null,
   SiteId BIGINT not null,
   LastRevisionId BIGINT null,
   RevisionNumber INT not null,
   RevisionAuthor nvarchar(64) null,
   [Body] ntext not null,
   [BodyImage] VARBINARY(MAX) null,
   LastRevised DATETIME not null,
   LatestRevisionEncoding nvarchar(64) null,
   LatestRevisionFormat INT not null,
   primary key (Id),
  unique ([Uid]),
  unique ([Name], SiteId)
)
create table TagPostMap (
	PostId BIGINT not null,
   TagId BIGINT not null,
   primary key (TagId, PostId)
)
create table [PostRevision] (
	Id BIGINT IDENTITY NOT NULL,
   [RevisionNumber] INT not null,
   [Summary] ntext null,
   [Author] nvarchar(64) null,
   [TagsCommaSeperated] nvarchar(1024) null,
   [RevisedDate] DATETIME not null,
   [ReviseReason] nvarchar(150) null,
   [PublishedDate] DATETIME null,
   [PublisherRoleName] nvarchar(64) null,
   [ApprovedDate] DATETIME null,
   [ApproverRoleName] nvarchar(64) null,
   [BodyEncoding] nvarchar(64) null,
   [Format] INT not null,
   [ViewsCount] BIGINT null,
   [Body] ntext not null,
   [BodyImage] VARBINARY(MAX) null,
   PostId BIGINT not null,
   ReviserId BIGINT not null,
   ApproverId BIGINT null,
   PublisherId BIGINT null,
   OwnerId BIGINT null,
   primary key (Id)
)
create table [PostSerie] (
	Id BIGINT IDENTITY NOT NULL,
   [Uid] UNIQUEIDENTIFIER not null,
   [Name] nvarchar(64) not null,
   [Title] nvarchar(64) not null,
   [Description] nvarchar(255) null,
   [IsPrivate] BIT not null,
   SiteId BIGINT null,
   primary key (Id),
  unique ([Uid]),
  unique ([Name], SiteId)
)
create table [Rating] (
	Id BIGINT IDENTITY NOT NULL,
   [Rate] DECIMAL(19,5) not null,
   [RatedDate] DATETIME not null,
   [UserIdentity] nvarchar(100) null,
   [UserEmail] nvarchar(100) null,
   PostId BIGINT not null,
   primary key (Id)
)
create table [Tag] (
	Id BIGINT IDENTITY NOT NULL,
   [Uid] UNIQUEIDENTIFIER not null,
   [Name] nvarchar(64) not null,
   SiteId BIGINT not null,
   primary key (Id),
  unique ([Uid]),
  unique ([Name], SiteId)
)
create table [Category] (
	Id BIGINT not null,
   [IsPrivate] BIT not null,
   [Description] nvarchar(255) null,
   ParentId BIGINT null,
   primary key (Id)
)
create table [AppFunction] (
	Id BIGINT IDENTITY NOT NULL,
   [Uid] UNIQUEIDENTIFIER not null,
   [Key] INT not null,
   [Name] nvarchar(64) null,
   [Title] nvarchar(64) null,
   [Description] nvarchar(255) null,
   ParentId BIGINT null,
   primary key (Id),
  unique ([Uid]),
  unique ([Key]),
  unique ([Name])
)
create table AppFunctionRoleMap (
	AppFunctionId BIGINT not null,
   RoleId BIGINT not null,
   primary key (RoleId, AppFunctionId)
)
create table [PostPermission] (
	Id BIGINT IDENTITY NOT NULL,
   [Read] BIT not null,
   [Create] BIT not null,
   [Update] BIT not null,
   [Delete] BIT not null,
   [Execute] BIT not null,
   UserId BIGINT not null,
   PostId BIGINT not null,
   primary key (Id)
)
create table [PostWorkflowRule] (
	Id BIGINT IDENTITY NOT NULL,
   [Uid] UNIQUEIDENTIFIER not null,
   [Name] nvarchar(64) null,
   [IsActive] BIT not null,
   [ReturnToAuthorForPublish] BIT not null,
   ApproverRoleId BIGINT not null,
   PublisherRoleId BIGINT null,
   SiteId BIGINT not null,
   primary key (Id),
  unique ([Uid]),
  unique ([Name], SiteId)
)
create table [Role] (
	Id BIGINT IDENTITY NOT NULL,
   [Uid] UNIQUEIDENTIFIER not null,
   [Name] nvarchar(64) not null,
   [Description] nvarchar(255) not null,
   [IsSystemRole] BIT not null,
   [RoleType] INT not null,
   primary key (Id),
  unique ([Uid]),
  unique ([Name]),
  unique ([Description])
)
create table UserRoleMap (
	RoleId BIGINT not null,
   UserId BIGINT not null,
   primary key (UserId, RoleId)
)
create table [UserClaim] (
	Id BIGINT IDENTITY NOT NULL,
   [ClaimType] nvarchar(64) not null,
   [ClaimValue] nvarchar(1024) not null,
   UserId BIGINT not null,
   primary key (Id)
)
create table [UserLogin] (
	Id BIGINT IDENTITY NOT NULL,
   [LoginProvider] nvarchar(150) not null,
   [ProviderKey] nvarchar(255) not null,
   UserId BIGINT not null,
   primary key (Id),
  unique ([LoginProvider], [ProviderKey], UserId)
)
create table [User] (
	Id BIGINT IDENTITY NOT NULL,
   [Uid] UNIQUEIDENTIFIER not null,
   [UserName] nvarchar(100) not null,
   [Email] nvarchar(100) null,
   [EmailConfirm] BIT not null,
   [PasswordHashAlgorithm] nvarchar(32) not null,
   [PasswordHash] nvarchar(255) null,
   [SecurityStamp] nvarchar(255) null,
   [PhoneNumber] nvarchar(32) null,
   [PhoneNumberConfirmed] BIT not null,
   [TwoFactorEnabled] BIT not null,
   [LockoutEndDateUtc] DATETIME null,
   [LockoutEnabled] BIT not null,
   [AccessFailedCount] INT not null,
   [IsSystemUser] BIT not null,
   [Status] INT not null,
   [StatusReason] nvarchar(150) null,
   PartyId BIGINT null,
   primary key (Id),
  unique ([Uid]),
  unique ([UserName]),
  unique ([Email])
)
create table SiteUserMap (
	UserId BIGINT not null,
   SiteId BIGINT not null,
   primary key (SiteId, UserId)
)
create table [UserSession] (
	Id BIGINT IDENTITY NOT NULL,
   [Uid] UNIQUEIDENTIFIER not null,
   [Status] INT not null,
   [StartTimestamp] DATETIME default (getdate())  not null,
   [EndTimestamp] DATETIME null,
   UserId BIGINT not null,
   primary key (Id),
  unique ([Uid])
)
create table [Pingback] (
	Id BIGINT IDENTITY NOT NULL,
   [Uid] UNIQUEIDENTIFIER not null,
   [TargetUri] nvarchar(1024) not null,
   [TargetTitle] nvarchar(1024) null,
   [IsSpam] BIT not null,
   [IsTrackback] BIT not null,
   [Received] DATETIME not null,
   PostId BIGINT null,
   SnippetId BIGINT null,
   primary key (Id),
  unique ([Uid])
)
create table [Portal] (
	Id BIGINT IDENTITY NOT NULL,
   [Uid] UNIQUEIDENTIFIER not null,
   [Name] nvarchar(64) not null,
   [Title] nvarchar(64) not null,
   [Description] nvarchar(255) null,
   OrganisationId BIGINT null,
   OwnerId BIGINT not null,
   primary key (Id),
  unique ([Uid]),
  unique ([Name])
)
create table [Redirect] (
	Id BIGINT IDENTITY NOT NULL,
   [Uid] UNIQUEIDENTIFIER not null,
   [From] nvarchar(1024) not null,
   [To] nvarchar(1024) not null,
   [Timestamp] DATETIME not null,
   primary key (Id),
  unique ([Uid])
)
create table [Setting] (
	Id BIGINT IDENTITY NOT NULL,
   [Uid] UNIQUEIDENTIFIER not null,
   [Section] nvarchar(64) not null,
   [Name] nvarchar(64) not null,
   [Title] nvarchar(64) null,
   [Description] nvarchar(1024) null,
   [Value] ntext null,
   SiteId BIGINT null,
   primary key (Id),
  unique ([Uid])
)
create table [Site] (
	Id BIGINT IDENTITY NOT NULL,
   [Uid] UNIQUEIDENTIFIER not null,
   [Name] nvarchar(64) not null,
   [Title] nvarchar(64) not null,
   [Description] nvarchar(255) null,
   [IsDefault] BIT not null,
   [IsActive] BIT not null,
   [IsPrivate] BIT not null,
   OwnerId BIGINT not null,
   PortalId BIGINT not null,
   primary key (Id),
  unique ([Uid]),
  unique ([Name])
)
create table [Zone] (
	Id BIGINT IDENTITY NOT NULL,
   [Uid] UNIQUEIDENTIFIER not null,
   [Name] nvarchar(64) not null,
   [ZoneType] INT not null,
   [Title] nvarchar(64) not null,
   [IsActive] BIT not null,
   [IsPrivate] BIT not null,
   [Description] nvarchar(255) null,
   SiteId BIGINT not null,
   primary key (Id),
  unique ([Uid]),
  unique ([Name], SiteId)
)
create table [JobTaskDetail] (
	Id BIGINT IDENTITY NOT NULL,
   [Arguments] nvarchar(1024) null,
   [TryNo] INT not null,
   [Status] INT not null,
   [Started] DATETIME default (getdate())  not null,
   [Updated] DATETIME null,
   [Finished] DATETIME null,
   [OutputLog] ntext null,
   JobTaskId BIGINT not null,
   primary key (Id)
)
create table [JobTask] (
	Id BIGINT IDENTITY NOT NULL,
   [Uid] UNIQUEIDENTIFIER not null,
   [Name] nvarchar(64) not null,
   [Status] INT not null,
   [ProgressEstimate] INT not null,
   [Created] DATETIME not null,
   [Completed] DATETIME null,
   [FailedMessage] nvarchar(2048) null,
   [OwnerUserName] nvarchar(150) null,
   JobId BIGINT not null,
   primary key (Id),
  unique ([Uid])
)
create table [PostLink] (
	Id BIGINT IDENTITY NOT NULL,
   [Type] nvarchar(64) not null,
   [Ref] nvarchar(1024) not null,
   PostId BIGINT not null,
   primary key (Id)
)
create table [LogEntry] (
	Id BIGINT IDENTITY NOT NULL,
   [Uid] UNIQUEIDENTIFIER not null,
   [Timestamp] DATETIME not null,
   [Level] INT not null,
   [Message] nvarchar(2048) not null,
   [LoggerName] nvarchar(64) null,
   [Thread] nvarchar(150) null,
   [ProcessUser] nvarchar(150) null,
   [SessionToken] nvarchar(150) null,
   [SessionUser] nvarchar(150) null,
   [StackTrace] nvarchar(2048) null,
   [ClientStackTrace] nvarchar(2048) null,
   [Exception] nvarchar(1024) null,
   [Extra] nvarchar(1024) null,
   primary key (Id),
  unique ([Uid])
)
create table [SnippetComment] (
	Id BIGINT IDENTITY NOT NULL,
   [Uid] UNIQUEIDENTIFIER not null,
   [AuthorName] nvarchar(64) not null,
   [AuthorUrl] nvarchar(1024) null,
   [AuthorEmail] nvarchar(100) null,
   [AuthorIp] nvarchar(32) null,
   [Body] ntext not null,
   [Posted] DATETIME not null,
   [Status] INT not null,
   [RevisionNumber] INT not null,
   SnippetId BIGINT not null,
   primary key (Id),
  unique ([Uid])
)
create table [SnippetLink] (
	Id BIGINT IDENTITY NOT NULL,
   [Type] nvarchar(64) not null,
   [Ref] nvarchar(1024) not null,
   SnippetId BIGINT not null,
   primary key (Id)
)
create table [Snippet] (
	Id BIGINT IDENTITY NOT NULL,
   [Uid] UNIQUEIDENTIFIER not null,
   [Name] nvarchar(64) not null,
   [Title] nvarchar(64) not null,
   [MimeType] nvarchar(150) null,
   [Author] nvarchar(64) null,
   [CommentsCount] INT not null,
   [Rating] DECIMAL(19,5) not null,
   [DisableDiscussionDays] INT null,
   [IsDiscussionEnabled] BIT not null,
   [IsRatingEnabled] BIT not null,
   [IsAnonymousCommentAllowed] BIT not null,
   [Created] DATETIME not null,
   [IsPrivate] BIT not null,
   [IsContentBinary] BIT not null,
   CreatorId BIGINT not null,
   LastRevisionId BIGINT null,
   RevisionNumber INT not null,
   RevisionAuthor nvarchar(64) null,
   [Body] ntext not null,
   [BodyImage] VARBINARY(MAX) null,
   LastRevised DATETIME not null,
   LatestRevisionEncoding nvarchar(64) null,
   primary key (Id),
  unique ([Uid]),
  unique ([Name])
)
create table [SnippetRating] (
	Id BIGINT IDENTITY NOT NULL,
   [Rate] DECIMAL(19,5) not null,
   [RatedDate] DATETIME not null,
   [UserIdentity] nvarchar(100) null,
   [UserEmail] nvarchar(100) null,
   SnippetId BIGINT not null,
   primary key (Id)
)
create table [SnippetRevision] (
	Id BIGINT IDENTITY NOT NULL,
   [RevisionNumber] INT not null,
   [Summary] ntext null,
   [Author] nvarchar(64) null,
   [RevisedDate] DATETIME not null,
   [ReviseReason] nvarchar(150) null,
   [BodyEncoding] nvarchar(64) null,
   [Body] ntext not null,
   [BodyImage] VARBINARY(MAX) null,
   SnippetId BIGINT not null,
   ReviserId BIGINT not null,
   primary key (Id)
)
alter table [Job] 
	add constraint FK_Jobs_User 
	foreign key (OwnerId) 
	references [User]
alter table [Job] 
	add constraint FK_Jobs_Site 
	foreign key (SiteId) 
	references [Site]
alter table [Address] 
	add constraint FK_Addresses_Party 
	foreign key (PartyId) 
	references [Party]
alter table PartyRoleMap 
	add constraint FK_PartyRoleMap_PartyRole 
	foreign key (PartyRoleId) 
	references [PartyRole]
alter table PartyRoleMap 
	add constraint FK_PartyRoleMap_Party 
	foreign key (PartyId) 
	references [Party]
alter table [Organisation] 
	add constraint FK_Party_Organisation 
	foreign key (Id) 
	references [Party]
alter table [Person] 
	add constraint FK_Party_Person 
	foreign key (Id) 
	references [Party]
alter table [Telephone] 
	add constraint FK_Telephones_Party 
	foreign key (PartyId) 
	references [Party]
alter table [Comment] 
	add constraint FK_Comments_Post 
	foreign key (PostId) 
	references [Post]
alter table [Post] 
	add constraint FK_Entries_Zone 
	foreign key (ZoneId) 
	references [Zone]
alter table [Post] 
	add constraint FK_Posts_PostSerie 
	foreign key (PostSerieId) 
	references [PostSerie]
alter table [Post] 
	add constraint FK_Creator_Post 
	foreign key (CreatorId) 
	references [User]
alter table [Post] 
	add constraint FK_Entries_Site 
	foreign key (SiteId) 
	references [Site]
alter table TagPostMap 
	add constraint FK_TagPostMap_Tag 
	foreign key (TagId) 
	references [Tag]
alter table TagPostMap 
	add constraint FK_TagPostMap_Post 
	foreign key (PostId) 
	references [Post]
alter table [PostRevision] 
	add constraint FK_Revisions_Post 
	foreign key (PostId) 
	references [Post]
alter table [PostRevision] 
	add constraint FK_Reviser_PostRevision 
	foreign key (ReviserId) 
	references [User]
alter table [PostRevision] 
	add constraint FK_Approver_PostRevision 
	foreign key (ApproverId) 
	references [User]
alter table [PostRevision] 
	add constraint FK_Publisher_PostRevision 
	foreign key (PublisherId) 
	references [User]
alter table [PostRevision] 
	add constraint FK_PostRevisions_User 
	foreign key (OwnerId) 
	references [User]
alter table [PostSerie] 
	add constraint FK_PostSeries_Site 
	foreign key (SiteId) 
	references [Site]
alter table [Rating] 
	add constraint FK_Ratings_Post 
	foreign key (PostId) 
	references [Post]
alter table [Tag] 
	add constraint FK_Tags_Site 
	foreign key (SiteId) 
	references [Site]
alter table [Category] 
	add constraint FK_Tag_Category 
	foreign key (Id) 
	references [Tag]
alter table [Category] 
	add constraint FK_Parent_Category 
	foreign key (ParentId) 
	references [Category]
alter table [AppFunction] 
	add constraint FK_Parent_AppFunction 
	foreign key (ParentId) 
	references [AppFunction]
alter table AppFunctionRoleMap 
	add constraint FK_AppFunctionRoleMap_Role 
	foreign key (RoleId) 
	references [Role]
alter table AppFunctionRoleMap 
	add constraint FK_AppFunctionRoleMap_AppFunction 
	foreign key (AppFunctionId) 
	references [AppFunction]
alter table [PostPermission] 
	add constraint FK_Permissions_User 
	foreign key (UserId) 
	references [User]
alter table [PostPermission] 
	add constraint FK_Permissions_Post 
	foreign key (PostId) 
	references [Post]
alter table [PostWorkflowRule] 
	add constraint FK_ApproverRole_PostWorkflowRule 
	foreign key (ApproverRoleId) 
	references [Role]
alter table [PostWorkflowRule] 
	add constraint FK_PublisherRole_PostWorkflowRule 
	foreign key (PublisherRoleId) 
	references [Role]
alter table [PostWorkflowRule] 
	add constraint FK_WorkflowRules_Site 
	foreign key (SiteId) 
	references [Site]
alter table UserRoleMap 
	add constraint FK_UserRoleMap_User 
	foreign key (UserId) 
	references [User]
alter table UserRoleMap 
	add constraint FK_UserRoleMap_Role 
	foreign key (RoleId) 
	references [Role]
alter table [UserClaim] 
	add constraint FK_UserClaims_User 
	foreign key (UserId) 
	references [User]
alter table [UserLogin] 
	add constraint FK_UserLogins_User 
	foreign key (UserId) 
	references [User]
alter table [User] 
	add constraint FK_Users_Party 
	foreign key (PartyId) 
	references [Party]
alter table SiteUserMap 
	add constraint FK_SiteUserMap_Site 
	foreign key (SiteId) 
	references [Site]
alter table SiteUserMap 
	add constraint FK_SiteUserMap_User 
	foreign key (UserId) 
	references [User]
alter table [UserSession] 
	add constraint FK_UserSessions_User 
	foreign key (UserId) 
	references [User]
alter table [Pingback] 
	add constraint FK_Pingbacks_Post 
	foreign key (PostId) 
	references [Post]
alter table [Pingback] 
	add constraint FK_Pingbacks_Snippet 
	foreign key (SnippetId) 
	references [Snippet]
alter table [Portal] 
	add constraint FK_Organisation_Portal 
	foreign key (OrganisationId) 
	references [Organisation]
alter table [Portal] 
	add constraint FK_Owner_Portal 
	foreign key (OwnerId) 
	references [User]
alter table [Setting] 
	add constraint FK_Settings_Site 
	foreign key (SiteId) 
	references [Site]
alter table [Site] 
	add constraint FK_Owner_Site 
	foreign key (OwnerId) 
	references [User]
alter table [Site] 
	add constraint FK_Sites_Portal 
	foreign key (PortalId) 
	references [Portal]
alter table [Zone] 
	add constraint FK_Zones_Site 
	foreign key (SiteId) 
	references [Site]
alter table [JobTaskDetail] 
	add constraint FK_Details_JobTask 
	foreign key (JobTaskId) 
	references [JobTask]
alter table [JobTask] 
	add constraint FK_Tasks_Job 
	foreign key (JobId) 
	references [Job]
alter table [PostLink] 
	add constraint FK_Links_Post 
	foreign key (PostId) 
	references [Post]
alter table [SnippetComment] 
	add constraint FK_Comments_Snippet 
	foreign key (SnippetId) 
	references [Snippet]
alter table [SnippetLink] 
	add constraint FK_Links_Snippet 
	foreign key (SnippetId) 
	references [Snippet]
alter table [Snippet] 
	add constraint FK_Creator_Snippet 
	foreign key (CreatorId) 
	references [User]
alter table [SnippetRating] 
	add constraint FK_Ratings_Snippet 
	foreign key (SnippetId) 
	references [Snippet]
alter table [SnippetRevision] 
	add constraint FK_Revisions_Snippet 
	foreign key (SnippetId) 
	references [Snippet]
alter table [SnippetRevision] 
	add constraint FK_Reviser_SnippetRevision 
	foreign key (ReviserId) 
	references [User]