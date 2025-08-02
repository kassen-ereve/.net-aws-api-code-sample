using get_employee_lambda_function.Helpers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace get_employee_lambda_function_unittest.Helpers
{
    public class ValidationHelperTests
    {
        [Theory]
        [InlineData("GET", "GET")]
        [InlineData("get", "GET")]
        [InlineData("POST", "post")]
        [InlineData("DeLeTe", "DELETE")]
        public void ValidateHTTPRequestMethod_ValidMethods_DoesNotThrow(string requestMethod, string expectedMethod)
        {
            // Act & Assert
            var exception = Record.Exception(() =>
                ValidationHelper.ValidateHTTPRequestMethod(requestMethod, expectedMethod));

            Assert.Null(exception);
        }

        [Theory]
        [InlineData("GET", "POST")]
        [InlineData("PUT", "DELETE")]
        [InlineData("PATCH", "GET")]
        public void ValidateHTTPRequestMethod_InvalidMethods_ThrowsException(string requestMethod, string expectedMethod)
        {
            // Act & Assert
            var ex = Assert.Throws<Exception>(() =>
                ValidationHelper.ValidateHTTPRequestMethod(requestMethod, expectedMethod));

            Assert.Contains("Incorrect AWS API configuration", ex.Message);
            Assert.Contains(expectedMethod, ex.Message);
            Assert.Contains(requestMethod, ex.Message);
        }
    }
}
