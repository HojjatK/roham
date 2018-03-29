using System;
using NUnit.Framework;

namespace Roham
{
    public abstract class UnitTestFixture
    {   
    }

    public abstract class UnitTestFixture<TSubject> : UnitTestFixture
    {
        private Func<TSubject> _subjectFactory { get; }

        protected UnitTestFixture(Func<TSubject> subjectFactory)
        {
            _subjectFactory = subjectFactory;
        }

        protected TSubject CreateSubject()
        {
            return _subjectFactory();
        }
    }
}
