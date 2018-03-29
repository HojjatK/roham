using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;

namespace Roham.Lib.Validation
{
    public static class ValidatorUtil
    {
        public static void Validate<T>(T instance) where T : class
        {
            var ctx = new ValidationContext(instance, null, null);
            Validator.ValidateObject(instance, ctx);
        }

        public static void ValidateAll<T>(T instance) where T : class
        {
            var ctx = new ValidationContext(instance, null, null);
            var validationResults = new List<ValidationResult>();
            bool isValid = Validator.TryValidateObject(instance, ctx, validationResults, true);
            if (!isValid)
            {
                throw new AggregateException(validationResults.Select((e) => new ValidationException(e.ErrorMessage)));
            }
        }

        public static bool TryValidate<T>(T instance, out List<ValidationResult> validationResults)
        {
            var ctx = new ValidationContext(instance, null, null);
            validationResults = new List<ValidationResult>();
            return Validator.TryValidateObject(instance, ctx, validationResults, true);
        }
    }
}
