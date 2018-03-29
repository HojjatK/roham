using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

namespace Roham.Domain.Settings
{
    public class SiteSettings : ISettings
    {
        public long? SiteId { get; set; }

        [DisplayName("Akismet API Key")]
        [StringLength(30)]
        [DefaultValue("37726b9324fe")]
        [Description("If you have your own API key for Akismet, place it here.")]
        [SettingKey("akismet-api-key")]
        public string AkismetApiKey { get; set; }

        [DisplayName("Introduction")]
        [StringLength(5000)]
        [DataType("Markdown")]
        [Description("The welcome text that is shown on the home page. You can use markdown.")]
        [DefaultValue("Welcome to your site. You can <a href=\"/login\">login</a> and edit this message in the administration section.")]
        [SettingKey("site-introduction")]
        public string Introduction { get; set; }

        [DisplayName("Main Links")]
        [StringLength(5000)]
        [DataType("HTML")]
        [Description("A list of links shown at the top of each page. Use HTML for this.")]
        [DefaultValue("<li><a href=\"/about\">About</a></li>")]
        [SettingKey("main-links")]
        public string MainLinks { get; set; }

        [DisplayName("Footer")]
        [StringLength(3000)]
        [DataType("HTML")]
        [DefaultValue("<p>Powered by <a href=\"#\">Roham</a>, the blog engine of real developers.</p>")]
        [Description("This will appear at the bottom of the page - use it to add copyright information, links to any web hosts, people or technologies that helped you to build the site, and so on.")]
        [SettingKey("footer")]
        public string Footer { get; set; }

        [DisplayName("Default Page")]
        [Description("When users visit the root (/) of your site, it will be equivalent to visiting the page you specify here.")]
        [DefaultValue("blog")]
        [StringLength(100)]
        [SettingKey("default-page")]
        public string DefaultPage { get; set; }

        [DisplayName("Author")]
        [StringLength(100)]
        [Description("Your name. Rendered as a meta tag.")]
        [DefaultValue("Daffy Duck")]
        [SettingKey("search-author")]
        public string SearchAuthor { get; set; }

        [DisplayName("Meta-Description")]
        [StringLength(150)]
        [Description("The description shown to search engines in the meta description tag.")]
        [DefaultValue("My website.")]
        [SettingKey("search-description")]
        public string SearchDescription { get; set; }

        [DisplayName("Meta-Keywords")]
        [StringLength(100)]
        [Description("Keywords shown to search engines in the meta-keywords tag (comma-separated text).")]
        [DefaultValue(".net, c#, test")]
        [SettingKey("search-keywords")]
        public string SearchKeywords { get; set; }

        [DisplayName("Spam blacklist")]
        [StringLength(500)]
        [DefaultValue("casino")]
        [Description("Comments with these words (case-insensitive) will automatically be marked as spam, in addition to Akismet. Seperate using spaces or newlines.")]
        [SettingKey("spam-words")]
        public string SpamWords { get; set; }

        [DisplayName("Disable comments after")]
        [DefaultValue(0)]
        [Description("If a post is older than this many days, comments will be disabled. Use 0 to allow comments indefinitely.")]
        [SettingKey("spam-comment-disable")]
        public int DisableCommentsOlderThan { get; set; }

        [DisplayName("HTML Head")]
        [StringLength(2000)]
        [DefaultValue("")]
        [Description("Custom HTML that will appear just before the &lt;/head&gt; tag")]
        [SettingKey("html-head")]
        public string HtmlHead { get; set; }

        [DisplayName("HTML Footer")]
        [StringLength(2000)]
        [DefaultValue("")]
        [Description("Custom HTML that will appear just before the &lt;/body&gt; tag")]
        [SettingKey("html-foot")]
        public string HtmlFooter { get; set; }

        [DisplayName("Theme")]
        [StringLength(100)]
        [DefaultValue("Official")]
        [Description("The theme which will be used for this website.")]
        [SettingKey("site-theme")]
        public string SiteTheme { get; set; }

        [DisplayName("From")]
        [StringLength(200)]
        [DefaultValue("from@your-site.com")]
        [RegularExpression("^[A-Za-z0-9._%+-]+@([A-Za-z0-9-]+\\.)+([A-Za-z0-9]{2,4}|museum)$", ErrorMessage = "Please enter a valid email address")]
        [Description("The email address from which emails will be sent.")]
        [SettingKey("smtp-from-email-address")]
        public string SmtpFromEmailAddress { get; set; }
    }
}
