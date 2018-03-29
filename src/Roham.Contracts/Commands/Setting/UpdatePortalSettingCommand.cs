namespace Roham.Contracts.Commands.Setting
{
    public class UpdatePortalSettingCommand : UpdateSettingCommand
    {
        public string SettingKey { get; set; }
        public string SettingValue { get; set; }
    }
}
