using System.Linq;

namespace System.Reflection
{
    public class AssemblyInfo
    {
        public AssemblyInfo(Assembly assembly)
        {
            if (assembly == null)
            {
                throw new NullReferenceException("assembly");
            }
            Title = GetAttributeValue<AssemblyTitleAttribute>(assembly, a => a.Title);
            Description = GetAttributeValue<AssemblyDescriptionAttribute>(assembly, a => a.Description);
            Product = GetAttributeValue<AssemblyProductAttribute>(assembly, a => a.Product);
            Copyright = GetAttributeValue<AssemblyCopyrightAttribute>(assembly, a => a.Copyright);
            Company = GetAttributeValue<AssemblyCompanyAttribute>(assembly, a => a.Company);
            Version = assembly.GetName().Version != null ? assembly.GetName().Version.ToString() : "1.0.0.0";
        }

        public string Title { get; }
        public string Description { get; }
        public string Product { get; }
        public string Copyright { get; }
        public string Company { get; }
        public string Version { get; }

        private string GetAttributeValue<TAttr>(Assembly assembly, Func<TAttr, string> resolveFunc) where TAttr : Attribute
        {
            var attributes = assembly.GetCustomAttributes(typeof(TAttr)).ToList();
            if (attributes.Count > 0)
            {
                return resolveFunc((TAttr)attributes[0]);
            }
            return "";
        }
    }
}
