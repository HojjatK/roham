using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Web.Mvc;
using Roham.Data;
using Roham.Contracts.Dtos;
using Roham.Resources;
using Roham.Domain.Entities.Sites;

namespace Roham.Web.Areas.Admin.ViewModels
{
    public class InstallViewModel
    {
        public InstallViewModel()
        {
            Uid = Guid.NewGuid().ToString();
            PortalName = WebAppInfo.Name;
            AdminUserName = "";
            DatabaseProviders = new List<SelectListItem>
            {
                new SelectListItem { Text ="Sql Server", Value = DbProviders.SqlServer.ToString() },
                new SelectListItem { Text ="SQLite", Value = DbProviders.SQLite.ToString() }
            };
            CacheProviders = new List<SelectListItem>
            {
                new SelectListItem { Text = "In-Memory (In Process)", Value = Data.CacheProviders.Memory.ToString() },
                new SelectListItem { Text = "Redis (Out of Process )", Value = Data.CacheProviders.Redis.ToString() }
            };
            EmailProviders = new List<SelectListItem>
            {
                new SelectListItem { Text = "Smtp", Value = "smtp" },
                new SelectListItem { Text = "None", Value = null }
            };
            Zones = new List<InstallZoneDto> {
                new InstallZoneDto {
                    Code = ZoneTypeCodes.Blog.ToString(),
                    Name = Labels.Blog,
                    Title = Labels.Blog,
                    Description = ScreenTexts.BlogZone_Description
                },

                new InstallZoneDto {
                    Code = ZoneTypeCodes.Blog.ToString(),
                    Name = Labels.Article,
                    Title = Labels.Article,
                    Description = ScreenTexts.ArticleZone_Description
                },

                new InstallZoneDto {
                    Code = ZoneTypeCodes.Blog.ToString(),
                    Name = Labels.Lab,
                    Title = Labels.Lab,
                    Description = ScreenTexts.LabZone_Description
                },

                new InstallZoneDto {
                    Code = ZoneTypeCodes.Wikki.ToString(),
                    Name = Labels.Wikki,
                    Title = Labels.Wikki,
                    Description = ScreenTexts.WikkiZone_Description
                },

                new InstallZoneDto {
                    Code = ZoneTypeCodes.News.ToString(),
                    Name = Labels.News,
                    Title = Labels.News,
                    Description = ScreenTexts.NewsZone_Description
                }
            };
            SelectedZones = new[] { Labels.Blog, Labels.Article, Labels.Lab };            
            Smtp = new SmtpViewModel
            {
                Host = "localhost",
                Port = 25
            };
        }

        public bool IsInstalled { get; set; }

        public string InstallationKey { get; set; }

        [Required(ErrorMessage = "Portal name is requried")]
        [MinLength(3, ErrorMessage = "Portal name should have at least 3 characters")]
        public string PortalName { get; set; }

        [Required(ErrorMessage = "Admin user name is requried")]
        [MinLength(3, ErrorMessage = "Admin user name should have at least 3 characters")]
        public string AdminUserName { get; set; }

        [Required(ErrorMessage = "Admin user password is requried")]
        [MinLength(8, ErrorMessage = "Password should have at least 8 characters")]
        public string AdminPassword { get; set; }

        [Required(ErrorMessage = "Confirm admin user password is requried")]
        [MinLength(8, ErrorMessage = "Password should have at least 8 characters")]
        public string ConfirmAdminPassword { get; set; }

        public List<InstallZoneDto> Zones { get; set; }
        public string[] SelectedZones { get; set; }

        [Required(ErrorMessage = "Database provider is requried")]
        public string SelectedDatabaseProvider { get; set; }

        public List<SelectListItem> DatabaseProviders { get; }

        public bool Advanced { get; set; }

        public SqlServerViewModel SqlServer { get; set; }

        public SqliteViewModel Sqlite { get; set; }

        public SmtpViewModel Smtp { get; set; }

        [Required(ErrorMessage = "Cache provider is requried")]
        public string SelectedCacheProvider { get; set; }

        public List<SelectListItem> CacheProviders { get; }

        public CacheConfigsViewModel CacheConfigs { get; set; }

        public bool AdvancedCache { get; set; }

        [Required(ErrorMessage = "Email provider is requried")]
        public string SelectedEmailProvider { get; set; }

        public List<SelectListItem> EmailProviders { get; }

        public bool UseSmtp => SelectedEmailProvider == "smtp";

        public string SmtpFrom { get; set; }

        public string Uid { get; set; }
    }
}
