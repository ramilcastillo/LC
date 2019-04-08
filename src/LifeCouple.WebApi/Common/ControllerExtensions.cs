using System;
using LifeCouple.WebApi.DomainLogic;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ModelBinding;

namespace LifeCouple.WebApi.Common
{
    public static class ControllerExtensions
    {
        /// <summary>
        /// Typically used for Get operations when no data can be found
        /// </summary>
        /// <param name="controller"></param>
        /// <param name="message"></param>
        /// <returns></returns>
        public static NotFoundObjectResult ApiErrorMessage404NotFound(this Controller controller, string message)
        {
            return new NotFoundObjectResult(new ErrorResponseInfo { Error = new Error { Code = ValidationErrorCode.NotFound.ToString(), Message = message } });
        }

        public static ObjectResult ApiMessage501NotImplemented(this Controller controller, object message = null)
        {
            return controller.StatusCode(StatusCodes.Status501NotImplemented, $"debugInfo:{controller.ControllerContext.ActionDescriptor.ActionName} {(message ?? null)}");
        }

        /// <summary>
        /// To be used with successfull Put responses since we do not responed with content (that would mean a 200 response code)
        /// </summary>
        /// <param name="controller"></param>
        /// <returns></returns>
        public static NoContentResult ApiPutMessage204NotContent(this Controller controller)
        {
            //TODO: consider adding logging
            return new NoContentResult();
        }

        /// <summary>
        /// To be used with successfull POST responses since we do not responed with content (that would mean a 200 response code)
        /// </summary>
        /// <param name="controller"></param>
        /// <returns></returns>
        public static NoContentResult ApiPostMessage204NotContent(this Controller controller)
        {
            //TODO: consider adding logging
            return new NoContentResult();
        }


        public static IActionResult ApiErrorMessage400BadRequestUserIdInTokenNotFound(this Controller controller, string externalRefId)
        {
            return controller.ApiErrorMessage400BadRequest(null, null, externalRefId);
        }

        public static IActionResult ApiErrorMessage400BadRequest(this Controller controller, Exception exc)
        {
            return controller.ApiErrorMessage400BadRequest(exc, null, null);
        }

        public static IActionResult ApiErrorMessage400BadRequest(this Controller controller, ModelStateDictionary modelState)
        {
            return controller.ApiErrorMessage400BadRequest(null, modelState, null);
        }

        public static IActionResult ApiErrorMessage400BadRequest(this Controller controller, BusinessLogicException businessLogicException)
        {
            var errorResponse = new ErrorResponseInfo
            {
                Error = new Error
                {
                    Code = businessLogicException.ErrorCode.ToString(),
                    Message = $"BusinessLogicException",
                    DebugMessage = businessLogicException.DebugMessage,
                }
            };
            return controller.BadRequest(errorResponse);
        }

        public static IActionResult ApiErrorMessage400BadRequest(this Controller controller, Exception exc, ModelStateDictionary modelState, string externalRefId)
        {
            //TODO: consider logging

            var errorResponse = new ErrorResponseInfo();
            if (exc == null && modelState == null)
            {
                errorResponse.Error = new Error { Code = "UserIdInTokenNotFound", Message = $"UserId '{externalRefId}' in Bearer token not found in Application data, user needs to be created first" };
                return controller.BadRequest(errorResponse);
            }

            if (exc != null)
            {
                errorResponse.Error = new Error { Code = ValidationErrorCode.UnhandledException.ToString(), DebugMessage = exc.Message, Message = exc.GetType().Name };
                return controller.BadRequest(errorResponse);
            }

            if (modelState != null)
            {
                errorResponse.Error = new Error { Code = "InvalidModelState", Message = modelState.ToString() };
                return controller.BadRequest(errorResponse);
            }

            errorResponse.Error = new Error { Code = "UnknownError", Message = "An unknown error occured" };
            return controller.BadRequest(errorResponse);


        }

        public static JwtPayloadInfo GetJwtPayloadInfo(this Controller controller)
        {
            var payload = JwtPayloadInfo.Extract(controller.HttpContext.User.Claims);

            //Added for instrumentation purposes
            controller.HttpContext.Items.Add("lc_ExtRefId", payload.ExtReferenceId);

            return payload;
        }
    }
}
