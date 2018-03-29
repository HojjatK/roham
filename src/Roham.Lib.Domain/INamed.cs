using Roham.Lib.Strings;

namespace Roham.Lib.Domain
{
    public interface INamed
    {
        PageName Name { get; }
    }

    public interface IUserNamed
    {
        string UserName { get; }
    }
}
