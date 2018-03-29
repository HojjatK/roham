using System.Collections.Generic;
using System.Linq;

namespace System
{
    public static class EnumExtensions
    {
        public static IEnumerable<Enum> GetFlags(this Enum value)
        {
            return Enum.GetValues(value.GetType()).Cast<Enum>().Where(value.HasFlag);
        }

        public static IEnumerable<Enum> GetPowerOfTwoFlags(Type enumType, bool skipZeroValue = true)
        {
            ulong flag = 1L;
            foreach (var value in Enum.GetValues(enumType).Cast<Enum>())
            {
                ulong bits = Convert.ToUInt64(value);
                if (bits == 0L && skipZeroValue)
                {
                    continue;
                }

                while (flag < bits)
                {
                    flag <<= 1;
                }

                if (flag == bits || bits == 0L)
                {
                    yield return value;
                }
            }
        }
    }
}
