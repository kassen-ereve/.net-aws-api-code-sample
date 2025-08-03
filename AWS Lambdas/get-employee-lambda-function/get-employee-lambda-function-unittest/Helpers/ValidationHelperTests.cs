using get_employee_lambda_function.Helpers;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace get_employee_lambda_function_unittest.Helpers
{
    public class ValidationHelperTests
    {
        [Theory]
        [InlineData("GET", "GET")]
        [InlineData("get", "GET")]
        [InlineData("POST", "post")]
        [InlineData("DeLeTe", "DELETE")]
        public void ValidateHTTPRequestMethod_ValidMethods_DoesNotThrow(string requestMethod, string expectedMethod)
        {
            // Act & Assert
            var exception = Record.Exception(() =>
                ValidationHelper.ValidateHTTPRequestMethod(requestMethod, expectedMethod));

            Assert.Null(exception);
        }

        [Theory]
        [InlineData("GET", "POST")]
        [InlineData("PUT", "DELETE")]
        [InlineData("PATCH", "GET")]
        public void ValidateHTTPRequestMethod_InvalidMethods_ThrowsException(string requestMethod, string expectedMethod)
        {
            // Act & Assert
            var ex = Assert.Throws<Exception>(() =>
                ValidationHelper.ValidateHTTPRequestMethod(requestMethod, expectedMethod));

            Assert.Contains("Incorrect AWS API configuration", ex.Message);
            Assert.Contains(expectedMethod, ex.Message);
            Assert.Contains(requestMethod, ex.Message);
        }

        [Fact]
        public void ValidateUUID_ShouldReturnNull_WhenValidUUID()
        {
            string validUuid = new string('a', 36);

            var result = ValidationHelper.ValidateUUID(validUuid, 36, "TestID");

            Assert.Null(result);
        }

        [Fact]
        public void ValidateUUID_ShouldReturnBadRequest_WhenUUIDContainsWhitespace()
        {
            string uuidWithSpace = "abc def";

            var result = ValidationHelper.ValidateUUID(uuidWithSpace, 7, "TestID");

            Assert.NotNull(result);
            Assert.Equal(400, result!.StatusCode);
            Assert.Contains("must not contain any whitespace", result.Body);
        }

        [Fact]
        public void ValidateUUID_ShouldReturnBadRequest_WhenUUIDIncorrectLength()
        {
            string shortUuid = new string('a', 10);

            var result = ValidationHelper.ValidateUUID(shortUuid, 36, "TestID");

            Assert.NotNull(result);
            Assert.Equal(400, result!.StatusCode);
            Assert.Contains("must be exactly 36 in length", result.Body);
        }

        [Fact]
        public void ValidateEmployeeID_ShouldReturnNull_WhenValid()
        {
            string validEmployeeID = new string('e', 73);
            var result = ValidationHelper.ValidateEmployeeID(validEmployeeID);

            Assert.Null(result);
        }

        [Fact]
        public void ValidateEmployeeID_ShouldReturnBadRequest_WhenInvalid()
        {
            string invalidEmployeeID = new string('x', 70); // Invalid length
            var result = ValidationHelper.ValidateEmployeeID(invalidEmployeeID);

            Assert.NotNull(result);
            Assert.Equal(400, result!.StatusCode);
            Assert.Contains("EmployeeID", result.Body);
        }

        [Fact]
        public void ValidateCompanyID_ShouldReturnNull_WhenValid()
        {
            string validCompanyID = new string('c', 36);
            var result = ValidationHelper.ValidateCompanyID(validCompanyID);

            Assert.Null(result);
        }

        [Fact]
        public void ValidateCompanyID_ShouldReturnBadRequest_WhenContainsWhitespace()
        {
            string companyIDWithSpace = "abc def ghi jkl mno pqr stu vwx yz12 3456";
            var result = ValidationHelper.ValidateCompanyID(companyIDWithSpace);

            Assert.NotNull(result);
            Assert.Equal(400, result!.StatusCode);
            Assert.Contains("CompanyID", result.Body);
        }
    }
}
