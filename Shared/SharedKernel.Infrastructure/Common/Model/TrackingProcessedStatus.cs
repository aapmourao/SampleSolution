namespace SharedKernel.Infrastructure.Common.Model
{
    public record TrackingProcessedStatus
    {
        public Guid Id = Guid.NewGuid();
        public DateTime CreatedAtUtc = DateTime.UtcNow;
        public DateTime? ProcessedAtUtc = null;

        public string? ProcessedMessage = null;
        public bool? ProcessedResult = null;

        public void ProcessedWithError(string message)
        {
            ProcessedMessage = message;
            Processed(false);
        }
        public void ProcessedWithSuccess(string message)
        {
            ProcessedMessage = message;
            Processed(true);
        }

        private void Processed(bool result)
        {
            ProcessedResult = result;
            ProcessedAtUtc = DateTime.UtcNow;
        }
    }
}