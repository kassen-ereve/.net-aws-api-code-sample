using Amazon.DynamoDBv2.Model;
using Amazon.Lambda.APIGatewayEvents;
using get_employee_lambda_function.Helpers;
using System.Net;

namespace get_employee_lambda_function_unittest
{
    public class ApiHelperTests
    {
        [Fact]
        public void GetResponse_ReturnsCorrectResponse()
        {
            var body = "{\"message\":\"test\"}";
            var statusCode = 200;

            var response = ApiHelper.GetResponse(body, statusCode);

            Assert.Equal(body, response.Body);
            Assert.Equal(statusCode, response.StatusCode);
            Assert.True(response.Headers.ContainsKey("Content-Type"));
            Assert.Equal("application/json", response.Headers["Content-Type"]);
        }

        [Theory]
        [InlineData("Success message")]
        [InlineData("")]
        [InlineData(null)]
        public void GetSuccessResponse_ReturnsOkWithMessage(string message)
        {
            var response = ApiHelper.GetSuccessResponse(message);

            Assert.Equal((int)HttpStatusCode.OK, response.StatusCode);
            Assert.Contains("Message", response.Body);
            if (message != null)
                Assert.Contains(message, response.Body);
        }

        [Fact]
        public void GetSuccessResponseJSON_ReturnsOkWithSerializedObject()
        {
            var body = new { Key = "value" };

            var response = ApiHelper.GetSuccessResponseJSON(body);

            Assert.Equal((int)HttpStatusCode.OK, response.StatusCode);
            Assert.Contains("value", response.Body);
        }

        [Theory]
        [InlineData("Error occurred")]
        [InlineData("")]
        [InlineData(null)]
        public void GetFailedResponse_ReturnsInternalServerError(string message)
        {
            var response = ApiHelper.GetFailedResponse(message);

            Assert.Equal((int)HttpStatusCode.InternalServerError, response.StatusCode);
            Assert.Contains("Error", response.Body);
            if (message != null)
                Assert.Contains(message, response.Body);
        }

        [Fact]
        public void GetBadRequestResponse_ReturnsBadRequest()
        {
            var message = "Invalid input";

            var response = ApiHelper.GetBadRequestResponse(message);

            Assert.Equal((int)HttpStatusCode.BadRequest, response.StatusCode);
            Assert.Contains("Invalid input", response.Body);
        }

        [Fact]
        public void GetNotFoundResponse_ReturnsNotFound()
        {
            var message = "Not found";

            var response = ApiHelper.GetNotFoundResponse(message);

            Assert.Equal((int)HttpStatusCode.NotFound, response.StatusCode);
            Assert.Contains("Not found", response.Body);
        }

        [Fact]
        public void SearchEmployees_ReturnsMatchingEmployees_CaseInsensitive()
        {
            var list = new List<Dictionary<string, string>>
        {
            new Dictionary<string, string> { { "EmployeeName", "John Doe" } },
            new Dictionary<string, string> { { "EmployeeName", "Alice Smith" } },
            new Dictionary<string, string> { { "OtherKey", "NoName" } }
        };

            var result = ApiHelper.SearchEmployees("john", list).ToList();

            Assert.Single(result);
            Assert.Equal("John Doe", result[0]["EmployeeName"]);
        }

        [Fact]
        public void SearchEmployees_ReturnsEmpty_WhenNoMatch()
        {
            var list = new List<Dictionary<string, string>>
        {
            new Dictionary<string, string> { { "EmployeeName", "Alice" } }
        };

            var result = ApiHelper.SearchEmployees("Bob", list);

            Assert.Empty(result);
        }

        [Fact]
        public void SearchEmployees_IgnoresEntriesWithoutEmployeeName()
        {
            var list = new List<Dictionary<string, string>>
        {
            new Dictionary<string, string> { { "OtherKey", "Value" } }
        };

            var result = ApiHelper.SearchEmployees("anything", list);

            Assert.Empty(result);
        }

        [Fact]
        public void GetStringAttributes_ReturnsAllStringValues()
        {
            var attributes = new Dictionary<string, AttributeValue>
        {
            { "Name", new AttributeValue { S = "Alice" } },
            { "Age", new AttributeValue { S = "30" } }
        };

            var result = ApiHelper.GetStringAttributes(attributes);

            Assert.Equal("Alice", result["Name"]);
            Assert.Equal("30", result["Age"]);
        }

        [Fact]
        public void GetStringAttributes_ReturnsEmptyStringForNullValues()
        {
            var attributes = new Dictionary<string, AttributeValue>
        {
            { "Missing", null },
            { "NullString", new AttributeValue() }
        };

            var result = ApiHelper.GetStringAttributes(attributes);

            Assert.Equal("", result["Missing"]);
            Assert.Equal("", result["NullString"]);
        }

        [Fact]
        public void GetCloudwatchErrorLog_ReturnsFormattedJson()
        {
            var request = new APIGatewayProxyRequest
            {
                Body = "input body",
                Path = "/example",
                PathParameters = new Dictionary<string, string> { { "id", "123" } },
                QueryStringParameters = new Dictionary<string, string> { { "search", "value" } }
            };

            var ex = new Exception("Test error");

            var result = ApiHelper.GetCloudwatchErrorLog(request, ex);

            Assert.Contains("FunctionHandler", result);
            Assert.Contains("Test error", result);
            Assert.Contains("/example", result);
            Assert.Contains("input body", result);
            Assert.Contains("PathParameters", result);
            Assert.Contains("QueryParameters", result);
        }

        [Fact]
        public void GetCloudwatchErrorLog_HandlesNullProperties()
        {
            var request = new APIGatewayProxyRequest
            {
                Body = null,
                Path = null,
                PathParameters = null,
                QueryStringParameters = null
            };

            var ex = new Exception("Some exception");

            var result = ApiHelper.GetCloudwatchErrorLog(request, ex);

            Assert.Contains("Some exception", result);
            Assert.Contains("FunctionHandler", result);
            Assert.Contains("Path", result);
            Assert.Contains("Payload", result);
        }
    }
}