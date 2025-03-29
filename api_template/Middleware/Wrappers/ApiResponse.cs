using System;
using System.Runtime.Serialization;
using System.Text.Json.Serialization;

namespace api_template.Middleware.Wrappers
{
    [Serializable]
    [DataContract]
    public class ApiResponse
    {
        [DataMember]
        public string Version { get; set; }

        [DataMember]
        public int StatusCode { get; set; }

        [DataMember]
        public bool IsError { get; set; }

        [DataMember]
        public string Message { get; set; }

        [DataMember(EmitDefaultValue = false)]
        public ApiError? ResponseException { get; set; }

        [DataMember(EmitDefaultValue = false)]
        public object? Result { get; set; }

        [JsonConstructor]
        public ApiResponse(int statusCode, string message = "", object? result = null, ApiError? apiError = null, string apiVersion = "1.0.0")
        {
            StatusCode = statusCode;
            Message = message;
            Result = result;
            ResponseException = apiError;
            Version = apiVersion;
            IsError = false;
        }

        public ApiResponse(int statusCode, ApiError apiError)
        {
            StatusCode = statusCode;
            Message = apiError.ExceptionMessage;
            ResponseException = apiError;
            Version = "1.0.0";
            IsError = true;
        }
    }
}
