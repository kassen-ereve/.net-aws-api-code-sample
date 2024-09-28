# Employee API with AWS Lambda, API Gateway, and DynamoDB

This repository demonstrates a simple API for handling CRUD operations (GET, POST, PUT, DELETE) on an Employee entity. The project is built using AWS Lambda for serverless API handling, AWS API Gateway for routing, and DynamoDB as the database. Each CRUD operation is handled by a separate Lambda function. 

Swagger UI is integrated to allow viewers to test the API endpoints directly, without needing to set up AWS or run the project locally.

## Features

- **AWS Lambda**: Each CRUD operation (GET, POST, PUT, DELETE) is handled by a separate Lambda function for simplicity and scalability.
- **AWS API Gateway**: Used for routing API requests to the correct Lambda functions.
- **DynamoDB**: The database for persisting Employee data.
- **Swagger UI**: Provides a user-friendly interface for testing the API endpoints.

## API Endpoints

The following endpoints are available via Swagger UI for testing:

- `GET /employee/{id}` - Fetches an employee by ID.
- `POST /employee` - Adds a new employee.
- `PUT /employee/{id}` - Updates an employee's details.
- `DELETE /employee/{id}` - Deletes an employee.

## Testing the API

You don't need to set up AWS or run the project locally to test the API. Simply use the Swagger UI interface to test each functionality:

1. **Access Swagger UI**: 
   After deployment, Swagger UI will be available at:  
   `https://your-api-gateway-url/swagger`

   (Replace `your-api-gateway-url` with the actual URL of your deployed API Gateway.)

2. **Test API Endpoints**: 
   Use the Swagger UI to test the following operations:
   - Fetch employee details (`GET`)
   - Add a new employee (`POST`)
   - Update employee information (`PUT`)
   - Delete an employee (`DELETE`)

## Swagger UI Overview

Swagger UI provides a web-based interface where you can:
- Test API endpoints by making GET, POST, PUT, and DELETE requests.
- View request/response details in real time.
- Explore the structure of each API endpoint.

You can access Swagger UI through the provided URL once the API Gateway is deployed.

## Unit Testing

This project includes unit tests to ensure the reliability of the Employee API. The unit tests are written using:
- **xUnit**: A testing framework for .NET projects.
- **Moq**: A popular library for creating mock objects.
