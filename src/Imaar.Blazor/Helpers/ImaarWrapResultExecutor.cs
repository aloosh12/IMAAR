using Microsoft.AspNetCore.Mvc.Infrastructure;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using System;
using Microsoft.Extensions.Logging;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using System.Collections.Generic;
using Volo.Abp.Http;

namespace Imaar.Blazor.Helpers
{
    public class ImaarWrapResultExecutor : ObjectResultExecutor
    {
        public ImaarWrapResultExecutor(OutputFormatterSelector formatterSelector,
                                          IHttpResponseStreamWriterFactory writerFactory,
                                          ILoggerFactory loggerFactory,
                                          IOptions<MvcOptions> mvcOptions)
            : base(formatterSelector, writerFactory, loggerFactory, mvcOptions)
        {
        }

        public override Task ExecuteAsync(ActionContext context, ObjectResult result)
        {
            
            if (context.HttpContext.Request.Path.Value.Contains("/api/mobile/"))
            {
                var success = Success(context.HttpContext.Response.StatusCode);

               // context.HttpContext.Response.StatusCode = 200;

                var response = new ResponseEnvelope<object>();
                response.Data = result.Value;
             //   response.Success = success;
                if (!success)
                {
                    var res = result.Value as RemoteServiceErrorResponse;
                    response.Message = res.Error.Message;
                    response.Data = null;
                    response.Code = context.HttpContext.Response.StatusCode;
                }

                //if (result.Value != null)
                //{
                //    TypeCode typeCode = Type.GetTypeCode(result.Value.GetType());
                //    if (typeCode == TypeCode.Object)
                //        result.Value = response;
                //}

                result.Value = response;
            }

            return base.ExecuteAsync(context, result);
        }

        

        private bool Success(int code)
        {
            var successList = new List<int>() {
           StatusCodes.Status200OK,
           StatusCodes.Status201Created,
           //StatusCodes.Status202Accepted,
           StatusCodes.Status204NoContent,
           //StatusCodes.Status205ResetContent,
           //StatusCodes.Status206PartialContent,
           //StatusCodes.Status207MultiStatus,
           //StatusCodes.Status208AlreadyReported,
           //StatusCodes.Status226IMUsed

           };

            if (successList.Contains(code))
                return true;

            return false;
        }
    }

    internal class ResponseEnvelope<T>
    {
        //public T Data { set; get; }
        //public string ApiVersion { set; get; }
        //public string OtherInfoHere { set; get; }

        public T Data { set; get; }
        public string Message { get; set; }
        public int Code { get; set; }
    }
}
