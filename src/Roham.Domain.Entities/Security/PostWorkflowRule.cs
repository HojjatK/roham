using System.ComponentModel.DataAnnotations;
using Roham.Lib.Domain;
using Roham.Lib.Domain.DataAnnotation;
using Roham.Domain.Entities.Sites;

namespace Roham.Domain.Entities.Security
{
    /// <summary>
    /// Post entry's workflow rule domain entity.
    /// </summary>
    public class PostWorkflowRule : AggregateRoot
    {
        [Unique("UQ_Site_Workflow_Name")]
        [MaxLength(Lengths.Name)]
        public virtual string Name { get; set; }

        [Required]
        public virtual bool IsActive { get; set; }

        [Required]
        public virtual bool ReturnToAuthorForPublish { get; set; }

        [Required]
        public virtual Role ApproverRole { get; set; }

        public virtual Role PublisherRole { get; set; }

        [Required]
        [Unique("UQ_Site_Workflow_Name")]
        public virtual Site Site { get; set; }
    }
}
