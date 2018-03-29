using Roham.Lib.Domain;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

namespace Roham.Domain.Settings
{   
    public class PortalSettings : ISettings
    {
        public long? SiteId
        {
            get { return null; }
            set { }
        }

        [DefaultValue("filesystem")]
        [Description("The storage provider")]
        [SettingKey("storage-provider")]
        [StringLength(Lengths.LongName)]
        public string StorageProvider { get; set; }

        [DefaultValue("files")]
        [Description("Files uploaded for entires will be stored here. Use ~/XYZ to indicate a file path under the website root.")]
        [SettingKey("upload-path")]
        [StringLength(Lengths.Location)]
        public string UploadPath { get; set; }

        [Description("The container to store blobs in.")]
        [SettingKey("storage-connection-string")]
        [StringLength(Lengths.Connection)]
        public string StorageConnectionString { get; set; }

        [Description("The container to store blobs in.")]
        [DefaultValue("RohamWeb")]
        [SettingKey("blog-container-name")]
        [StringLength(Lengths.LongName)]
        public string BlobContainerName { get; set; }

        [DefaultValue("dark")]
        [Description("The theme which will be used for admin website.")]
        [SettingKey("admin-theme")]
        [StringLength(Lengths.LongName)]
        public string AdminTheme { get; set; }
    }
}
