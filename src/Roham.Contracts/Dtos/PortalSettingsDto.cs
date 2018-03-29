using Roham.Lib.Domain;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace Roham.Contracts.Dtos
{
    public class PortalSettingsDto 
    {   
        public List<KeyValuePair<string, string>> AvailableStorageProviders { get; set; }

        [MaxLength(Lengths.LongName)]
        public string StorageProvider { get; set; }

        [MaxLength(Lengths.Location)]
        public string UploadPath { get; set; }

        [MaxLength(Lengths.Connection)]
        public string StorageConnectionString { get; set; }

        [MaxLength(Lengths.LongName)]
        public string BlobContainerName { get; set; }

        public List<KeyValuePair<string, string>> AvailableThemes { get; set; }

        [MaxLength(Lengths.Name)]
        public string AdminTheme { get; set; }
    }
}
