using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Autodesk.AutoCAD.ApplicationServices;
using Autodesk.AutoCAD.EditorInput;
using Autodesk.AutoCAD.Runtime;
using Microsoft.Owin.Hosting;
using System.Net.Http;

[assembly: ExtensionApplication(
     typeof(AcadHttpServerHost.HttpServerHostInitializer))]

namespace AcadHttpServerHost
{
    public class HttpServerHostInitializer : IExtensionApplication
    {

        static HttpClient _httpServer = null;
        string baseAddress = "http://localhost:9000/";


        #region IExtensionApplication Members
        public void Initialize()
        {
            Document dwg = Application.DocumentManager.MdiActiveDocument;
            Editor ed = dwg.Editor;

            try
            {
                ed.WriteMessage("\nInitializing HTTP server hosting...");


                WebApp.Start<Startup>(url: baseAddress);

                // Create HttpCient and make a request to api/values 
                _httpServer = new HttpClient();
                var response = _httpServer.GetAsync(baseAddress + "api/values").Result;


                ed.WriteMessage("completed.");
                Autodesk.AutoCAD.Internal.Utils.PostCommandPrompt();
            }
            catch (System.Exception ex)
            {
                ed.WriteMessage("failed:\n");
                ed.WriteMessage(ex.Message);
                Autodesk.AutoCAD.Internal.Utils.PostCommandPrompt();
            }
        }

        public void Terminate()
        {
            if (_httpServer != null)
            {
                _httpServer.Dispose();
            }
        }


        #endregion

        /*
        #region private methods

        private HttpSelfHostServer CreateHttpSelfHostServer(string baseUrl)
        {
            HttpSelfHostConfiguration config = ConfigurateHost(baseUrl);
            HttpSelfHostServer server = new HttpSelfHostServer(config);
            return server;
        }

        private HttpSelfHostConfiguration ConfigurateHost(string baseUrl)
        {
            HttpSelfHostConfiguration config =
                new HttpSelfHostConfiguration(baseUrl);

            config.Routes.MapHttpRoute(
                name: "Default Api",
                routeTemplate: "api/{controller}/{id}",
                defaults: new
                {
                    id = RouteParameter.Optional
                }
                );

            return config;
        }

        #endregion
    */
    }
}
