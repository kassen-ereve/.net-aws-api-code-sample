using Amazon.DynamoDBv2;
using Amazon.DynamoDBv2.Model;
using get_employee_lambda_function.Services;
using Moq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace get_employee_lambda_function_unittest.Services
{
    public class DynamoDBServiceTests
    {
        [Fact]
        public async Task GetAllEmployeesAsync_ReturnsParsedResults()
        {
            var mockItems = new List<Dictionary<string, AttributeValue>>
            {
                new Dictionary<string, AttributeValue>
                {
                    { "Name", new AttributeValue { S = "John" } }
                }
            };

            dynamoDBMock
                .Setup(x => x.QueryAsync(It.IsAny<QueryRequest>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync(new QueryResponse { Items = mockItems });

            var result = await service.GetAllEmployeesAsync();

            Assert.Single(result);
            Assert.Equal("John", result.First()["Name"]);
        }

        [Fact]
        public async Task GetEmployeesByCompanyIDAsync_ReturnsFilteredResults()
        {
            var mockItems = new List<Dictionary<string, AttributeValue>>
            {
                new Dictionary<string, AttributeValue>
                {
                    { "Name", new AttributeValue { S = "Alice" } }
                }
            };

            dynamoDBMock
                .Setup(x => x.QueryAsync(It.IsAny<QueryRequest>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync(new QueryResponse { Items = mockItems });

            var result = await service.GetEmployeesByCompanyIDAsync("COMP123");

            Assert.Single(result);
            Assert.Equal("Alice", result.First()["Name"]);
        }

        [Fact]
        public async Task GetEmployeesByIDAsync_ReturnsSingleEmployee()
        {
            var mockItems = new List<Dictionary<string, AttributeValue>>
            {
                new Dictionary<string, AttributeValue>
                {
                    { "Name", new AttributeValue { S = "Bob" } }
                }
            };

            dynamoDBMock
                .Setup(x => x.QueryAsync(It.IsAny<QueryRequest>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync(new QueryResponse { Items = mockItems });

            var result = await service.GetEmployeesByIDAsync("EMP456");

            Assert.NotNull(result);
            Assert.Equal("Bob", result["Name"]);
        }

        [Fact]
        public async Task GetEmployeesByIDAsync_ReturnsNull_WhenNoMatch()
        {
            dynamoDBMock
                .Setup(x => x.QueryAsync(It.IsAny<QueryRequest>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync(new QueryResponse { Items = new List<Dictionary<string, AttributeValue>>() });

            var result = await service.GetEmployeesByIDAsync("NON_EXISTENT");

            Assert.Null(result);
        }

        public DynamoDBServiceTests()
        {
            dynamoDBMock = new Mock<IAmazonDynamoDB>();
            service = new DynamoDBService(dynamoDBMock.Object);
        }

        private readonly Mock<IAmazonDynamoDB> dynamoDBMock;
        private readonly DynamoDBService service;
    }
}
