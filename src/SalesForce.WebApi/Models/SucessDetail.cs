namespace SalesForce.WebApi.Models
{
    public class SucessDetail
    {
        public string Id { get; set; }
        public bool Success { get; set; }
        public dynamic Errors { get; set; }
    }
}