using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace Common.Contracts
{
    public interface IHttpClient
    {
        Tuple<object, HttpStatusCode> PostRequest(string route, object obj = null);
        Tuple<object, HttpStatusCode> GetRequest(string route);
    }
}
