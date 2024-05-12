namespace VerificationProvider.Interfaces
{
    public interface ICleanerService
    {
        Task RemoveExpiredRecordsAsync();
    }
}