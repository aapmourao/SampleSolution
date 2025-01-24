namespace SharedKernel.Infrastructure.EventualConsistency.Options;
    public class EventualConsistencyOptions
    {
        public static string Section = "EventualConsistency";
        public bool KeepHistoryEnabled { get; init; } = false;
        public int KeepHistoryInDays { get; init; } = 7;
    }
