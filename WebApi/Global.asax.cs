using Serilog;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Http;
using System.Web.Routing;
using WebApi.Middleware;

namespace WebApi
{
    public class WebApiApplication : System.Web.HttpApplication
    {
        protected void Application_Start()
        {
            GlobalConfiguration.Configure(WebApiConfig.Register);
        }

        protected void Application_Error(object sender, EventArgs e)
        {
            var exception = Server.GetLastError();
            Response.Clear();

            Server.ClearError();

            Response.ContentType = "application/json";
            var (statusCode, err, trackingNumber) = GlobalHandleExceptionFilter.createErrorResponseJson(exception);
            Log.Error(exception, $"Exception is occured Tracking Number: {trackingNumber}");
            Log.CloseAndFlush();

            Response.StatusCode = (int)statusCode;
            var errorResponse = new
            {
                err.Title,
                err.TrackingNumber,
                err.Description
            };

            var jsonError = new System.Web.Script.Serialization.JavaScriptSerializer().Serialize(errorResponse);
            Response.Write(jsonError);
            Response.End();
            
        }

        protected void Application_End()
        {
            Log.CloseAndFlush();
        }
    }
}
