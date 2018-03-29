using System;
using System.Collections.Generic;
using System.Linq;
using System.ComponentModel.DataAnnotations;
using Roham.Lib.Validation;

namespace Roham.Lib.Domain.CQS.Command
{
    public abstract class AbstractCommand : ICommand
    {
        public bool TryValidate(out List<ValidationResult> errors)
        {
            return ValidatorUtil.TryValidate(this, out errors);
        }

        public bool TryValidate(out string errorMessage)
        {
            List<ValidationResult> errors;
            bool isValid = TryValidate(out errors);
            errorMessage = string.Join(Environment.NewLine, errors.Select(e => e.ErrorMessage));
            return isValid;
        }
    }
}
