# 🧑‍💼 Get Employee Lambda

## 📌 Description

This AWS Lambda function serves as an API handler for **retrieving employee data** from DynamoDB. It supports the following operations via **API Gateway**:

- `GET /employee` – Retrieve all employees.
- `GET /employee/{EmployeeID}` – Retrieve an employee by their unique ID.
- `GET /employee/company/{CompanyID}` – Retrieve employees belonging to a specific company.

---

## 🚀 Trigger

- **Trigger Type:** API Gateway (HTTP API or REST API)
- **HTTP Methods:** `GET`

---

## 🛠️ Function Signature

```csharp
public async Task<APIGatewayProxyResponse> FunctionHandlerAsync(APIGatewayProxyRequest apiRequest, ILambdaContext context)