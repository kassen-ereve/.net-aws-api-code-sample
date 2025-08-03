using Amazon.Lambda.APIGatewayEvents;
using get_employee_lambda_function.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace get_employee_lambda_function.Helpers
{
    public static class ValidationHelper
    {
        public static void ValidateHTTPRequestMethod(string requestHTTPMethod, string expectedHTTPMethod)
        {
            if (!string.Equals(requestHTTPMethod, expectedHTTPMethod, StringComparison.OrdinalIgnoreCase))
                throw new Exception($"Incorrect AWS API configuration for this lambda. This Lambda only accept [{expectedHTTPMethod}] Requests. Current request's HTTP method: {requestHTTPMethod}");
        }
        public static APIGatewayProxyResponse? ValidateUUID(string uuid, int expectedIdLength, string idLabel)
        {
            // Check if the string contains any whitespace characters
            if (uuid.Any(char.IsWhiteSpace))
                return ApiHelper.GetBadRequestResponse(message: $"Invalid {idLabel}: The provided ID {uuid} must not contain any whitespace.");

            // Check if the string length 
            if (uuid.Length != expectedIdLength)
                return ApiHelper.GetBadRequestResponse(message: $"Invalid {idLabel}: The provided ID {uuid} must be exactly {expectedIdLength} in length.");

            return null;
        }

        public static APIGatewayProxyResponse? ValidateEmployeeID(string employeeID) => ValidateUUID(uuid: employeeID, expectedIdLength: 73, idLabel: "EmployeeID");

        public static APIGatewayProxyResponse? ValidateCompanyID(string companyID) => ValidateUUID(uuid: companyID, expectedIdLength: 36, idLabel: "CompanyID");

    }
}
