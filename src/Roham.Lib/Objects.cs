namespace System
{
    public static class Objects
    {
        public static void Requires<TException>(bool predicate, Func<TException> exFunc = null)
            where TException : Exception, new()
        {
            if (!predicate)
            {
                if (exFunc != null)
                {
                    throw exFunc();
                }
                else
                {
                    throw new TException();
                }
            }
        }
    }
}
