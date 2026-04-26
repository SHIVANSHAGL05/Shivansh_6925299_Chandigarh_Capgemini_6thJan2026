using Microsoft.Azure.Functions.Worker;
using Microsoft.Azure.Functions.Worker.Http;
using System.Net;

namespace BookStoreFunctions;

public class GetBookStock
{
    [Function("GetBookStock")]
    public async Task<HttpResponseData> Run(
        [HttpTrigger(AuthorizationLevel.Function, "get")] HttpRequestData req)
    {
        var response = req.CreateResponse(HttpStatusCode.OK);
        await response.WriteAsJsonAsync(new { BookId = 1, Title = "Beyond Earth", Stock = 49 });
        return response;
    }
}
