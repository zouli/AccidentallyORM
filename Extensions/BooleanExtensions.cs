namespace AccidentallyORM.Extensions
{
    public static class BooleanExtensions
    {
        public static string ToStringByFormat(this bool source, string format)
        {
            format += format.Contains(":") ? "" : ":";
            var formats = format.Split(new[] { ':' });
            return source ? formats[0] : formats[1];
        }
    }
}