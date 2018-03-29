namespace Roham.Contracts.Dtos
{
    public class UserPostPermissionDto
    {
        public long UserId { get; set; }
        public long PostId { get; set; }
        public string Title { get; set; }
        public string Description { get; set; }

        public bool View { get; set; }
        public bool Create { get; set; }
        public bool Update { get; set; }
        public bool Delete { get; set; }
    }
}
