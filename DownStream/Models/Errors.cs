
using System.Net;

namespace DownStream.Models
{
    public class ApiException : Exception
    {
        public HttpStatusCode StatusCode { get; set; }

        public ApiException(string message, HttpStatusCode statusCode = HttpStatusCode.InternalServerError)
            : base(message)
        {
            StatusCode = statusCode;
        }
    }

    // Error details model
    public class ErrorDetails
    {
        public string StatusCode { get; set; }
        public string Message { get; set; }

        public override string ToString()
        {
            return System.Text.Json.JsonSerializer.Serialize(this);
        }
    }

    public static class ErrorCodes
    {
        public const string GeneralException = "01";
        public const string InvalidRequest = "02";
    }
}