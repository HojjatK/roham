using System.ComponentModel.DataAnnotations;
using System.Collections.Generic;
using Roham.Lib.Strings;
using Roham.Lib.Domain;
using Roham.Lib.Domain.DataAnnotation;
using Roham.Domain.Entities.Entries;
using Roham.Domain.Entities.Sites;

namespace Roham.Domain.Entities.Snippets
{
    /// <summary>
    /// Code snippet entry domain entity.
    /// </summary>
    public class Snippet : Entry<SnippetRevision, SnippetComment, SnippetRating, SnippetLink>
    {
        [Required(AllowEmptyStrings = false, ErrorMessage = "Please provide a name.")]
        [Unique("UQ_Snippet_Name")]
        [MaxLength(Lengths.Name)]
        public virtual PageName Name { get; set; }

        private ICollection<Pingback> _pingbacks;
        public virtual ICollection<Pingback> Pingbacks
        {
            get { return this.LazySet(ref _pingbacks); }
            protected set { _pingbacks = value.AsSet(); }
        }

        public override SnippetRevision Revise()
        {
            var revision = base.Revise();
            revision.Snippet = this;            
            return revision;
        }

        public override SnippetComment Comment()
        {
            var comment = base.Comment();
            comment.Snippet = this;
            return comment;
        }

        public override SnippetRating Rate()
        {
            var rating = base.Rate();
            rating.Snippet = this;
            return rating;
        }
    }
}
