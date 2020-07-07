using System;

namespace SalesForce.WebApi.Models
{
    public class ChatterRequest
    {
        public String Text { get; set; }
        public String SubjectId { get; set; }
    }
}