using System.Collections.Generic;
using System.Net;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace SalesForce.WebApi.Models
{
    public class CompositeResponse
    {
        public HttpStatusCode HttpStatusCode { get; set; }
        public string ReferenceId { get; set; }
        public Dictionary<string, string> HttpHeaders { get; set; }
        public dynamic Body { get; set; }
        public SucessDetail Sucess
        {
            get
            {
                try
                {
                    if (!IsSuccessStatusCode || (int)HttpStatusCode != 201)
                        return null;
                    return Body.ToObject<SucessDetail>();
                }
                catch { return null; }
            }
        }
        public string Id
        {
            get
            {
                try
                {
                    if (!IsSuccessStatusCode || (int)HttpStatusCode != 201)
                        return null;
                    return Body.ToObject<SucessDetail>().Id;
                }
                catch { return null; }
            }
        }
        public List<ErroDetail> Errors
        {
            get
            {
                try
                {
                    if (IsSuccessStatusCode)
                        return null;

                    return Body.ToObject<List<ErroDetail>>();
                }
                catch { return null; }
            }
        }

        public bool IsSuccessStatusCode
        {
            get { return ((int)HttpStatusCode >= 200) && ((int)HttpStatusCode <= 299); }
        }
    }
}