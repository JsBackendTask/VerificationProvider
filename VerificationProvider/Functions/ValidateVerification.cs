using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.Functions.Worker;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using VerificationProvider.Data.Contexts;
using VerificationProvider.Interfaces;
using VerificationProvider.Models;

namespace VerificationProvider.Functions
{
    public class ValidateVerification(ILogger<ValidateVerification> logger, IValidationVerificationService validationVerificationService)
    {
        private readonly ILogger<ValidateVerification> _logger = logger;
        private readonly IValidationVerificationService _validationVerificationService = validationVerificationService;


        [Function("ValidateVerification")]
        public async Task<IActionResult> Run([HttpTrigger(AuthorizationLevel.Function, "post", Route = "verification")] HttpRequest req)
        {
            try
            {
                var validateRequest = await _validationVerificationService.UnpackValidateRequestAsync(req);
                if (validateRequest != null)
                {
                    var validateResult = await _validationVerificationService.ValidateCodeAsync(validateRequest);
                    if (validateResult)
                    {
                        return new OkResult();
                    }
                }
            }
            catch (Exception ex)
            {
                _logger.LogError($"ERROR : ValidateVerification.Run() :: {ex.Message}");
            }

            return new UnauthorizedResult();
        }

    }
}
