// INKED MP2 | Shared helper: current time rounded to whole seconds (matches DATETIME2(0) columns)
// Owner: Rashel Maxine Baquirin (Billing module) - helper used by Orders, Invoices, Payments
namespace INKED.Application.Billing;

public static class DateTimeHelper
{
    public static DateTime NowToSecond()
    {
        var now = DateTime.Now;
        return new DateTime(now.Year, now.Month, now.Day, now.Hour, now.Minute, now.Second, now.Kind);
    }
}
