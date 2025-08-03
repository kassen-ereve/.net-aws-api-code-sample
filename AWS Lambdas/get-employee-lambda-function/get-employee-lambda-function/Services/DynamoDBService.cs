using Amazon.DynamoDBv2;
using Amazon.DynamoDBv2.Model;
using get_employee_lambda_function.Helpers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace get_employee_lambda_function.Services
{
    public interface IDynamoDBService
    {
        Task<IEnumerable<Dictionary<string, string>>> GetAllEmployeesAsync();
        Task<IEnumerable<Dictionary<string, string>>> GetEmployeesByCompanyIDAsync(string companyID);
        Task<Dictionary<string, string>> GetEmployeesByIDAsync(string employeeID);
    }

    public class DynamoDBService : IDynamoDBService
    {
        const string MainTable = "EFD";
        const string Entity = "Employee";

        private static readonly AmazonDynamoDBClient dynamoDBClient = new();

        private async Task<IEnumerable<Dictionary<string, string>>> GetCircuitsAsync(QueryRequest request)
        {
            var circuits = new List<Dictionary<string, string>>();
            Dictionary<string, AttributeValue>? lastEvaluatedKey = null;
            do
            {
                // DynamoDB returns paginated data; set the start key to continue from the last evaluated key
                request.ExclusiveStartKey = lastEvaluatedKey;

                QueryResponse response = await dynamoDBClient.QueryAsync(request);
                foreach (Dictionary<string, AttributeValue> attributeList in response.Items)
                {
                    var circuit = ApiHelper.GetStringAttributes(attributeList: attributeList);
                    circuits.Add(circuit);
                }

                lastEvaluatedKey = response.LastEvaluatedKey;
            } while (lastEvaluatedKey != null && lastEvaluatedKey.Count > 0);

            return circuits;
        }

        public async Task<IEnumerable<Dictionary<string, string>>> GetAllEmployeesAsync()
        {
            var request = new QueryRequest
            {
                TableName = MainTable,
                KeyConditionExpression = "Entity = :v_entity",
                ExpressionAttributeValues = new Dictionary<string, AttributeValue> {
                            {":v_entity", new AttributeValue { S = Entity }}}
            };

            return await GetCircuitsAsync(request: request);
        }

        public async Task<IEnumerable<Dictionary<string, string>>> GetEmployeesByCompanyIDAsync(string companyID)
        {
            var request = new QueryRequest
            {
                TableName = MainTable,
                KeyConditionExpression = "Entity = :v_entity and begins_with (EntityID, :v_entityid)",
                ExpressionAttributeValues = new Dictionary<string, AttributeValue> {
                            {":v_entity", new AttributeValue { S = Entity }},
                            {":v_entityid", new AttributeValue { S = companyID }}}
            };

            return await GetCircuitsAsync(request: request);
        }

        public async Task<Dictionary<string, string>> GetEmployeesByIDAsync(string employeeID)
        {
            var request = new QueryRequest
            {
                TableName = MainTable,
                KeyConditionExpression = "Entity = :v_entity and EntityID = :v_entityid",
                ExpressionAttributeValues = new Dictionary<string, AttributeValue> {
                            {":v_entity", new AttributeValue { S = Entity }},
                            {":v_entityid", new AttributeValue { S = employeeID }}}
            };

            var queryResult = await GetCircuitsAsync(request: request);
            return queryResult.FirstOrDefault();
        }
    }
}
