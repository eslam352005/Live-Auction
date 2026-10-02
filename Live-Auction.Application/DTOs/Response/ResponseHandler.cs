using System;
using System.Collections.Generic;
using System.Text;

namespace Live_Auction.Application.DTOs.Response
{
    public static class ResponseHandler
    {

        public static ResponseDto<T> Success<T>(T data, string message = null!)
        {
            return new ResponseDto<T>()
            {

                Success = true,
                StatusCode = 200,
                Message = message ?? "Request was successful.",
                Data = data
            };
        }
        public static ResponseDto<T> NotFound<T>(string message = null!)
        {
            return new ResponseDto<T>()
            {

                Success = false,
                StatusCode = 404,
                Message = message ?? " Not found.",

            };
        }
        public static ResponseDto<T> BadRequest<T>(string message = null!, List<string> errors = null!)
        {
            return new ResponseDto<T>()
            {

                Success = false,
                StatusCode = 400,
                Message = message ?? "Bad request.",
                Errors = errors

            };
        }
        public static ResponseDto<T> UnAuthorized<T>(string message = null!)
        {
            return new ResponseDto<T>()
            {

                Success = false,
                StatusCode = 401,
                Message = message ?? "Unauthorized.",

            };
        }
    }
}
