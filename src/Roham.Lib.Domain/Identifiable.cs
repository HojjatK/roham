namespace Roham.Lib.Domain
{
    public abstract class Identifiable : Entity<long>
    {
        public override string ToString()
        {
            return $"{GetType().Name} (Id: {Id})";
        }
    }
}
