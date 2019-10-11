using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using System.Web.Http;
using Autodesk.AutoCAD.ApplicationServices;
using Autodesk.AutoCAD.DatabaseServices;

namespace AcadHttpServerHost
{
    public class SysVariableController : AcadActionController
    {

        // GET api/values/5 
        public string Get(string varName)
        {
            try
            {
                object varValue = Application.GetSystemVariable(varName);
                return varValue.ToString();
            }
            catch
            {
                return "Invalid SYSTEM VARIABLE name: \"" + varName + "\"";
            }
        }


        public string Get()
        {
            return "Please supply SYSTEM VARIABLE name!";
        }




        public HttpResponseMessage Put([FromBody]AcadSysVar sysVar)
        {
            ActionMessage.Length = 0;
            HttpStatusCode code = HttpStatusCode.Accepted;

            if (sysVar == null)
            {
                code = HttpStatusCode.ExpectationFailed;
                ActionMessage.Append("SysVar argument is not supplied.");
            }
            else
            {
                if (!UpdateSystemVariable(sysVar))
                {
                    code = HttpStatusCode.ExpectationFailed;
                }
            }

            if (code != HttpStatusCode.Accepted)
                return Request.CreateErrorResponse(
                    code, ActionMessage.ToString());
            else
                return Request.CreateResponse<string>(
                    code, ActionMessage.ToString());
        }





        private bool UpdateSystemVariable(AcadSysVar sysVar)
        {
            try
            {
                Database db = HostApplicationServices.WorkingDatabase;
                Document doc = Application.DocumentManager.GetDocument(db);
                using (DocumentLock l = doc.LockDocument())
                {
                    Application.SetSystemVariable(sysVar.Name, sysVar.Value);
                }
                ActionMessage.Append(
                    "System variable \"" + sysVar.Name +
                    "\" is updated successfully.");
                return true;
            }
            catch (System.Exception ex)
            {
                ActionMessage.Append(
                    "Setting system variable \"" + sysVar.Name +
                    "\" failed:\n" + ex.Message);
                return false;
            }
        }
    }
}
