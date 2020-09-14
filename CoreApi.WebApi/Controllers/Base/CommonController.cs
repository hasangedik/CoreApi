using System.Net;
using CoreApi.Contract;
using CoreApi.WebApi.Filters;
using Microsoft.AspNetCore.Mvc;

namespace CoreApi.WebApi.Controllers.Base
{
    [TypeFilter(typeof(TransactionManagementFilter))]
    public class CommonController : ControllerBase
    {
        [NonAction]
        protected new OkObjectResult Ok() => Ok(null);

        [NonAction]
        public override OkObjectResult Ok(object value)
        {
            var response = new ResponseContract
            {
                Message = "OK",
                StatusCode = HttpStatusCode.OK,
                Result = value
            };

            return base.Ok(response);
        }

        [NonAction]
        protected OkObjectResult ErrorResult(string message, HttpStatusCode errorCode)
        {
            var response = new ResponseContract
            {
                Error = true,
                ErrorCode = (int)errorCode,
                Message = message,
                StatusCode = HttpStatusCode.OK
            };

            return base.Ok(response);
        }

        [NonAction]
        public new UnauthorizedObjectResult Unauthorized() => Unauthorized(null);

        [NonAction]
        public override UnauthorizedObjectResult Unauthorized(object value)
        {
            var response = new ResponseContract
            {
                Error = true,
                Message = "Unauthorized user.",
                Result = value,
                StatusCode = HttpStatusCode.Unauthorized
            };
            return base.Unauthorized(response);
        }

        [NonAction]
        public new BadRequestObjectResult BadRequest() => BadRequest(null);

        [NonAction]
        protected BadRequestObjectResult BadRequest(string message)
        {
            var response = new ResponseContract
            {
                Error = true,
                Message = message,
                StatusCode = HttpStatusCode.BadRequest,
                ErrorCode = 400
            };

            return base.BadRequest(response);
        }

        [NonAction]
        public BadRequestObjectResult BadRequest(string message, object value, int errorCode = 400)
        {
            var response = new ResponseContract
            {
                Error = true,
                Message = message,
                Result = value,
                StatusCode = HttpStatusCode.BadRequest,
                ErrorCode = errorCode
            };
            return base.BadRequest(response);
        }

        [NonAction]
        protected ObjectResult InternalServerError(string message, object data = null)
        {
            var response = new ResponseContract
            {
                Error = true,
                Message = message,
                Result = data,
                StatusCode = HttpStatusCode.InternalServerError
            };
            return base.StatusCode((int)HttpStatusCode.InternalServerError, response);
        }

        [NonAction]
        public ObjectResult NotFoundError(string message = "Content not found.", object data = null, int errorCode = 404)
        {
            var response = new ResponseContract
            {
                Error = true,
                Message = message,
                Result = data,
                StatusCode = HttpStatusCode.NotFound,
                ErrorCode = errorCode
            };
            return base.StatusCode((int)HttpStatusCode.NotFound, response);
        }

        [NonAction]
        public ObjectResult ForbiddenError(string message = null, object data = null, int errorCode = 403)
        {
            var response = new ResponseContract
            {
                Error = true,
                Message = message,
                Result = data,
                StatusCode = HttpStatusCode.Forbidden,
                ErrorCode = errorCode
            };
            return base.StatusCode((int)HttpStatusCode.Forbidden, response);
        }

        [NonAction]
        public ObjectResult CreatedResponse(string message, object data = null)
        {
            var response = new ResponseContract
            {
                Error = false,
                Message = message,
                Result = data,
                StatusCode = HttpStatusCode.Created,
                ErrorCode = (int)HttpStatusCode.Created
            };
            return base.StatusCode((int)HttpStatusCode.Created, response);
        }

        [NonAction]
        public ObjectResult ConflictResponse(string message, object data = null)
        {
            var response = new ResponseContract
            {
                Error = true,
                Message = message,
                Result = data,
                StatusCode = HttpStatusCode.Conflict,
                ErrorCode = (int)HttpStatusCode.Conflict
            };
            return base.StatusCode((int)HttpStatusCode.Conflict, response);
        }

        private string GetCurrentUrl(bool withQueryString = false)
        {
            return withQueryString ? $"{Request.Scheme}://{Request.Host}{Request.Path}{Request.QueryString}" : $"{Request.Scheme}://{Request.Host}{Request.Path}";
        }
    }
}