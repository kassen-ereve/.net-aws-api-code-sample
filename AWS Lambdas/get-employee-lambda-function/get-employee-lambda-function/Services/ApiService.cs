using Amazon.DynamoDBv2.Model;
using Amazon.Lambda.APIGatewayEvents;
using get_employee_lambda_function.Helpers;
using System;
using System.Collections.Generic;
using System.ComponentModel.Design;
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
            apiRequest.QueryStringParameters?.TryGetValue("EmployeeName", out searchText);

            var result = await dynamoDBService.GetAllEmployeesAsync();

            if (!string.IsNullOrWhiteSpace(searchText)) // Is doing a search
                result = SearchEmployees(searchText: searchText, employeeList: result);

            return GetSuccessResponseJSON<IEnumerable<Dictionary<string, string>>>(result);
        }

        public async Task<APIGatewayProxyResponse> GetByCompanyIDAsync(APIGatewayProxyRequest apiRequest, IDynamoDBService dynamoDBService)
        {
            string? companyID = null,
                searchText = null;

            apiRequest.PathParameters?.TryGetValue("CompanyID", out companyID);
            apiRequest.QueryStringParameters?.TryGetValue("EmployeeName", out searchText);

            if (apiRequest.PathParameters == null || !apiRequest.PathParameters.Any() || string.IsNullOrWhiteSpace(companyID))
                return GetBadRequestResponse(message: "Missing required CompanyID on endpoint [/employee/company/{CompanyID}]");

            var idValidationResult = ValidationHelper.ValidateCompanyID(companyID: companyID);

            if (idValidationResult != null) // Validation failed return the validation result
                return idValidationResult;

            var result = await dynamoDBService.GetEmployeesByCompanyIDAsync(companyID: companyID);

            if (!string.IsNullOrWhiteSpace(searchText)) // Is doing a search
                result = SearchEmployees(searchText: searchText, employeeList: result);

            return GetSuccessResponseJSON<IEnumerable<Dictionary<string, string>>>(result);
        }

        public async Task<APIGatewayProxyResponse> GetByEmployeeIDAsync(APIGatewayProxyRequest apiRequest, IDynamoDBService dynamoDBService)
        {
            string? employeeID = null;
            apiRequest.PathParameters?.TryGetValue("EmployeeID", out employeeID);

            if (apiRequest.PathParameters == null || !apiRequest.PathParameters.Any() || string.IsNullOrWhiteSpace(employeeID))
                return GetBadRequestResponse(message: "Missing required EmployeeID on endpoint [/employee/{EmployeeID}]");

            var idValidationResult = ValidationHelper.ValidateEmployeeID(employeeID: employeeID);

            if (idValidationResult != null) // Validation failed return the validation result
                return idValidationResult;

            var result = await dynamoDBService.GetEmployeesByIDAsync(employeeID: employeeID);

            return result != null && result.Any()
                ? GetSuccessResponseJSON<Dictionary<string, string>>(result)
                : GetNotFoundResponse($"Employee with an ID[{employeeID}] not found.");
        }
    }
}
