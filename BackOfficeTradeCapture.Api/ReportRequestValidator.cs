namespace BackOfficeTradeCapture.Api;

public static class ReportRequestValidator
{
    private static readonly DateOnly MinAllowedDate = new(2000, 1, 1);

    public static Dictionary<string, string[]> Validate(
        string? fromRaw, bool fromParsed, DateOnly fromDate,
        string? toRaw, bool toParsed, DateOnly toDate)
    {
        var errors = new Dictionary<string, string[]>();
        var today = DateOnly.FromDateTime(DateTime.UtcNow);

        if (string.IsNullOrWhiteSpace(fromRaw))
            errors.Add("from", ["'from' date is required."]);
        else if (!fromParsed)
            errors.Add("from", ["'from' must be a valid date in yyyy-MM-dd format."]);

        if (string.IsNullOrWhiteSpace(toRaw))
            errors.Add("to", ["'to' date is required."]);
        else if (!toParsed)
            errors.Add("to", ["'to' must be a valid date in yyyy-MM-dd format."]);

        if (errors.Count > 0) return errors;

        if (fromDate < MinAllowedDate)
            errors.Add("from", [$"Date cannot be earlier than {MinAllowedDate:yyyy-MM-dd}."]);

        if (toDate > today)
            errors.Add("to", ["Date cannot be in the future."]);

        if (fromDate > toDate)
            errors.Add("from", ["'from' must be on or before 'to'."]);

        if (toDate.DayNumber - fromDate.DayNumber > 365)
            errors.Add("range", ["Date range cannot exceed 365 days."]);

        return errors;
    }

}