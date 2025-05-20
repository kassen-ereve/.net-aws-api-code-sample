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
    }
}
