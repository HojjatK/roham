using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Roham.Web.ViewModels
{
    public class PostViewModel
    {
        public PostViewModel()
        {
            Sharing = true;
        }

        public bool Sharing { get; set; }

        public bool Disqus { get; set; }

        public string Uri { get; set; }

        public string Title { get; set; }

        public DateTime? DisplayDate { get; set;}

        public string Tags { get; set; }

        public int CommentsCount { get; set; }

        public string HtmlContent { get; set; }
    }
}