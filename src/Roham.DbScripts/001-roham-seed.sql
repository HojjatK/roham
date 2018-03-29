-- ====================================
-- Roham initialize database script
-- ====================================

------------------
-- Roles 
------------------
declare @SysAdminRoleId bigint, @AdminRoleId bigint, @ModeratorId bigint;

insert into [dbo].[Role]([Uid], Name, [Description], RoleType, IsSystemRole) values(NEWID(), 'SysAdmin', 'System Administrator', 1, 1)
select @SysAdminRoleId = SCOPE_IDENTITY()

insert into [dbo].[Role]([Uid], Name, [Description], RoleType, IsSystemRole) values(NEWID(), 'Admin', 'Administrator', 2, 1)
select @AdminRoleId = SCOPE_IDENTITY()

insert into [dbo].[Role]([Uid], Name, [Description], RoleType, IsSystemRole) values(NEWID(), 'Modertor', 'Moderator users', 3, 1)
select @ModeratorId = SCOPE_IDENTITY()

------------------
-- AppFunction
------------------
-- Dashboard
declare @parentId bigint, @parentId2 bigint;
insert into [dbo].[AppFunction]([Uid], [Key], [Name], [Title], [Description], ParentId) values(NEWID(), 1, 'dashboard', 'Dashboard', 'User dashboard', null) 
											  
-- Congis									  
insert into [dbo].[AppFunction]([Uid], [Key], [Name], [Title], [Description], ParentId) values(NEWID(), 2, 'configs', 'Configurations', 'Portal global configurations', null) 
select @parentId = SCOPE_IDENTITY()			  
insert into [dbo].[AppFunction]([Uid], [Key], [Name], [Title], [Description], ParentId) values(NEWID(), 21, 'portal', 'Portal', 'Portal information', @parentId) 
insert into [dbo].[AppFunction]([Uid], [Key], [Name], [Title], [Description], ParentId) values(NEWID(), 22, 'portal-settings', 'Portal Settings', 'Portal settings', @parentId);
											  
-- Site, Zone, Category						  
insert into [dbo].[AppFunction]([Uid], [Key], [Name], [Title], [Description], ParentId) values(NEWID(), 3, 'sites', 'Sites', 'View sites', null);
select @parentId = SCOPE_IDENTITY()			  
insert into [dbo].[AppFunction]([Uid], [Key], [Name], [Title], [Description], ParentId) values(NEWID(), 31, 'create-site', 'Create Site', 'Create a new site', @parentId);
insert into [dbo].[AppFunction]([Uid], [Key], [Name], [Title], [Description], ParentId) values(NEWID(), 32, 'update-site', 'Update Site', 'Update site information and its setting', @parentId);
insert into [dbo].[AppFunction]([Uid], [Key], [Name], [Title], [Description], ParentId) values(NEWID(), 33, 'delete-site', 'Delete Site', 'Delete an existing site', @parentId);
insert into [dbo].[AppFunction]([Uid], [Key], [Name], [Title], [Description], ParentId) values(NEWID(), 34, 'site-settings', 'Site Settings', 'Site settings', @parentId);
insert into [dbo].[AppFunction]([Uid], [Key], [Name], [Title], [Description], ParentId) values(NEWID(), 35, 'zones', 'Zones', 'View site''s zones', @parentId);
select @parentId2 = SCOPE_IDENTITY()		  
insert into [dbo].[AppFunction]([Uid], [Key], [Name], [Title], [Description], ParentId) values(NEWID(), 351, 'create-zone', 'Create Zone', 'Create a new zone for a site', @parentId2);
insert into [dbo].[AppFunction]([Uid], [Key], [Name], [Title], [Description], ParentId) values(NEWID(), 352, 'update-zone', 'Update Zone', 'Update an existing zone information', @parentId2);
insert into [dbo].[AppFunction]([Uid], [Key], [Name], [Title], [Description], ParentId) values(NEWID(), 353, 'delete-zone', 'Delete Zone', 'Delete an existing zone', @parentId2);
insert into [dbo].[AppFunction]([Uid], [Key], [Name], [Title], [Description], ParentId) values(NEWID(), 36, 'categories', 'Categories', 'View categories', @parentId) 
select @parentId2 = SCOPE_IDENTITY()			
insert into [dbo].[AppFunction]([Uid], [Key], [Name], [Title], [Description], ParentId) values(NEWID(), 361, 'create-category', 'Create Category', 'Create a new category', @parentId2);
insert into [dbo].[AppFunction]([Uid], [Key], [Name], [Title], [Description], ParentId) values(NEWID(), 362, 'update-category', 'Update Category', 'Update an existing category information', @parentId2);
insert into [dbo].[AppFunction]([Uid], [Key], [Name], [Title], [Description], ParentId) values(NEWID(), 363, 'delete-category', 'Delete Category', 'Delete an existing category', @parentId2);
											  
-- Roles 							  		  
insert into [dbo].[AppFunction]([Uid], [Key], [Name], [Title], [Description], ParentId) values(NEWID(), 4, 'roles', 'Roles', 'View roles', null);
select @parentId = SCOPE_IDENTITY()			  
insert into [dbo].[AppFunction]([Uid], [Key], [Name], [Title], [Description], ParentId) values(NEWID(), 41, 'create-role', 'Create Role', 'Create a new user''s role', @parentId);
insert into [dbo].[AppFunction]([Uid], [Key], [Name], [Title], [Description], ParentId) values(NEWID(), 42, 'update-role', 'Update Role', 'Update an existing user''s role information', @parentId);
insert into [dbo].[AppFunction]([Uid], [Key], [Name], [Title], [Description], ParentId) values(NEWID(), 43, 'delete-role', 'Delete Role', 'Delete an existing user''s role', @parentId);
											  
-- Users		  							  
insert into [dbo].[AppFunction]([Uid], [Key], [Name], [Title], [Description], ParentId) values(NEWID(), 5, 'users', 'Users', 'View users', null) 
select @parentId = SCOPE_IDENTITY()			  
insert into [dbo].[AppFunction]([Uid], [Key], [Name], [Title], [Description], ParentId) values(NEWID(), 51, 'create-user', 'Create User', 'Create a new user', @parentId);
insert into [dbo].[AppFunction]([Uid], [Key], [Name], [Title], [Description], ParentId) values(NEWID(), 52, 'update-user', 'Update User', 'Update an existing user''s information', @parentId);
insert into [dbo].[AppFunction]([Uid], [Key], [Name], [Title], [Description], ParentId) values(NEWID(), 53, 'delete-user', 'Delete User', 'Delete an existing user', @parentId);											  
insert into [dbo].[AppFunction]([Uid], [Key], [Name], [Title], [Description], ParentId) values(NEWID(), 54, 'user-permissions', 'User Permissions', 'View users'' permissions', @parentId);
select @parentId2 = SCOPE_IDENTITY()			
insert into [dbo].[AppFunction]([Uid], [Key], [Name], [Title], [Description], ParentId) values(NEWID(), 541, 'update-user-permission', 'Update User Permission', 'Update user''s permission', @parentId2);
insert into [dbo].[AppFunction]([Uid], [Key], [Name], [Title], [Description], ParentId) values(NEWID(), 542, 'delete-user-permission', 'Delete User Permission', 'Delete user''s permission', @parentId2);						  
										  	  
-- Posts									  
insert into [dbo].[AppFunction]([Uid], [Key], [Name], [Title], [Description], ParentId) values(NEWID(), 6, 'posts', 'Posts', 'View post entries', null) 
select @parentId = SCOPE_IDENTITY()			  
insert into [dbo].[AppFunction]([Uid], [Key], [Name], [Title], [Description], ParentId) values(NEWID(), 61, 'create-post', 'Create Post', 'Create a new entry', @parentId);
insert into [dbo].[AppFunction]([Uid], [Key], [Name], [Title], [Description], ParentId) values(NEWID(), 62, 'save-post', 'Save Post', 'Update an existing entry meta data', @parentId);
insert into [dbo].[AppFunction]([Uid], [Key], [Name], [Title], [Description], ParentId) values(NEWID(), 63, 'revise-post', 'Revise Post', 'Create a new revision for an existing Entry', @parentId);
insert into [dbo].[AppFunction]([Uid], [Key], [Name], [Title], [Description], ParentId) values(NEWID(), 64, 'publish-post', 'Publish Post', 'Publish entry', @parentId);
insert into [dbo].[AppFunction]([Uid], [Key], [Name], [Title], [Description], ParentId) values(NEWID(), 65, 'delete-post', 'Delete Post', 'Delete an existing entry revision', @parentId);
insert into [dbo].[AppFunction]([Uid], [Key], [Name], [Title], [Description], ParentId) values(NEWID(), 66, 'post-series', 'Post Series', 'View entry series', @parentId) 
select @parentId2 = SCOPE_IDENTITY()		  
insert into [dbo].[AppFunction]([Uid], [Key], [Name], [Title], [Description], ParentId) values(NEWID(), 661, 'create-post-serie', 'Create Post Serie', 'Create a new entry serie', @parentId2);
insert into [dbo].[AppFunction]([Uid], [Key], [Name], [Title], [Description], ParentId) values(NEWID(), 662, 'update-post-serie', 'Update Post Serie', 'Update an existing entry serie', @parentId2);
insert into [dbo].[AppFunction]([Uid], [Key], [Name], [Title], [Description], ParentId) values(NEWID(), 663, 'delete-post-serie', 'Delete Post Serie', 'Delete an existing entry serie', @parentId2);											  
insert into [dbo].[AppFunction]([Uid], [Key], [Name], [Title], [Description], ParentId) values(NEWID(), 67, 'comments', 'Comments', 'View entries'' comments', @parentId) 
select @parentId2 = SCOPE_IDENTITY()		  
insert into [dbo].[AppFunction]([Uid], [Key], [Name], [Title], [Description], ParentId) values(NEWID(), 671, 'comment-as-spam', 'Declare Comment As Spam', 'Declare a comment of an entry as an spam', @parentId2);
insert into [dbo].[AppFunction]([Uid], [Key], [Name], [Title], [Description], ParentId) values(NEWID(), 672, 'delete-comment', 'Delete Comment', 'Delete an existing comment of an entry', @parentId2);
insert into [dbo].[AppFunction]([Uid], [Key], [Name], [Title], [Description], ParentId) values(NEWID(), 673, 'disable-comment', 'Disable Comment', 'Disable putting comments in an entry', @parentId2);											  
insert into [dbo].[AppFunction]([Uid], [Key], [Name], [Title], [Description], ParentId) values(NEWID(), 68, 'tags', 'Tags', 'View entries''s tags', @parentId) 
select @parentId2 = SCOPE_IDENTITY()		  
insert into [dbo].[AppFunction]([Uid], [Key], [Name], [Title], [Description], ParentId) values(NEWID(), 681, 'revise-tag', 'Revise Tag', 'Revise an existing tag given to an entry', @parentId2);
insert into [dbo].[AppFunction]([Uid], [Key], [Name], [Title], [Description], ParentId) values(NEWID(), 682, 'delete-tag', 'Delete Tag', 'Delete a tag from an entry', @parentId2);
insert into [dbo].[AppFunction]([Uid], [Key], [Name], [Title], [Description], ParentId) values(NEWID(), 69, 'ratings', 'Ratings', 'View entries'' ratings', @parentId) 
select @parentId2 = SCOPE_IDENTITY()		  
insert into [dbo].[AppFunction]([Uid], [Key], [Name], [Title], [Description], ParentId) values(NEWID(), 691, 'reset-rating', 'Reset Rating', 'Reset an entry ratings', @parentId2);
insert into [dbo].[AppFunction]([Uid], [Key], [Name], [Title], [Description], ParentId) values(NEWID(), 692, 'disable-rating', 'Disable Rating', 'Disable entry ratings', @parentId2);
											  
-- Jobs										  
insert into [dbo].[AppFunction]([Uid], [Key], [Name], [Title], [Description], ParentId) values(NEWID(), 7, 'jobs', 'Jobs', 'View jobs', null) 
select @parentId = SCOPE_IDENTITY()			  
insert into [dbo].[AppFunction]([Uid], [Key], [Name], [Title], [Description], ParentId) values(NEWID(), 71, 'create-job', 'Create Job', 'Create a new job', @parentId) 
insert into [dbo].[AppFunction]([Uid], [Key], [Name], [Title], [Description], ParentId) values(NEWID(), 72, 'update-job', 'Update Job', 'Update an existing job information', @parentId) 
insert into [dbo].[AppFunction]([Uid], [Key], [Name], [Title], [Description], ParentId) values(NEWID(), 73, 'delete-job', 'Delete Job', 'Delete an existing job', @parentId) 
insert into [dbo].[AppFunction]([Uid], [Key], [Name], [Title], [Description], ParentId) values(NEWID(), 74, 'execute-job', 'Execute Job', 'Execute an existing job', @parentId) 
											  
-- Report Management						  
insert into [dbo].[AppFunction]([Uid], [Key], [Name], [Title], [Description], ParentId) values(NEWID(), 8, 'reports', 'Reports', 'View Reports', null) 


-- SysAdmin has permission to all functions
insert into [dbo].AppFunctionRoleMap(AppFunctionId, RoleId)
select Id, @SysAdminRoleId from [dbo].AppFunction

-- Admin
insert into [dbo].AppFunctionRoleMap(AppFunctionId, RoleId)
select Id, @AdminRoleId from [dbo].AppFunction where Name in 
('dashboard', 
 'sites','create-site', 'update-site', 'delete-site', 'site-settings',
 'zones', 'create-zone', 'delete-zone', 
 'categories', 'create-category', 'update-category', 'delete-category',
 'roles', 'users', 'create-user', 'update-user', 'delete-user', 
 'posts', 'create-post', 'save-post', 'revise-post', 'publish-post', 'delete-post', 
 'post-series', 'create-post-serie', 'update-post-serie', 'delete-post-serie', 
 'comments', 'comment-as-spam', 'delete-comment', 'disable-comment',
 'tags', 'revise-tag', 'delete-tag',
 'ratings', 'reset-rating', 'disable-rating',
 'jobs', 'create-job', 'update-job', 'delete-job', 'execute-job', 
 'reports')

-- ModeratorUser
insert into [dbo].AppFunctionRoleMap(AppFunctionId, RoleId)
select Id, @ModeratorId from [dbo].AppFunction where Name in 
('dashboard', 
 'posts', 'create-post', 'save-post', 'revise-post', 'publish-post', 'delete-post', 
 'post-series', 'create-post-serie', 'update-post-serie', 'delete-post-serie', 
 'comments', 
 'tags',
 'ratings',
 'reports')
 go

------------------
-- ZoneType
------------------
--insert into [dbo].[ZoneType]([Uid], Name, Code, [Description]) values(NEWID(), 'Web Content', 1, 'Generic web content entries')
--insert into [dbo].[ZoneType]([Uid], Name, Code, [Description]) values(NEWID(), 'Blog', 2, 'Blog entries')
--insert into [dbo].[ZoneType]([Uid], Name, Code, [Description]) values(NEWID(), 'Wiki', 3, 'Wiki entries')
--insert into [dbo].[ZoneType]([Uid], Name, Code, [Description]) values(NEWID(), 'News', 4, 'News and announcement entries')
--insert into [dbo].[ZoneType]([Uid], Name, Code, [Description]) values(NEWID(), 'Document', 5, 'Document, image or any binary content entries')
--go

------------------
-- Partyrole
------------------
insert into [dbo].[PartyRole] ([Uid], Name, [Description]) values (NEWID(), 'Customer', 'Customer Party Role')
insert into [dbo].[PartyRole] ([Uid], Name, [Description]) values (NEWID(), 'Worker', 'Worker Party Role')
go