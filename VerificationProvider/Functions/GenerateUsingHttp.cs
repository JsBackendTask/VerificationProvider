using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.Functions.Worker;
using Microsoft.Extensions.Logging;
using VerificationProvider.Interfaces;

namespace VerificationProvider.Functions
{
    public class GenerateUsingHttp(ILogger<GenerateUsingHttp> logger, IVerificationService verificationService)
    {
        private readonly ILogger<GenerateUsingHttp> _logger = logger;
        private readonly IVerificationService _verificationService = verificationService;

        [Function("GenerateUsingHttp")]
        public IActionResult Run([HttpTrigger(AuthorizationLevel.Function, "post")] HttpRequest req)
        {
            try
            {
                var code = _verificationService.GenerateCode();
                return new OkObjectResult(code);
            }
            catch (Exception ex)
            {
                _logger.LogError($"ERROR : GenerateUsingHttp.Run() :: {ex.Message}");
                return new StatusCodeResult(StatusCodes.Status500InternalServerError);
            }
        }
    }
}
