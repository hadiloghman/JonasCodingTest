using Serilog;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Net;
using System.Threading;
using System.Threading.Tasks;
using System.Web;
using System.Web.Http.ExceptionHandling;
using System.Web.Http.Filters;
using System.Web.UI.WebControls;
using DataAccessLayer.Model.Models;
using Newtonsoft.Json;
using Ninject.Activation;

namespace WebApi.Middleware
{
    public class GlobalHandleExceptionFilter : IExceptionFilter
    {
        ILogger _logger;
        public GlobalHandleExceptionFilter(ILogger logger)
        {
            this._logger = logger;
        }
        public bool AllowMultiple => true;

        public async Task ExecuteExceptionFilterAsync(
            HttpActionExecutedContext filterContext,
            CancellationToken cancellationToken)
        {
            var (statusCode, err, trackingNumber) = createErrorResponseJson(filterContext.Exception);
            _logger.Error(err, $"Exception is occured Tracking Number: {trackingNumber}");
            
            filterContext.Response = filterContext.Request.CreateResponse(
                statusCode,
                new
                {
                    err.Title,
                    err.TrackingNumber,
                    err.Description
                },
                "application/json"
            );

            await Task.CompletedTask;
        }

        public static (HttpStatusCode, ErrorGeneral, string) createErrorResponseJson(Exception exception)
        {
            string trackingNumber;
            HttpStatusCode statusCode;
            ErrorGeneral err;
            if (exception is ErrorHandled)
            {
                statusCode = HttpStatusCode.BadRequest;
                err = (ErrorGeneral)exception;
            }
            else
            {
                statusCode = HttpStatusCode.InternalServerError;
                err = new ErrorGeneral
                {
                    Title = "An Exception has been occured",
                    Description = "Contact support team"
                };
            }
            trackingNumber = err.TrackingNumber;

            return (statusCode, err, trackingNumber);
        }
    }
}