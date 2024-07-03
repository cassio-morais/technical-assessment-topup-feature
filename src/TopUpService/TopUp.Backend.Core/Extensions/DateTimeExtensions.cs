namespace Backend.TopUp.Core.Extensions
{
    public class DateTimeExtensions : IDateTimeExtensions
    {
        public DateTimeOffset NewDateUtc(DateTime date) => DateTime.SpecifyKind(date, DateTimeKind.Utc);

        public DateTimeOffset NowUtc() => DateTime.SpecifyKind(DateTime.Now, DateTimeKind.Utc);
    }

    public interface IDateTimeExtensions
    {
        DateTimeOffset NowUtc();

        DateTimeOffset NewDateUtc(DateTime date);
    }
}
