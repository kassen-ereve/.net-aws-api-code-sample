using Amazon.Lambda.APIGatewayEvents;
using get_employee_lambda_function.Services;
using Moq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace get_employee_lambda_function_unittest.Services
{
    public class ApiServiceTests
    {
        [Fact]
        public async Task GetAllEmployeesAsync_ReturnsAllEmployees_WhenNoSearchTextProvided()
        {
            var employees = new List<Dictionary<string, string>> { new() { ["Name"] = "John" } };
            var request = new APIGatewayProxyRequest();

            dynamoDBServiceMock.Setup(x => x.GetAllEmployeesAsync()).ReturnsAsync(employees);

            var response = await targetClass.GetAllEmployeesAsync(request, dynamoDBServiceMock.Object);

            Assert.Equal(200, response.StatusCode);
            Assert.Contains("John", response.Body);
        }

        [Fact]
        public async Task GetAllEmployeesAsync_FiltersByName_WhenSearchTextProvided()
        {
            var employees = new List<Dictionary<string, string>>
            {
                new() { ["EmployeeName"] = "Alice Smith" },
                new() { ["EmployeeName"] = "Bob Johnson" }
            };

            var request = new APIGatewayProxyRequest
            {
                QueryStringParameters = new Dictionary<string, string> { ["EmployeeName"] = "alice" }
            };

            dynamoDBServiceMock.Setup(x => x.GetAllEmployeesAsync()).ReturnsAsync(employees);

            var response = await targetClass.GetAllEmployeesAsync(request, dynamoDBServiceMock.Object);

            Assert.Equal(200, response.StatusCode);
            Assert.Contains("Alice", response.Body);
            Assert.DoesNotContain("Bob", response.Body);
        }

        [Fact]
        public async Task GetByCompanyIDAsync_ReturnsBadRequest_WhenCompanyIDMissing()
        {
            var request = new APIGatewayProxyRequest
            {
                PathParameters = new Dictionary<string, string>() // Empty dictionary
            };

            var response = await targetClass.GetByCompanyIDAsync(request, dynamoDBServiceMock.Object);

            Assert.Equal(400, response.StatusCode);
            Assert.Contains("Missing required CompanyID", response.Body);
        }

        [Fact]
        public async Task GetByCompanyIDAsync_ReturnsValidationError_WhenCompanyIDInvalid()
        {
            var request = new APIGatewayProxyRequest
            {
                PathParameters = new Dictionary<string, string> { ["CompanyID"] = "invalid" }
            };

            var response = await targetClass.GetByCompanyIDAsync(request, dynamoDBServiceMock.Object);

            Assert.Equal(400, response.StatusCode);
            Assert.Contains("Invalid CompanyID", response.Body);
        }

        [Fact]
        public async Task GetByCompanyIDAsync_ReturnsFilteredResults_WhenSearchTextProvided()
        {
            var request = new APIGatewayProxyRequest
            {
                PathParameters = new Dictionary<string, string> { ["CompanyID"] = new string('c', 36) },
                QueryStringParameters = new Dictionary<string, string> { ["EmployeeName"] = "Bob" }
            };

            var data = new List<Dictionary<string, string>>
            {
                new() { ["EmployeeName"] = "Bob" },
                new() { ["EmployeeName"] = "Alice" }
            };

            dynamoDBServiceMock
                .Setup(x => x.GetEmployeesByCompanyIDAsync(It.IsAny<string>()))
                .ReturnsAsync(data);

            var response = await targetClass.GetByCompanyIDAsync(request, dynamoDBServiceMock.Object);

            Assert.Equal(200, response.StatusCode);
            Assert.Contains("Bob", response.Body);
            Assert.DoesNotContain("Alice", response.Body);
        }

        [Fact]
        public async Task GetByEmployeeIDAsync_ReturnsNotFound_WhenNoResults()
        {
            var employeeID = new string('e', 73);
            var request = new APIGatewayProxyRequest
            {
                PathParameters = new Dictionary<string, string> { ["EmployeeID"] = employeeID }
            };

            dynamoDBServiceMock
                .Setup(x => x.GetEmployeesByIDAsync(employeeID))
                .ReturnsAsync(new Dictionary<string, string>());

            var response = await targetClass.GetByEmployeeIDAsync(request, dynamoDBServiceMock.Object);

            Assert.Equal(404, response.StatusCode);
            Assert.Contains("not found", response.Body);
        }

        [Fact]
        public async Task GetByEmployeeIDAsync_ReturnsEmployee_WhenFound()
        {
            var employeeID = new string('e', 73);
            var data = new Dictionary<string, string> { { "Name", "Bob" } };

            var request = new APIGatewayProxyRequest
            {
                PathParameters = new Dictionary<string, string> { ["EmployeeID"] = employeeID }
            };

            dynamoDBServiceMock
                .Setup(x => x.GetEmployeesByIDAsync(employeeID))
                .ReturnsAsync(data);

            var response = await targetClass.GetByEmployeeIDAsync(request, dynamoDBServiceMock.Object);

            Assert.Equal(200, response.StatusCode);
            Assert.Contains("Bob", response.Body);
        }

        public ApiServiceTests()
        {
            dynamoDBServiceMock = new Mock<IDynamoDBService>();
            targetClass = new ApiService();
        }

        private readonly Mock<IDynamoDBService> dynamoDBServiceMock;
        private readonly ApiService targetClass;
    }
}
