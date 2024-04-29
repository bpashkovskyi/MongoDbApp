using Microsoft.AspNetCore.Mvc;

namespace MongoDbApp.IntegrationTests;

public abstract class ControllerTestsBase
{
    protected T? GetOkResultModel<T>(IActionResult actionResult)
        where T : class
    {

        Assert.IsInstanceOfType<OkObjectResult>(actionResult);

        var okObjectResult = (OkObjectResult)actionResult;

        Assert.IsInstanceOfType<T>(okObjectResult.Value);

        return okObjectResult.Value as T;
    }
}