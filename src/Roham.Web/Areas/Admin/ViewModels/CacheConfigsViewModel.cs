namespace Roham.Web.Areas.Admin.ViewModels
{
    public class CacheConfigsViewModel
    {
        public Data.CacheProviders Provider { get; set; }

        public string ConnectionString { get; set; }
        
        public string Host { get; set; }

        public int Port { get; set; }

        public string Password { get; set; }

        public bool Ssl { get; set; }
    }
}
