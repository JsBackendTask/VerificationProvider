using Microsoft.AspNetCore.Http;
using VerificationProvider.Models;

namespace VerificationProvider.Interfaces
{
    public interface IValidationVerificationService
    {
        Task<ValidateRequest> UnpackValidateRequestAsync(HttpRequest req);
        Task<bool> ValidateCodeAsync(ValidateRequest validateRequest);
    }
}