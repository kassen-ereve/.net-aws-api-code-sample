using Amazon.Lambda.APIGatewayEvents;
using Amazon.Lambda.Core;
using get_employee_lambda_function;
using get_employee_lambda_function.Services;
using Moq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace get_employee_lambda_function_unittest
{
    public class FunctionTests
    {
        [Fact]
        public async void FunctionHandlerAsync_should_call_GetAllEmployeesAsync_if_correct_end_point_resource()
        {
            var request = new APIGatewayProxyRequest { Resource = "/employee", HttpMethod = "GET" };

            await targetClass.FunctionHandlerAsync(request, lambdaContextMock.Object);

            apiServiceMock.Verify(
                expression: service => service.GetAllEmployeesAsync(It.IsAny<APIGatewayProxyRequest>(), It.IsAny<IDynamoDBService>()),
                times: Times.Once
            );
        }

        [Fact]
        public async void FunctionHandlerAsync_should_call_GetByEmployeeIDAsync_if_correct_end_point_resource()
        {
            var request = new APIGatewayProxyRequest { Resource = "/employee/{EmployeeID}", HttpMethod = "GET" };

            await targetClass.FunctionHandlerAsync(request, lambdaContextMock.Object);

            apiServiceMock.Verify(
                expression: service => service.GetByEmployeeIDAsync(It.IsAny<APIGatewayProxyRequest>(), It.IsAny<IDynamoDBService>()),
                times: Times.Once
            );
        }

        [Fact]
        public async void FunctionHandlerAsync_should_call_GetByCompanyIDAsync_if_correct_end_point_resource()
        {
            var request = new APIGatewayProxyRequest { Resource = "/employee/company/{CompanyID}", HttpMethod = "GET" };

            await targetClass.FunctionHandlerAsync(request, lambdaContextMock.Object);

            apiServiceMock.Verify(
                expression: service => service.GetByCompanyIDAsync(It.IsAny<APIGatewayProxyRequest>(), It.IsAny<IDynamoDBService>()),
                times: Times.Once
            );
        }


        [Fact]
        public async void FunctionHandlerAsync_should_return_not_found_response_if_endpoint_not_found()
        {
            var request = new APIGatewayProxyRequest { Resource = "/invalid/endpoint", HttpMethod = "GET" };

            var result = await targetClass.FunctionHandlerAsync(request, lambdaContextMock.Object);

            Assert.Equal((int)HttpStatusCode.NotFound, result.StatusCode);
            Assert.Equal("{\"Error\":\"No API Endpoint found for resource [/invalid/endpoint]\"}", result.Body);
        }

        public FunctionTests()
        {
            // Dependencies
            apiServiceMock = new Mock<IApiService>();
            dynamoDBServiceMock = new Mock<IDynamoDBService>();
            lambdaContextMock = new Mock<ILambdaContext>();

            // Target Class
            targetClass = new Function(dynamoDBServiceMock.Object, apiServiceMock.Object);
        }

        private readonly Mock<IApiService> apiServiceMock;
        private readonly Mock<IDynamoDBService> dynamoDBServiceMock;
        private readonly Mock<ILambdaContext> lambdaContextMock;
        private readonly Function targetClass;
    }
}
