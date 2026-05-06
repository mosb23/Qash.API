using Qash.API.Domain.Enums;

namespace Qash.API.Infrastructure.Scheduling;

public static class RecurringScheduleCalculator
{
    public static DateTime GetNextOccurrenceUtc(DateTime anchorUtc, RecurringFrequency frequency)
    {
        return frequency switch
        {
            RecurringFrequency.Daily => anchorUtc.AddDays(1),
            RecurringFrequency.Weekly => anchorUtc.AddDays(7),
            RecurringFrequency.Monthly => anchorUtc.AddMonths(1),
            RecurringFrequency.Yearly => anchorUtc.AddYears(1),
            _ => anchorUtc.AddMonths(1)
        };
    }
}
