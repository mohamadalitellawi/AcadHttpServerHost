using System.Collections.Generic;
using System.Text;
using System.Web.Http;

namespace AcadHttpServerHost
{
    public class AcadActionController : ApiController
    {
        private static StringBuilder _msg = new StringBuilder();
        public static StringBuilder ActionMessage
        {
            get { return _msg; }
        }
    }
}
