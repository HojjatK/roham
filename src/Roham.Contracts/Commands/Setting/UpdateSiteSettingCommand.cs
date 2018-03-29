namespace Roham.Contracts.Commands.Setting
{
    public class UpdateSiteSettingCommand : UpdateSettingCommand
    {
        public long SiteId { get; set; }
    }
}
