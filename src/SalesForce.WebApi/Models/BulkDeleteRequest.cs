using System.Collections.Generic;
using Newtonsoft.Json;

namespace SalesForce.WebApi.Models
{
    public class BulkDeleteRequest
    {
        public bool AllOrNone { get; set; }
        public List<string> RecordId { get; set; }
    }
}