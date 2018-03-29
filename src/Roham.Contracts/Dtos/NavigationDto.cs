using System.Collections.Generic;

namespace Roham.Contracts.Dtos
{
    public class NavigationDto 
    {
        public string Title { get; set; }

        private List<NavItemDto> navItems = new List<NavItemDto>();
        public List<NavItemDto> NavItems
        {
            get { return navItems; }
            set { navItems = value; }
        }
    }

    public class NavItemDto
    {
        private List<NavItemDto> subItems = new List<NavItemDto>();        

        public int Key { get; set; }
        public string Name { get; set; }
        public string Title { get; set; }        
        public int Level { get; set; }
        public string Parent { get; set; }
        public bool IsAccessible { get; set; }

        public List<NavItemDto> SubItems
        {
            get { return subItems; }
            set { subItems = value; }
        }
    }
}
