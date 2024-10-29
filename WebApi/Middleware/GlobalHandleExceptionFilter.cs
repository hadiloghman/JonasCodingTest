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

namespace WebApi.Middleware
{
    public class GlobalHandleExceptionFilter : IExceptionFilter
    {
        private string _view;
        private string _master;


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
            var trackingNumber = "";
            var exception = filterContext.Exception;
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
            this._logger.Error(filterContext.Exception, $"Exception is occured Tracking Number: {trackingNumber}");



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


    }
}