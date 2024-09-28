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
    }
}
