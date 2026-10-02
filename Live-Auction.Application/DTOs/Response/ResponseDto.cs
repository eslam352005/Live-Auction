using System;
using System.Collections.Generic;
using System.Text;

namespace Live_Auction.Application.DTOs.Response
{
    public class ResponseDto<T>
    {
        public bool Success { get; set; }
        public int StatusCode { get; set; }
        public string Message { get; set; }
        public T Data { get; set; }
        public List<string> Errors { get; set; }

        public ResponseDto()
        {
            Errors = new List<string>();
        }

        public ResponseDto(T data)
        {
            Success = true;
            Data = data;
            StatusCode = 200;
            Errors = new List<string>();
        }

        public ResponseDto(string message, int statusCode)
        {
            Success = false;
            Message = message;
            StatusCode = statusCode;
            Errors = new List<string>();
        }
        public ResponseDto(string message, int statusCode, List<string> errors)
        {
            Success = false;
            Message = message;
            StatusCode = statusCode;
            Errors = errors;
        }
    }
}
