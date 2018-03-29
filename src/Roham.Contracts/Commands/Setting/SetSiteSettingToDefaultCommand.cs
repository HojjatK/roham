namespace Roham.Contracts.Commands.Setting
{
    public class SetSiteSettingToDefaultCommand : SetSettingToDefaultCommand
    {
        public long SiteId { get; set; }
    }
}
