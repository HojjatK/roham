namespace Roham.Contracts.Dtos
{
    public class RoleFunctionDto 
    {
        public string Uid { get; set; }

        public long Id { get; set; }

        public long RoleId { get; set; }

        public string Name { get; set; }

        public string DisplayName { get; set; }

        public string Description { get; set; }        

        public string RoleName { get; set; }

        public RoleFunctionDto Parent { get; set; }
    }
}
