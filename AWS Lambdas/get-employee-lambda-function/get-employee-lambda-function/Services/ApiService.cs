using Amazon.Lambda.APIGatewayEvents;
using get_employee_lambda_function.Helpers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static get_employee_lambda_function.Helpers.ApiHelper;

namespace get_employee_lambda_function.Services
{
    public interface IApiService
    {
        Task<APIGatewayProxyResponse> GetAllEmployeesAsync(APIGatewayProxyRequest apiRequest, IDynamoDBService dynamoDBService);
        Task<APIGatewayProxyResponse> GetByEmployeeIDAsync(APIGatewayProxyRequest apiRequest, IDynamoDBService dynamoDBService);
        Task<APIGatewayProxyResponse> GetByCompanyIDAsync(APIGatewayProxyRequest apiRequest, IDynamoDBService dynamoDBService);
    }

    public class ApiService : IApiService
    {
        public async Task<APIGatewayProxyResponse> GetAllEmployeesAsync(APIGatewayProxyRequest apiRequest, IDynamoDBService dynamoDBService)
        {
            string? searchText = null;
            apiRequest.QueryStringParameters?.TryGetValue("Name", out searchText);

            var result = await dynamoDBService.GetAllEmployeesAsync();

            if (!string.IsNullOrWhiteSpace(searchText)) // Is doing a search
                result = SearchEmployees(searchText: searchText, employeeList: result);

            return GetSuccessResponseJSON<IEnumerable<Dictionary<string, string>>>(result);
        }
    }
}
