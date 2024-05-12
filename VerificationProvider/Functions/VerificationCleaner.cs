using System;
using Microsoft.Azure.Functions.Worker;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using VerificationProvider.Data.Contexts;
using VerificationProvider.Interfaces;
using VerificationProvider.Models;
using VerificationProvider.Services;

namespace VerificationProvider.Functions
{
    public class VerificationCleaner(ILogger<VerificationCleaner> logger, ICleanerService cleanerService)
    {
        private readonly ILogger<VerificationCleaner> _logger = logger;
        private readonly ICleanerService _cleanerService = cleanerService;

        [Function("VerificationCleaner")]
        public async Task Run([TimerTrigger("0 */1 * * * *")] TimerInfo myTimer)
        {
            try
            {
                await _cleanerService.RemoveExpiredRecordsAsync();
            }
            catch (Exception ex)
            {
                _logger.LogError($"ERROR : VerificationCleaner.Run() :: {ex.Message}");
            }
        }
    }
}
