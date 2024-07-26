using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.Functions.Worker;
using Microsoft.Extensions.Logging;

namespace Company.Function
{
    public class hmacshenlldot
    {
        private readonly ILogger<hmacshenlldot> _logger;

        public hmacshenlldot(ILogger<hmacshenlldot> logger)
        {
            _logger = logger;
        }

        [Function("hmacshenlldot")]
        public IActionResult Run([HttpTrigger(AuthorizationLevel.Function, "get", "post")] HttpRequest req)
        {
            _logger.LogInformation("C# HTTP trigger function processed a request.");
            return new OkObjectResult("Welcome to Azure Functions this done by Shenll teams!");
        }
    }
}
