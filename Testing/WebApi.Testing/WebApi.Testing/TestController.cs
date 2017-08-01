using System;
using System.Collections.Concurrent;
using System.Diagnostics;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using System.Web.Http;

namespace WebApi.Testing
{
    [RoutePrefix("v1/test")]
    public class TestController : ApiController
    {
        private static readonly ConcurrentDictionary<string, Data[]> DataHolder = new ConcurrentDictionary<string, Data[]>();

        [Route("getdata")]
        [HttpGet]
        public Data[] Retrieve(string un, string cn)
        {
            if (DataHolder.TryGetValue($"{un}_{cn}", out Data[] data))
            {
                return data;
            }
            throw new HttpResponseException(HttpStatusCode.NotFound);
        }

        [Route("mix")]
        [HttpPost]
        public HttpResponseMessage Posting([FromUri] string uname, [FromUri] string cname, [FromBody] Data[] data)
        {
            Debug.Assert(ReferenceEquals(DataHolder.AddOrUpdate($"{uname}_{cname}", data, (k, d) => data), data),
                "error");
            var responseData = new Response
            {
                Location = new Uri($"http://localhost:9001/v1/test/getdata/?un={uname}&cn={cname}", UriKind.Absolute),
                CName = cname,
                Count = data.Length,
                Uname = uname
            };
            var resp = Request.CreateResponse(HttpStatusCode.Created, responseData, Request.GetConfiguration());
            resp.Headers.Location = new Uri($"http://localhost:9001/v1/test/getdata/?un={uname}&cn={cname}", UriKind.Absolute);
            return resp;
        }
    }

    public class Data
    {
        public int Id { get; set; }
        public string Name { get; set; }
    }

    public class Response
    {
        public Uri Location { get; set; }
        public string Uname { get; set; }
        public string CName { get; set; }
        public int Count { get; set; }
    }
}