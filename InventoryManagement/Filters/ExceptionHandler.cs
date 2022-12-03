using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using EntityFramework.Exceptions.Common;
using InventoryManagement.Controllers;

namespace InventoryManagement.Filters
{
    public class ExceptionHandler : ExceptionFilterAttribute
    {
        private readonly ILogger<ExceptionHandler> _log;

        public ExceptionHandler(ILogger<ExceptionHandler> logger)
        {
            _log = logger ?? throw new ArgumentNullException(nameof(logger));
        }

        #region Comments
        /*
            EntityFramework.Exceptions provides the following more detailed database exceptions:
                CannotInsertNullException
                MaxLengthExceededException
                NumericOverflowException
                UniqueConstraintException

            'UseExceptionProcessor' must be set when adding the database context (AddDbContextPool) to Services in Program.cs
            If 'UseExceptionProcessor' is not set, the exceptions will all be caught as a generic DbUpdateException
        */
        #endregion
        public override Task OnExceptionAsync(ExceptionContext exContext)
        {
            base.OnExceptionAsync(exContext);

            var ex = exContext.Exception;
            var controllerName = exContext.RouteData.Values["controller"];
            var actionName = exContext.RouteData.Values["action"];
            switch (ex)
            {
                // The following exceptions are tested first as they
                // could also be captured under DbUpdateException
                case ValidationException:
                    _log.LogError("{Controller}->{Action}{Message}:\r\n{Exception}", controllerName, actionName, ex.Message, ex);
                    exContext.HttpContext.Response.Headers.Append(Constants.StatusReasonHeaderKey, "Validation Exception");
                    exContext.Result = new ConflictResult();
                    break;
                case CannotInsertNullException:
                    _log.LogError("{Controller}->{Action}{Message}:\r\n{Exception}", controllerName, actionName, ex.Message, ex);
                    exContext.HttpContext.Response.Headers.Append(Constants.StatusReasonHeaderKey, "Cannot Insert Null Exception");
                    exContext.Result = new ConflictResult();
                    break;
                case MaxLengthExceededException:
                    _log.LogError("{Controller}->{Action}{Message}:\r\n{Exception}", controllerName, actionName, ex.Message, ex);
                    exContext.HttpContext.Response.Headers.Append(Constants.StatusReasonHeaderKey, "Max Length Exceeded Exception");
                    exContext.Result = new ConflictResult();
                    break;
                case NumericOverflowException:
                    _log.LogError("{Controller}->{Action}{Message}:\r\n{Exception}", controllerName, actionName, ex.Message, ex);
                    exContext.HttpContext.Response.Headers.Append(Constants.StatusReasonHeaderKey, "Max Length Exceeded Exception");
                    exContext.Result = new ConflictResult();
                    break;
                case UniqueConstraintException:
                    _log.LogError("{Controller}->{Action}{Message}:\r\n{Exception}", controllerName, actionName, ex.Message, ex);
                    exContext.HttpContext.Response.Headers.Append(Constants.StatusReasonHeaderKey, "Unique Constraint Exception");
                    exContext.Result = new ConflictResult();
                    break;
                case DbUpdateException:
                    var isSqlExcepton = (ex.InnerException is SqlException) ? true : false;
                    if (isSqlExcepton)
                    {
                        _log.LogError("{Controller}->{Action}{SQLException}:\r\n{InnerException}", controllerName, actionName, "Sql Exception", ex.InnerException);
                    }
                    else
                    {
                        _log.LogError("{Controller}->{Action}{DbUpdateException}:\r\n{Exception}", controllerName, actionName, "Db Update Exception", ex);
                    }
                    exContext.HttpContext.Response.Headers.Append(Constants.StatusReasonHeaderKey, isSqlExcepton ? "Sql Exception" : "Db Update Exception");
                    exContext.Result = new ObjectResult(exContext.Result) { StatusCode = StatusCodes.Status500InternalServerError };
                    break;
                case Exception:
                    _log.LogError("{Controller}->{Action}{Message}:\r\n{Exception}", controllerName, actionName, ex.Message, ex);
                    exContext.HttpContext.Response.Headers.Append(Constants.StatusReasonHeaderKey, "Exception");
                    exContext.Result = new ObjectResult(exContext.Result) { StatusCode = StatusCodes.Status500InternalServerError };
                    break;
                default:
                    _log.LogError("{Controller}->{Action}{Message}:\r\n{Exception}", controllerName, actionName, ex.Message, ex);
                    exContext.HttpContext.Response.Headers.Append(Constants.StatusReasonHeaderKey, "Unknown Exception Type");
                    exContext.Result = new ObjectResult(exContext.Result) { StatusCode = StatusCodes.Status500InternalServerError };
                    break;
            }

            return Task.CompletedTask;
        }
    }
}