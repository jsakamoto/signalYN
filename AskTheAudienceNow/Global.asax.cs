using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Web;
using System.Web.Http;
using System.Web.Routing;

namespace AskTheAudienceNow
{
    public class WebApiApplication : System.Web.HttpApplication
    {
        protected void Application_Start()
        {
            GlobalConfiguration.Configure(WebApiConfig.Register);
            RouteConfig.RegisterRoutes(RouteTable.Routes);

            //this.Error += WebApiApplication_Error;
        }

        protected void Application_Error(object sender, EventArgs e)
        {
            var logText = new StringBuilder();
            logText.AppendLine("#exception");
            logText.AppendLine(Server.GetLastError().ToString());

            var context = HttpContext.Current;
            var request = context != null ? context.Request : null;
            if (request != null)
            {
                try
                {
                    logText
                        .AppendLine("#request")
                        .AppendLine(request.HttpMethod + " " + request.RawUrl)
                        .AppendLine(request.ServerVariables["ALL_RAW"]);
                }
                catch { }

                try
                {
                    logText.AppendLine("#server-variables");
                    foreach (var key in request.ServerVariables.AllKeys.Except("ALL_HTTP", "ALL_RAW"))
                    {
                        logText.AppendLine(key + "=" + request.ServerVariables[key]);
                    }
                }
                catch { }
            }

            try { Trace.TraceError(logText.ToString()); }
            catch { }
        }
    }
}
