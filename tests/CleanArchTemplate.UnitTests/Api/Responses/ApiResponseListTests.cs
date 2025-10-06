using AutoFixture;
using CleanArchTemplate.Api.Responses;

namespace CleanArchTemplate.UnitTests.Api.Responses;

public class ApiResponseListTests
{
    private readonly Fixture _fixture = new();

    [Fact]
    public void Constructor_SetsPropertiesCorrectly()
    {
        var data = _fixture.CreateMany<string>(5);
        var pageNumber = _fixture.Create<int>();
        var pageSize = _fixture.Create<int>();
        var totalCount = _fixture.Create<int>();
        var message = _fixture.Create<string>();

        var response = new ApiResponseList<string>(data, pageNumber, pageSize, totalCount, message);

        Assert.True(response.Success);
        Assert.Equal(data, response.Data);
        Assert.Equal(pageNumber, response.PageNumber);
        Assert.Equal(pageSize, response.PageSize);
        Assert.Equal(totalCount, response.TotalCount);
        Assert.Equal(message, response.Message);
    }

    [Theory]
    [InlineData(100, 10, 10)]
    [InlineData(101, 10, 11)]
    [InlineData(0, 10, 0)]
    [InlineData(10, 0, 0)]
    public void TotalPages_CalculatesCorrectly(int totalCount, int pageSize, int expectedPages)
    {
        var data = _fixture.CreateMany<string>(1);
        var response = new ApiResponseList<string>(data, 1, pageSize, totalCount);

        Assert.Equal(expectedPages, response.TotalPages);
    }

    [Fact]
    public void Constructor_AllowsNullDataAndMessage()
    {
        var response = new ApiResponseList<object>(null, 1, 10, 0, null);

        Assert.True(response.Success);
        Assert.Null(response.Data);
        Assert.Null(response.Message);
    }
}