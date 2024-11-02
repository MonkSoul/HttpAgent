// 版权归百小僧及百签科技（广东）有限公司所有。
// 
// 此源代码遵循位于源代码树根目录中的 LICENSE 文件的许可证。

namespace HttpAgent.Tests;

public class ValidationDeclarativeExtractorTests
{
    [Fact]
    public void New_ReturnOK()
    {
        Assert.True(
            typeof(IHttpDeclarativeExtractor).IsAssignableFrom(typeof(ValidationDeclarativeExtractor)));

        var extractor = new ValidationDeclarativeExtractor();
        Assert.NotNull(extractor);
    }

    [Fact]
    public void Extract_ReturnOK()
    {
        var method1 =
            typeof(IValidationAttributeDeclarativeTest).GetMethod(nameof(IValidationAttributeDeclarativeTest.Test1))!;
        var httpRequestBuilder1 = HttpRequestBuilder.Get("http://localhost");

        var context1 = new HttpDeclarativeExtractorContext(method1, [null, null]);
        Assert.Throws<ValidationException>(() =>
            new ValidationDeclarativeExtractor().Extract(httpRequestBuilder1, context1));

        var context2 = new HttpDeclarativeExtractorContext(method1, ["furion", null]);
        Assert.Throws<ValidationException>(() =>
            new ValidationDeclarativeExtractor().Extract(httpRequestBuilder1, context2));

        var context3 =
            new HttpDeclarativeExtractorContext(method1, ["furion", new ValidationModel { Name = "furion" }]);
        new ValidationDeclarativeExtractor().Extract(httpRequestBuilder1, context3);
    }

    [Fact]
    public void ValidateParameter_Invalid_Parameter()
    {
        Assert.Throws<ArgumentNullException>(() =>
            ValidationDeclarativeExtractor.ValidateParameter(null!, null));

        var method1 =
            typeof(IValidationAttributeDeclarativeTest).GetMethod(nameof(IValidationAttributeDeclarativeTest.Test1))!;
        var parameters = method1.GetParameters();

        var exception = Assert.Throws<ValidationException>(() =>
        {
            ValidationDeclarativeExtractor.ValidateParameter(parameters[0], null);
        });
        Assert.Equal("The str field is required.", exception.Message);

        var exception2 = Assert.Throws<ValidationException>(() =>
        {
            ValidationDeclarativeExtractor.ValidateParameter(parameters[1], null);
        });
        Assert.Equal("The obj field is required.", exception2.Message);
    }

    [Fact]
    public void ValidateParameter_ReturnOK()
    {
        var method1 =
            typeof(IValidationAttributeDeclarativeTest).GetMethod(nameof(IValidationAttributeDeclarativeTest.Test2))!;
        var parameters = method1.GetParameters();
        ValidationDeclarativeExtractor.ValidateParameter(parameters[0], null);
        ValidationDeclarativeExtractor.ValidateParameter(parameters[1], null);

        var exception = Assert.Throws<ValidationException>(() =>
        {
            ValidationDeclarativeExtractor.ValidateParameter(parameters[1], new ValidationModel());
        });
        Assert.Equal("The Name field is required.", exception.Message);

        var exception2 = Assert.Throws<ValidationException>(() =>
        {
            ValidationDeclarativeExtractor.ValidateParameter(parameters[1],
                new ValidationModel { Name = "fu" });
        });
        Assert.Equal("The field Name must be a string or array type with a minimum length of '3'.", exception2.Message);

        var method2 =
            typeof(IValidationAttributeDeclarativeTest).GetMethod(nameof(IValidationAttributeDeclarativeTest.Test3))!;
        var parameters2 = method2.GetParameters();

        var exception3 = Assert.Throws<ValidationException>(() =>
        {
            ValidationDeclarativeExtractor.ValidateParameter(parameters2[0], null);
        });
        Assert.Equal("The str field is required.", exception3.Message);

        var exception31 = Assert.Throws<ValidationException>(() =>
        {
            ValidationDeclarativeExtractor.ValidateParameter(parameters2[0], "a");
        });
        Assert.Equal("The field str must be a string or array type with a minimum length of '2'.", exception31.Message);

        var exception32 = Assert.Throws<ValidationException>(() =>
        {
            ValidationDeclarativeExtractor.ValidateParameter(parameters2[0], "furion");
        });
        Assert.Equal("The field str must be a string or array type with a maximum length of '5'.", exception32.Message);

        ValidationDeclarativeExtractor.ValidateParameter(parameters2[0], "fur");

        var exception4 = Assert.Throws<ValidationException>(() =>
        {
            ValidationDeclarativeExtractor.ValidateParameter(parameters2[1], 12);
        });
        Assert.Equal("The field age must be between 0 and 10.", exception4.Message);

        var exception41 = Assert.Throws<ValidationException>(() =>
        {
            ValidationDeclarativeExtractor.ValidateParameter(parameters2[1], -1);
        });
        Assert.Equal("The field age must be between 0 and 10.", exception41.Message);

        ValidationDeclarativeExtractor.ValidateParameter(parameters2[1], 5);
    }
}

public interface IValidationAttributeDeclarativeTest : IHttpDeclarativeExtractor
{
    [Get("http://localhost:5000")]
    Task Test1([Required] string str, [Required] ValidationModel obj);

    [Get("http://localhost:5000")]
    Task Test2(string str, ValidationModel obj);

    [Get("http://localhost:5000")]
    Task Test3([Required, MinLength(2), MaxLength(5)] string str, [Range(0, 10)] int age);
}

public class ValidationModel
{
    public int Id { get; set; }

    [Required, MinLength(3)] public string? Name { get; set; }
}