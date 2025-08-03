# 🧑‍💼 Post Employee Lambda

## 📌 Description

This AWS Lambda function serves as an API handler for **creating or saving employee data** to DynamoDB. It supports the following operations via **API Gateway**:

- `POST /employee` – Save employee data.

---

## 🚀 Trigger

- **Trigger Type:** API Gateway (HTTP API or REST API)
- **HTTP Methods:** `POST`

---

## 🛠️ Function Signature

```csharp
public async Task<APIGatewayProxyResponse> FunctionHandlerAsync(APIGatewayProxyRequest apiRequest, ILambdaContext context)