using System;
using System.Collections.Generic;
using Roham.Contracts.Dtos;
using Roham.Lib.Domain.CQS.Query;

namespace Roham.Contracts.Queries
{
    public class FindUsersByRoleQuery : IQuery<List<UserDto>>
    {
        public string QueryString => $"roleType:{RoleType}";

        public FindUsersByRoleQuery(string roleType)
        {
            this.RoleType = roleType;
        }

        public string RoleType { get; }

        public override string ToString()
        {
            return $"Find Users [{QueryString}]";
        }
    }
}