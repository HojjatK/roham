using System.Collections.Generic;

namespace Roham.Contracts.Dtos
{
    public class ResultDto
    {
        public ResultDto()
        {
            ErrorMessages = new List<string>();
        }

        public bool Succeed { get; set; }
        public List<string> ErrorMessages { get; }
    }
}