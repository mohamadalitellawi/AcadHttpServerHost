using AcadHttpServerHost;
using AcadHttpServerHost.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;

namespace MAT.CAD.WebApi
{
    class Program
    {
        HttpClient _client = null;

        public Program()
        {
            _client = new HttpClient();
            _client.BaseAddress = new Uri("http://localhost:9000");
            _client.DefaultRequestHeaders.Accept.Add(
                   new System.Net.Http.Headers.MediaTypeWithQualityHeaderValue(
                       "application/json"));
        }
        static void Main(string[] args)
        {

            Program App = new Program();
            try
            {
                App.GetSystemVariable("CTAB");
                App.SetSystemVariable(new AcadSysVar { Name = "CLAYER", Value = "0" });

                App.DrawCircle(200, 0, 0, 0);
            }
            catch (Exception ex)
            {
                Console.WriteLine("Error Happend: " + ex.Message + "\n" + ex.InnerException.Message);
            }
            finally
            {
                Console.WriteLine("Press Any Key to End...");
                Console.ReadKey();
            }
        }

        private void GetSystemVariable(string sysVarName)
        {

            HttpResponseMessage resMsg = _client.GetAsync(
                "api/SysVariable/?varName=" + sysVarName).Result;
            resMsg.EnsureSuccessStatusCode();

            string txt = resMsg.Content.ReadAsAsync<string>().Result;
            Console.WriteLine(txt);
        }

        private void SetSystemVariable(AcadSysVar sysVar)
        {
            HttpResponseMessage resMsg =
                _client.PutAsJsonAsync("api/SysVariable", sysVar).Result;

            string txt = resMsg.Content.ReadAsAsync<string>().Result;
            Console.WriteLine(txt); 

        }

        private void DrawCircle(double radius , double x,double y, double z)
        {

            CircleArgs circleArgs = new CircleArgs
            {
                X = x,
                Y = y,
                Z = z,
                Radius = radius
            };

            HttpResponseMessage resMsg = _client.PutAsJsonAsync("api/Draw", circleArgs).Result;
            string txt = resMsg.Content.ReadAsAsync<string>().Result;

            Console.WriteLine(txt);

        }
    }
}
