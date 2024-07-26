using System;
using System.IO;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;
using System.Security.Cryptography;
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
        public async Task<IActionResult> Run([HttpTrigger(AuthorizationLevel.Function, "post")] HttpRequest req)
        {
            _logger.LogInformation("C# HTTP trigger function processed a request.");

            // Read the request body
            string requestBody = await new StreamReader(req.Body).ReadToEndAsync();

            // Deserialize the request body to get key and message
            var data = JsonSerializer.Deserialize<Dictionary<string, string>>(requestBody);
            string key = data.ContainsKey("key") ? data["key"] : null;
            string message = data.ContainsKey("message") ? data["message"] : null;

            // Check if the key is empty
            if (string.IsNullOrEmpty(key))
            {
                return new BadRequestObjectResult(new { error = "Key cannot be empty." });
            }

            // Check if message is empty
            if (string.IsNullOrEmpty(message))
            {
                return new BadRequestObjectResult(new { error = "Message cannot be empty." });
            }

            // Generate HMAC token
            string hmacToken = CreateHmacToken(key, message);

            // Return the HMAC token as part of the response
            return new OkObjectResult(new { message = "Welcome to Azure Functions", hmacToken });
        }

        private string CreateHmacToken(string key, string message)
        {
            // Convert the key and message to byte arrays
            byte[] keyBytes = Encoding.UTF8.GetBytes(key);
            byte[] messageBytes = Encoding.UTF8.GetBytes(message);

            // Create an HMACSHA256 instance with the provided key
            using (var hmac = new HMACSHA256(keyBytes))
            {
                // Compute the HMAC for the message
                byte[] hashBytes = hmac.ComputeHash(messageBytes);

                // Convert the hash to a Base64 string
                string token = Convert.ToBase64String(hashBytes);

                return token;
            }
        }
    }
}
