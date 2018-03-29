using System.Collections.Generic;
using System.Web.Http;
using Roham.Lib.Domain.CQS.Command;
using Roham.Lib.Domain.CQS.Query;
using Roham.Contracts.Commands.Site;
using Roham.Contracts.Dtos;
using Roham.Contracts.Queries;
using Roham.Domain.Entities.Sites;
using Roham.Domain.Settings;
using System;
using Roham.Domain.Entities.Security;

namespace Roham.Web.Controllers.Api
{   
    [RoutePrefix("api/site")]
    [Authorize(Roles = SecurityRoleNames.SysAdmin_Admin)]
    public class SiteController : ApiControllerBase
    {
        private ISettingsProvider SettingsProvider { get; }

        public SiteController(
            ISettingsProvider settingsProvider,
            IQueryExecutor queryExecutor,
            ICommandDispatcher commandDispatcher) : base(queryExecutor, commandDispatcher)
        {
            SettingsProvider = settingsProvider;
        }

        [HttpGet]
        [Route("")]
        public List<SiteDto> GetSites()
        {
            return QueryExecutor.Execute(new FindAllQuery<SiteDto, Site>());
        }

        [HttpGet]
        [Route("{id:long}")]
        public SiteDto GetSite(long id)
        {
            return QueryExecutor.Execute(new FindByIdQuery<SiteDto, Site>(id));
        }

        [HttpPost]        
        [Route("")]
        [Authorize(Roles = SecurityRoleNames.SysAdmin)]
        public ResultDto NewSite(SiteDto siteDto)
        {
            return Result(() =>
            {
                var command = new AddSiteCommand
                {
                    SiteTitle = siteDto.Title,
                    Name = siteDto.Name,
                    IsActive = siteDto.IsActive,
                    IsPublic = siteDto.IsPublic,
                    IsDefault = siteDto.IsDefault,
                    Description = siteDto.Description,
                    OwnerUsername = User.Identity.Name, // current user
                };
                CommandDispatcher.Send(command);
            });
        }

        [HttpPut]        
        [Route("{id:long}")]
        [Authorize(Roles = SecurityRoleNames.SysAdmin)]
        public ResultDto UpdateSite(long id, SiteDto siteDto)
        {
            return Result(() => {
                var command = new UpdateSiteCommand
                {
                    SiteId = id,
                    SiteTitle = siteDto.Title,
                    Name = siteDto.Name,
                    IsActive = siteDto.IsActive,
                    IsPublic = siteDto.IsPublic,
                    Description = siteDto.Description,
                };
                CommandDispatcher.Send(command);
            });
        }

        [HttpDelete]        
        [Route("{id:long}")]
        [Authorize(Roles = SecurityRoleNames.SysAdmin)]
        public ResultDto DeleteSite(long id)
        {
            return Result(() =>
            {
                var command = new DeleteSiteCommand
                {
                    Id = id
                };
                CommandDispatcher.Send(command);
            });
        }

        [HttpGet]
        [Route("settings/{siteId}")]
        public SiteSettingsDto GetSiteSettings(long siteId, bool @default = false)
        {
            var siteSettings = @default ?
                SettingsProvider.GetDefaultSettings<SiteSettings>(siteId) :
                SettingsProvider.GetSettings<SiteSettings>(siteId);

            var siteDto = QueryExecutor.Execute(new FindByIdQuery<SiteDto, Domain.Entities.Sites.Site>(siteId));
            return ConvertFrom(siteDto, siteSettings);
        }

        [HttpPut]
        [Route("settings")]
        public ResultDto SaveSiteSettings(SiteSettingsDto siteSettingsDto)
        {
            return Result(() =>
            {
                var siteSettings = ConvertTo(siteSettingsDto);
                SettingsProvider.SaveSettings(siteSettings);
            });
        }

        private SiteSettingsDto ConvertFrom(SiteDto site, SiteSettings siteSettings)
        {
            if (siteSettings.SiteId != null && site.Id != siteSettings.SiteId.Value)
            {
                throw new ArgumentException("SiteId arguemtn is invalid");
            }
            return new SiteSettingsDto
            {
                SiteId = site.Id,
                SiteTitle = site.Title,
                AkismetApiKey = siteSettings.AkismetApiKey,
                Introduction = siteSettings.Introduction,
                MainLinks = siteSettings.MainLinks,
                PageTemplate = siteSettings.DefaultPage,
                SearchAuthor = siteSettings.SearchAuthor,
                SearchDescription = siteSettings.SearchDescription,
                SearchKeywords = siteSettings.SearchKeywords,
                SpamWords = siteSettings.SpamWords,
                HtmlHead = siteSettings.HtmlHead,
                HtmlFooter = siteSettings.HtmlFooter,
                Theme = siteSettings.SiteTheme,
                SmtpFromEmailAddress = siteSettings.SmtpFromEmailAddress,

                AvailableThemes = new List<KeyValuePair<string, string>>()
                {
                    new KeyValuePair<string, string>("default", "Default"),
                    new KeyValuePair<string, string>("dark", "Dark"),
                    new KeyValuePair<string, string>("redmine", "redmine"),
                },
                AvailablePageTemplates = new List<KeyValuePair<string, string>>
                {
                    new KeyValuePair<string, string>("default", "Default")
                }
            };
        }

        private SiteSettings ConvertTo(SiteSettingsDto siteSettingsDto)
        {
            return new SiteSettings
            {
                SiteId = siteSettingsDto.SiteId,
                AkismetApiKey = siteSettingsDto.AkismetApiKey,
                Introduction = siteSettingsDto.Introduction,
                MainLinks = siteSettingsDto.MainLinks,
                Footer = siteSettingsDto.MainLinks,
                DefaultPage = siteSettingsDto.PageTemplate,
                SearchAuthor = siteSettingsDto.SearchAuthor,
                SearchDescription = siteSettingsDto.SearchDescription,
                SearchKeywords = siteSettingsDto.SearchKeywords,
                SpamWords = siteSettingsDto.SpamWords,
                HtmlHead = siteSettingsDto.HtmlHead,
                HtmlFooter = siteSettingsDto.HtmlFooter,
                SiteTheme = siteSettingsDto.Theme,
                SmtpFromEmailAddress = siteSettingsDto.SmtpFromEmailAddress
            };
        }
    }
}
