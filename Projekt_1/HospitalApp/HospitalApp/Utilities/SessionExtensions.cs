namespace HospitalApp.Utilities
{
    public static class SessionExtensions
    {
        public static T? GetObject<T>(this ISession session, string key)
        {
            if (session.TryGetValue(key, out var value))
            {
                var json = System.Text.Encoding.UTF8.GetString(value);
                return System.Text.Json.JsonSerializer.Deserialize<T>(json);
            }
            return default;
        }

        public static void SetObject<T>(this ISession session, string key, T value)
        {
            var json = System.Text.Json.JsonSerializer.Serialize(value);
            session.Set(key, System.Text.Encoding.UTF8.GetBytes(json));
        }
    }
}
