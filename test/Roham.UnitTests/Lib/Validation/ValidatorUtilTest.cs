using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using NUnit.Framework;

namespace Roham.Lib.Validation
{
    public class ValidatorUtilTest
    {
        [TestFixture]
        [Category("Validator")]
        public class GivenValidObject : UnitTestFixture
        {
            private ValidatorTestClass subject = new ValidatorTestClass
            {
                GivenName = "Max",
                Surname = "Foo",
                Age = 40,
                Email = "max@foo.bar.com"
            };

            [Test]
            public void WhenValidateIsCalled_Then_NoErrorOccurs()
            {
                ValidatorUtil.Validate(subject);
            }

            [Test]
            public void WhenValidateAllIsCalled_Then_NoErrorOccurs()
            {
                ValidatorUtil.ValidateAll(subject);
            }

            [Test]
            public void WhenTryValidateIsCalled_Then_ResultIsTrue()
            {
                List<ValidationResult> errors;
                bool isValid = ValidatorUtil.TryValidate(subject, out errors);

                Assert.IsTrue(isValid, "Objest is not valid");
            }
        }

        [TestFixture]
        [Category("Validator")]
        public class GivenInvalidObject : UnitTestFixture
        {
            private ValidatorTestClass subject = new ValidatorTestClass
            {
                GivenName = "M", // invalid: min len
                Surname = null, // inavalid: required
                Age = 211, // invalid: max len
                Email = "max!foo.bar.com" // invalid email
            };

            [Test]
            public void WhenValidateIsCalled_Then_ValidateExceptionIsThrown()
            {
                Assert.Throws<ValidationException>(() => ValidatorUtil.Validate(subject));
            }

            [Test]
            public void WhenValidateAllIsCalled_Then_AggregateExceptionContainingAllValidateExceptionsIsThrown()
            {
                Assert.Throws<AggregateException>(() => ValidatorUtil.ValidateAll(subject));
            }

            [Test]
            public void WhenTryValidateIsCalled_Then_ResultIsFalseAndValidationErrorsAreReturned()
            {
                List<ValidationResult> errors;
                bool isValid = ValidatorUtil.TryValidate(subject, out errors);

                Assert.IsFalse(isValid);
                Assert.AreEqual(4, errors.Count); // four errors
            }
        }

        internal class ValidatorTestClass
        {
            [Required(AllowEmptyStrings = false)]
            [MinLength(2)]
            [MaxLength(20)]
            public string GivenName { get; set; }

            [Required(AllowEmptyStrings = false)]
            [MinLength(2)]
            [MaxLength(30)]
            public string Surname { get; set; }

            [System.ComponentModel.DataAnnotations.Range(0, 99)]
            public int Age { get; set; }

            [EmailAddress]
            public string Email { get; set; }
        }
    }
}
