using Azure.Messaging.ServiceBus;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using VerificationProvider.Data.Contexts;
using VerificationProvider.Data.Entities;
using VerificationProvider.Models;

namespace VerificationProvider.Services;

public class VerificationService(ILogger<VerificationService> logger, IServiceProvider serviceProvider) : IVerificationService
{
    private readonly ILogger<VerificationService> _logger = logger;
    private readonly IServiceProvider _serviceProvider = serviceProvider;

    public VerificationRequest UnpackVerificationRequest(ServiceBusReceivedMessage message)
    {
        try
        {
            var verficationRequest = JsonConvert.DeserializeObject<VerificationRequest>(message.Body.ToString());
            if (verficationRequest != null && !string.IsNullOrEmpty(verficationRequest.Email))
                return verficationRequest;
        }
        catch (Exception ex)
        {
            _logger.LogError($"ERROR : VerificationService.UnpackVerificationRequest() :: {ex.Message}");
        }

        return null!;
    }
    public string GenerateCode()
    {
        try
        {
            var rnd = new Random();
            var code = rnd.Next(10000, 99999);

            return code.ToString();
        }
        catch (Exception ex)
        {
            _logger.LogError($"ERROR : VerificationService.GenerateVerificationCode() :: {ex.Message}");
        }

        return null!;
    }
    public async Task<bool> SaveVerificationRequest(VerificationRequest verficationToken, string code)
    {
        try
        {
            using var context = _serviceProvider.GetRequiredService<DataContext>();

            var existingRequest = await context.VerificationRequests.FirstOrDefaultAsync(x => x.Email == verficationToken.Email);
            if (existingRequest != null)
            {
                existingRequest.Code = code;
                existingRequest.ExpiryDate = DateTime.Now.AddMinutes(5);
                context.Entry(existingRequest).State = EntityState.Modified;
            }
            else
            {
                context.VerificationRequests.Add(new Data.Entities.VerificationRequestEntity()
                {
                    Email = verficationToken.Email,
                    Code = code
                });

                await context.SaveChangesAsync();
                return true;
            }
        }

        catch (Exception ex)
        {
            _logger.LogError($"ERROR : VerificationService.SaveVerificationRequest() :: {ex.Message}");
        }

        return false;
    }

    public EmailRequest GeneratedEmailRequest(VerificationRequest verficationRequest, string code)
    {
        try
        {
            if (!string.IsNullOrEmpty(verficationRequest.Email) && !string.IsNullOrEmpty(code))
            {
                var emailRequest = new EmailRequest
                {
                    To = verficationRequest.Email,
                    Subject = $"Verification Code {code}",
                    HtmlBody = $@"<html lang='en'>
                                    <head>
                                        <meta charset='utf-8' />
                                        <meta name='viewport' content='width=device-width, initial-scale=1.0' />
                                        <title>Verification Code</title>
                                    </head>

                                    <body>
                                        <div style='color: #191919; max-width:500px'>
                                            <div style='background-color: #6366F1; color: white; text-align: center; padding: 20px 0;'>
                                                <h1 style='font-weight: 400;'>Verification Code</h1>
                                            </div>
                                            <div style='background-color: #D4D7E5; padding: 1rem 2rem;'>
                                                <p>Hello User</p>
                                                <p>Thank you for signing up with us. Please use the following code to verify your email address.</p>
                                                <p style='font-weight: 700; text-align: center; font-size: 48px; letter-spacing:.3rem'>{code}</p>
                                                <div style='color:#191919; font-size: 12px'>
                                                    <p>If it wasn't you that signed up please ignore this message.</p>
                                                </div>
                                            </div>
                                            <div style='color: #191919; text-align:center; font-size: 11px'>
                                                <p>© 2024 Silicon. All rights reserved. Pulvinar urna condimentum amet tempor. Feugiat in odio non nunc ornare consectetur.</p>
                                            </div>
                                        </div>
                                    </body>        
                                </html>",
                    PlainText = $"Greetings, Your verification code is {code}. If it wasn't you that signed up please ignore this message."
                };

                return emailRequest;
            }
        }
        catch (Exception ex)
        {
            _logger.LogError($"ERROR : VerificationService.GeneratedEmailRequest() :: {ex.Message}");
        }

        return null!;
    }
    public string GenerateServiceBusEmailReques0t(EmailRequest emailRequest)
    {
        try
        {
            var payload = JsonConvert.SerializeObject(emailRequest);
            if (!string.IsNullOrEmpty(payload))
                return payload;
        }

        catch (Exception ex)
        {
            _logger.LogError($"ERROR : VerificationService.GenerateServiceBusEmailReques0t() :: {ex.Message}");
        }

        return null!;
    }
}
