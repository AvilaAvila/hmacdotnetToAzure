using System;
using System.IO;
using System.Text;
using System.Security.Cryptography;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.Functions.Worker;
using Microsoft.Extensions.Logging;
using System.Text.Json;

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
        public async Task<IActionResult> Run([HttpTrigger(AuthorizationLevel.Function, "get", "post")] HttpRequest req)
        {
            _logger.LogInformation("C# HTTP trigger function processed a request.");

            // Read key and message from the request
            string key = req.Query["key"];
            string message = req.Query["message"];

            if (string.IsNullOrEmpty(key))
            {
                return new BadRequestObjectResult(new { error = "Key cannot be empty." });
            }

            if (req.Method == HttpMethods.Post && string.IsNullOrEmpty(key) || string.IsNullOrEmpty(message))
            {
                using (var reader = new StreamReader(req.Body))
                {
                    var body = await reader.ReadToEndAsync();
                    var data = JsonSerializer.Deserialize<Dictionary<string, string>>(body);
                    key = data.ContainsKey("key") ? data["key"] : key;
                    message = data.ContainsKey("message") ? data["message"] : message;
                }
            }

            // Generate HMAC token
            string hmacToken = CreateHmacToken(key, message);

            // Return the HMAC token as part of the response
            return new OkObjectResult(new { message = "Welcome to Azure Functions this done by Shenll teams!", hmacToken });
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
