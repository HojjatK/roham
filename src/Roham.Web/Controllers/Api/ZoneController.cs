using System;
using System.Collections.Generic;
using System.Web.Http;
using Roham.Lib.Domain.CQS.Command;
using Roham.Lib.Domain.CQS.Query;
using Roham.Contracts.Commands.Zone;
using Roham.Contracts.Dtos;
using Roham.Contracts.Queries;
using Roham.Domain.Entities.Sites;
using Roham.Domain.Entities.Security;

namespace Roham.Web.Controllers.Api
{   
    [RoutePrefix("api/site/{siteId:long}/zone")]
    [Authorize(Roles = SecurityRoleNames.SysAdmin_Admin)]
    public class ZoneController : ApiControllerBase
    {
        public ZoneController(
            IQueryExecutor queryExecutor,
            ICommandDispatcher commandDispatcher) : base(queryExecutor, commandDispatcher) { }

        [HttpGet]
        [Route("")]
        public List<ZoneDto> GetZones(long siteId)
        {
            return QueryExecutor.Execute(new FindZonesBySiteIdQuery(siteId));
        }

        [HttpGet]
        [Route("{id:long}")]
        public ZoneDto GetZone(long siteId, long id)
        {
            var zoneDto = QueryExecutor.Execute(new FindByIdQuery<ZoneDto, Zone>(id));
            if (zoneDto.SiteId != siteId)
            {
                throw new ArgumentException($"Zone with id: {id}, fetched site id is invalid");
            }
            return zoneDto;
        }

        [HttpPost]
        [Route("")]
        public ResultDto NewZone(ZoneDto zoneDto)
        {
            return Result(() =>
            {
                var command = new AddZoneCommand
                {
                    SiteId = zoneDto.SiteId,
                    Title = zoneDto.Title,
                    Name = zoneDto.Name,
                    ZoneType = zoneDto.ZoneType,
                    IsActive = zoneDto.IsActive,
                    IsPublic = zoneDto.IsPublic,
                    Description = zoneDto.Description,
                };
                CommandDispatcher.Send(command);
            });
        }

        [HttpPut]
        [Route("{id}")]
        public ResultDto UpdateZone(long siteId, long id, ZoneDto zoneDto)
        {
            return Result(() =>
            {
                var command = new UpdateZoneCommand
                {
                    SiteId = siteId,
                    ZoneId = id,
                    Title = zoneDto.Title,
                    Name = zoneDto.Name,
                    IsActive = zoneDto.IsActive,
                    IsPublic = zoneDto.IsPublic,
                    Description = zoneDto.Description,
                };
                CommandDispatcher.Send(command);
            });
        }

        [HttpDelete]
        [Route("{id}")]
        public ResultDto DeleteZone(long siteId, long id)
        {
            return Result(() =>
            {
                // TODO: check siteid, and id matche (zone belongs to site)
                var command = new DeleteZoneCommand
                {
                    ZoneId = id,
                };
                CommandDispatcher.Send(command);
            });
        }
    }
}
