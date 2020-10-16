using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using MISA.LocationAPI.Constants;
using MISA.LocationAPI.Response;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace MISALocationV2.Services.APIKeyAuth
{
    public class APIKeyAuthAttribute : Attribute, IAsyncActionFilter
    {
        /// <summary>
        /// Authentication cho các API dựa trên apikey và project 
        /// </summary>
        /// <param name="context"></param>
        /// <param name="next"></param>
        /// <returns></returns>
        /// Created by bvbao (23/9/2020)
        public async Task OnActionExecutionAsync(ActionExecutingContext context, ActionExecutionDelegate next)
        {
            if (!context.HttpContext.Request.Headers.TryGetValue("apikey", out var potentialApiKey) ||
                !context.HttpContext.Request.Headers.TryGetValue("project", out var potentialProject))
            {
                context.Result = new ObjectResult(new ErrorResponse(ConstMessages.InvalidKeyCode, ConstMessages.InvalidKeyMsg));
                return;
            }

            var configuration = context.HttpContext.RequestServices.GetRequiredService <IConfiguration>();

            var apiKey = configuration.GetValue <string>("ApiKeyHeader:apikey");
            var project = configuration.GetValue<string>("ApiKeyHeader:project");

            if (!apiKey.Equals(potentialApiKey) || !project.Equals(potentialProject))
            {
                context.Result = new ObjectResult(new ErrorResponse(ConstMessages.InvalidKeyCode, ConstMessages.InvalidKeyMsg));
                return;
            }
            await next();
        }
    }
}
