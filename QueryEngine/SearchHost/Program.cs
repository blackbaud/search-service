using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web.Http;
using System.Web.Http.SelfHost;
using DCMAPI;

namespace SearchHost
{
    class Program
    {
        static readonly Uri _baseAddress = new Uri("http://localhost:60064");
        static void Main(string[] args)
        {
            var config = new HttpSelfHostConfiguration(_baseAddress);
            config.MessageHandlers.Add(new CorsHandler());
            config.Routes.MapHttpRoute(name: "SearchApi", routeTemplate: "api/{controller}/{id}", defaults: new {id=RouteParameter.Optional});
            var server = new HttpSelfHostServer(config);
            server.OpenAsync().Wait();
            Console.WriteLine("Web API Self hosted on " + _baseAddress + " Hit ENTER to exit...");
            Console.ReadLine();
            server.CloseAsync().Wait();

        }
    }
}
