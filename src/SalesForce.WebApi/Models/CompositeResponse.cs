using System.Collections.Generic;

namespace SalesForce.WebApi.Models
{
    public class CompositeResponse : Response<SucessDetail>
    {
        public string ReferenceId { get; set; }
        public Dictionary<string, string> HttpHeaders { get; set; }
    }
}