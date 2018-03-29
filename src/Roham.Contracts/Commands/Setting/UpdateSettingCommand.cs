using System.ComponentModel.DataAnnotations;
using Roham.Lib.Domain.CQS.Command;

namespace Roham.Contracts.Commands.Setting
{
    public abstract class UpdateSettingCommand : AbstractCommand
    {
        [Required(AllowEmptyStrings = false)]
        public string Key { get; set; }

        public string Value { get; set; }

        public override string ToString()
        {
            return $"Key:{Key}, Value:{Value}";
        }
    }
}
