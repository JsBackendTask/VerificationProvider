using Azure.Messaging.ServiceBus;
using VerificationProvider.Data.Entities;
using VerificationProvider.Models;

namespace VerificationProvider.Interfaces
{
    public interface IVerificationService
    {
        string GenerateCode();
        EmailRequest GeneratedEmailRequest(VerificationRequest verficationRequest, string code);
        string GenerateServiceBusEmailReques0t(EmailRequest emailRequest);
        Task<bool> SaveVerificationRequest(VerificationRequest verficationToken, string code);
        VerificationRequest UnpackVerificationRequest(ServiceBusReceivedMessage message);
    }
}