using System.Collections.Generic;

namespace SalesForce.WebApi.Models
{
    public class ErroDetail
    {
        public string Message { get; set; }
        public string ErrorCode { get; set; }
        public List<string> Fields { get; set; }
    }
}