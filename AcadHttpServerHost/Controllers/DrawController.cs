using AcadHttpServerHost.Models;
using Autodesk.AutoCAD.ApplicationServices;
using Autodesk.AutoCAD.DatabaseServices;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;

namespace AcadHttpServerHost.Controllers
{
    public class DrawController : AcadActionController
    {
        public HttpResponseMessage Put( CircleArgs circleArgs)
        {
            ActionMessage.Length = 0;
            HttpStatusCode code = HttpStatusCode.Created;

            if (circleArgs == null)
            {
                code = HttpStatusCode.ExpectationFailed;
                ActionMessage.Append("Circleargs argument is not supplied.");
            }
            else
            {
                if (!DrawCircle(circleArgs))
                {
                    code = HttpStatusCode.ExpectationFailed;
                }
            }

            if (code != HttpStatusCode.Created)
            {
                return Request.CreateErrorResponse(code, ActionMessage.ToString());
            }
            else
            {
                return Request.CreateResponse<string>(code, ActionMessage.ToString());
            }

        }

        private bool DrawCircle(CircleArgs circleArgs)
        {
            Database db = HostApplicationServices.WorkingDatabase;
            Document doc = Application.DocumentManager.GetDocument(db);

            using (DocumentLock l = doc.LockDocument())
            {
                try
                {
                    using (Transaction tr = db.TransactionManager.StartTransaction())
                    {
                        BlockTableRecord model = tr.GetObject(SymbolUtilityServices.GetBlockModelSpaceId(db), OpenMode.ForWrite) as BlockTableRecord;

                        Circle c = new Circle();
                        c.Center = new Autodesk.AutoCAD.Geometry.Point3d(circleArgs.X, circleArgs.Y, circleArgs.Z);
                        c.Radius = circleArgs.Radius;
                        c.SetDatabaseDefaults(db);

                        model.AppendEntity(c);
                        tr.AddNewlyCreatedDBObject(c, true);

                        tr.Commit();
                    }

                    ActionMessage.Append("Cicle has been added into drawing successfully.");
                    return true;
                }
                catch (Exception ex)
                {       
                    ActionMessage.Append("Drawing circle failed:\n" + ex.Message + "\n" + ex.InnerException.Message);
                    return false;
                }
            }
        }
    }
}
