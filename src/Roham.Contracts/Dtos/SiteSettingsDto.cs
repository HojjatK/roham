using Roham.Lib.Domain;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace Roham.Contracts.Dtos
{
    public class SiteSettingsDto 
    {
        [Required]
        public long SiteId { get; set; }

        [MaxLength(Lengths.Name)]
        public string SiteTitle { get; set; }

        public List<KeyValuePair<string, string>> AvailableThemes { get; set; }

        [MaxLength(Lengths.Name)]
        public string Theme { get; set; }

        public List<KeyValuePair<string, string>> AvailablePageTemplates { get; set; }

        [MaxLength(Lengths.Name)]
        public string PageTemplate { get; set; }

        [MaxLength(Lengths.Description)]
        public string Introduction { get; set; }

        [MaxLength(Lengths.HtmlDescription)]
        public string HtmlHead { get; set; }

        [MaxLength(Lengths.HtmlDescription)]
        public string HtmlFooter { get; set; }

        [MaxLength(Lengths.LongDescription)]
        public string MainLinks { get; set; }

        [MaxLength(Lengths.Description)]
        public string AkismetApiKey { get; set; }

        [MaxLength(Lengths.LongName)]
        public string SearchAuthor { get; set; }

        [MaxLength(Lengths.Description)]
        public string SearchDescription { get; set; }

        [MaxLength(Lengths.LongDescription)]
        public string SearchKeywords { get; set; }

        [MaxLength(Lengths.LongDescription)]
        public string SpamWords { get; set; }

        [MaxLength(Lengths.Email)]
        public string SmtpFromEmailAddress { get; set; }        
    }
}
