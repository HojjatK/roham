namespace System.Threading
{
    public static class ReaderWriterLockSlimExtensions
    {
        public struct ReadLockScope : IDisposable
        {
            private readonly ReaderWriterLockSlim rwLock;

            public ReadLockScope(ReaderWriterLockSlim rwLock)
            {
                this.rwLock = rwLock;
                this.rwLock.EnterReadLock();
            }

            public void Dispose()
            {
                rwLock.ExitReadLock();
            }
        }

        public struct WriteLockScope : IDisposable
        {
            private readonly ReaderWriterLockSlim rwLock;

            public WriteLockScope(ReaderWriterLockSlim rwLock)
            {
                this.rwLock = rwLock;
                this.rwLock.EnterWriteLock();
            }

            public void Dispose()
            {
                rwLock.ExitWriteLock();
            }
        }

        public static ReadLockScope ReadScope(this ReaderWriterLockSlim rwLock)
        {
            return new ReadLockScope(rwLock);
        }

        public static WriteLockScope WriteScope(this ReaderWriterLockSlim rwLock)
        {
            return new WriteLockScope(rwLock);
        }
    }
}