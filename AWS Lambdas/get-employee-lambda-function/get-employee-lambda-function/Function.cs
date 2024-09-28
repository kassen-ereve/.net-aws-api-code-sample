using Amazon.Lambda.APIGatewayEvents;
using Amazon.Lambda.Core;
using get_employee_lambda_function.Services;
using static get_employee_lambda_function.Helpers.ValidationHelper;

// Assembly attribute to enable the Lambda function's JSON input to be converted into a .NET class.
[assembly: LambdaSerializer(typeof(Amazon.Lambda.Serialization.SystemTextJson.DefaultLambdaJsonSerializer))]

namespace get_employee_lambda_function;

public class Function
{
    IDynamoDBService DynamoDBService { get; }
    IApiService ApiService { get; }

    public Function() : this(new DynamoDBService(), new ApiService())
    { }

    public Function(IDynamoDBService dynamoDBService, IApiService apiService)
    {
        DynamoDBService = dynamoDBService;
        ApiService = apiService;
    }

    public async Task<APIGatewayProxyResponse> FunctionHandlerAsync(APIGatewayProxyRequest apiRequest, ILambdaContext context)
    {
        try
        {
            ValidateHTTPRequestMethod(requestHTTPMethod: apiRequest.HttpMethod, expectedHTTPMethod: "GET");

            switch (apiRequest.Resource)
            {
                case "/employee":
                    return await ApiService.GetAllEmployeesAsync(apiRequest: apiRequest, dynamoDBService: DynamoDBService);
                case "/employee/{employeeID}":
                    return await ApiService.GetByEmployeeIDAsync(apiRequest: apiRequest, dynamoDBService: DynamoDBService);
                case "/employee/company/{CompanyID}":
                    return await ApiService.GetByCompanyIDAsync(apiRequest: apiRequest, dynamoDBService: DynamoDBService);
                default:
                    return GetNotFoundResponse($"No API Endpoint found for resource [{apiRequest.Resource}]");
            }
        }
        catch (Exception ex)
        {
            context.Logger.LogLine($"Error: {GetCloudwatchErrorLog(apiRequest: apiRequest, exception: ex)}");

            return GetFailedResponse(message: "Internal Server Error");
        }
    }
}