using Amazon.Lambda.APIGatewayEvents;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;

namespace get_employee_lambda_function.Helpers
{
    public static class ApiHelper
    {
        public static APIGatewayProxyResponse GetResponse(string body, int statusCode) =>
            new APIGatewayProxyResponse()
            {
                Body = body,
                StatusCode = statusCode,
                Headers = new Dictionary<string, string> { { "Content-Type", "application/json" } }
            };

        public static APIGatewayProxyResponse GetSuccessResponse(string message)
        {
            var jsonMessage = new { Message = message };
            return GetResponse(body: JsonSerializer.Serialize(jsonMessage), statusCode: (int)HttpStatusCode.OK);
        }


        public static APIGatewayProxyResponse GetSuccessResponseJSON<T>(T body) =>
            GetResponse(body: JsonSerializer.Serialize<T>(body), statusCode: (int)HttpStatusCode.OK);

        public static APIGatewayProxyResponse GetFailedResponse(string message)
        {
            var jsonError = new { Error = message };
            return GetResponse(body: JsonSerializer.Serialize(jsonError), statusCode: (int)HttpStatusCode.InternalServerError);
        }

        public static APIGatewayProxyResponse GetBadRequestResponse(string message)
        {
            var jsonError = new { Error = message };
            return GetResponse(body: JsonSerializer.Serialize(jsonError), statusCode: (int)HttpStatusCode.BadRequest);
        }

        public static APIGatewayProxyResponse GetNotFoundResponse(string message)
        {
            var jsonError = new { Error = message };
            return GetResponse(body: JsonSerializer.Serialize(jsonError), statusCode: (int)HttpStatusCode.NotFound);
        }


        public static string GetCloudwatchErrorLog(APIGatewayProxyRequest apiRequest, Exception exception)
        {
            var error = new
            {
                Module = "FunctionHandler",
                ErrorMessage = exception.Message,
                ErrorTrack = exception.StackTrace,
                DateTime = DateTime.UtcNow.ToString(),
                Path = apiRequest.Path,
                Payload = new
                {
                    Body = apiRequest.Body,
                    PathParameters = apiRequest.PathParameters,
                    QueryParameters = apiRequest.QueryStringParameters
                }
            };

            return JsonSerializer.Serialize(error);
        }
    }
}
