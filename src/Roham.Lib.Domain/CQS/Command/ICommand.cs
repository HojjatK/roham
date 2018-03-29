using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace Roham.Lib.Domain.CQS.Command
{
    public interface ICommand
    {
        bool TryValidate(out List<ValidationResult> errors);
        bool TryValidate(out string errorMessage);
    }

    public interface ISecureCommand : ICommand
    {
        Guid TokenId { get; set; }        
    }
}
