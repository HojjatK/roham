using System.Collections.Generic;
using System.Web.Mvc;

namespace Roham.Web.Areas.Admin.ViewModels
{
    public class SqlServerViewModel
    {
        public SqlServerViewModel()
        {
            Authentications = new List<SelectListItem>
            {
                new SelectListItem { Text = "Database Authentication", Value = "false" },
                new SelectListItem { Text = "Windows Authenctication" , Value = "true" },
            };
            IntegratedSecurity = false;
        }

        public string ConnectionString { get; set; }

        public string DatabaseServer { get; set; }

        public string DatabaseName { get; set; }

        public bool IntegratedSecurity { get; set; }

        public List<SelectListItem> Authentications { get; set; }

        public string Username { get; set; }

        public string Password { get; set; }
    }
}
