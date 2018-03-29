namespace Roham.Domain.Entities.Posts
{
    public class CategoryMapping : IdentifiableSubclassMap<Category>
    {
        public CategoryMapping()
        {
            Map(x => x.IsPrivate);
            Map(x => x.Description);

            References(x => x.Parent).Column(MappingNames.ParentId);
        }
    }
}
