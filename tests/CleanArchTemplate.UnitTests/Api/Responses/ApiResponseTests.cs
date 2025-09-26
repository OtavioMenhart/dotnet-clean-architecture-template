using AutoFixture;
using CleanArchTemplate.Api.Responses;

namespace CleanArchTemplate.UnitTests.Api.Responses
{
    public class ApiResponseTests
    {
        private readonly Fixture _fixture = new();

        [Fact]
        public void Constructor_SetsDataAndMessage()
        {
            var data = _fixture.Create<string>();
            var message = _fixture.Create<string>();

            var response = new ApiResponse<string>(data, message);

            Assert.True(response.Success);
            Assert.Equal(data, response.Data);
            Assert.Equal(message, response.Message);
        }

        [Fact]
        public void Constructor_SetsSuccessTrueByDefault()
        {
            var response = new ApiResponse<int>(42);

            Assert.True(response.Success);
        }

        [Fact]
        public void Constructor_AllowsNullDataAndMessage()
        {
            var response = new ApiResponse<object>(null, null);

            Assert.True(response.Success);
            Assert.Null(response.Data);
            Assert.Null(response.Message);
        }
    }
}