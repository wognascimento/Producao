namespace Producao
{
    public sealed class DataBaseSettings
    {
        private static readonly DataBaseSettings instance = new();
        public string? Host { get; set; }
        public string? Database { get; set; }
        public string? Username { get; set; }
        public string? Password { get; set; }

        public string connectionString { get; set; } //=> $"Host={Host};Database={Database};Username={Username};Password={Password}";

        public static DataBaseSettings Instance => DataBaseSettings.instance;
    }
}
