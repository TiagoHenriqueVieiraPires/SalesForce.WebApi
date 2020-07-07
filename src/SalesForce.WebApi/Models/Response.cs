using System.Collections.Generic;
using System.Net;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace SalesForce.WebApi.Models
{
    public class Response<TSuccess>
    {
        public virtual HttpStatusCode HttpStatusCode { get; set; }
        public virtual dynamic Body { get; set; }
        public TSuccess Sucess
        {
            get
            {
                try
                {
                    if (!IsSuccessStatusCode)
                        return default(TSuccess);
                    return Body.ToObject<TSuccess>();
                }
                catch { return default(TSuccess); }
            }
        }
        public string Id
        {
            get
            {
                try
                {
                    if (!IsSuccessStatusCode)
                        return null;
                    return Body.ToObject<TSuccess>().Id;
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