using Bulky.DataAccess.Exceptions;
using Bulky.Utility;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Abstractions;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using Microsoft.AspNetCore.Mvc.ViewFeatures;

namespace BulkyWeb.CustomMiddleWares
{
    public class CustomExceptionHandlerMiddleWare
    {
        private readonly RequestDelegate _next;
        private readonly ILogger<CustomExceptionHandlerMiddleWare> _logger;

        public CustomExceptionHandlerMiddleWare(RequestDelegate next, ILogger<CustomExceptionHandlerMiddleWare> logger)
        {
            _next = next;
            _logger = logger;
        }

        public async Task Invoke(HttpContext context)
        {
            try
            {
                await _next(context);
                await Handle404IfEndpointNotFoundAsync(context);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Unhandled Exception");
                var errorVM = BuildErrorViewModelFromException(ex);
                await RenderErrorViewAsync(context, errorVM);
            }
        }

        private async Task Handle404IfEndpointNotFoundAsync(HttpContext context)
        {
            if (context.Response.StatusCode == StatusCodes.Status404NotFound)
            {
                var errorVM = new ErrorVM
                {
                    StatusCode = 404,
                    Message = $"Endpoint was not found."
                };

                await RenderErrorViewAsync(context, errorVM);
            }
        }

        private ErrorVM BuildErrorViewModelFromException(Exception ex)
        {
            return ex switch
            {
                BadRequestException bad => new ErrorVM
                {
                    StatusCode = 400,
                    Message = bad.Message,
                    Errors = bad.Errors
                },
                CategoryNotFoundException notFound => new ErrorVM
                {
                    StatusCode = 404,
                    Message = notFound.Message
                },
                _ => new ErrorVM
                {
                    StatusCode = 500,
                    Message = "Internal Server Error"
                }
            };
        }

        private async Task RenderErrorViewAsync(HttpContext context, ErrorVM errorVM)
        {
            context.Response.StatusCode = errorVM.StatusCode;

            var result = new ViewResult
            {
                ViewName = "Error",
                ViewData = new ViewDataDictionary<ErrorVM>(
                    new EmptyModelMetadataProvider(),
                    new ModelStateDictionary())
                {
                    Model = errorVM
                }
            };

            var actionContext = new ActionContext(
                context,
                context.GetRouteData(),
                new ActionDescriptor());

            await result.ExecuteResultAsync(actionContext);
        }
    }
}
