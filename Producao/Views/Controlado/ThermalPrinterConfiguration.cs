using System;
using System.IO;
using System.Text.Json;

namespace Producao.Views.Controlado
{
    internal sealed class ThermalPrinterConfiguration
    {
        private const string FileName = "thermal-printer.json";

        public string IpAddress { get; set; } = "192.168.0.113";
        public int Port { get; set; } = 9100;

        private static string ConfigPath
        {
            get
            {
                var folder = Path.Combine(
                    Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData),
                    "SIG",
                    "Producao");

                Directory.CreateDirectory(folder);
                return Path.Combine(folder, FileName);
            }
        }

        public static ThermalPrinterConfiguration Load()
        {
            try
            {
                if (!File.Exists(ConfigPath))
                {
                    return new ThermalPrinterConfiguration();
                }

                var json = File.ReadAllText(ConfigPath);
                return JsonSerializer.Deserialize<ThermalPrinterConfiguration>(json) ?? new ThermalPrinterConfiguration();
            }
            catch
            {
                return new ThermalPrinterConfiguration();
            }
        }

        public void Save()
        {
            var json = JsonSerializer.Serialize(this, new JsonSerializerOptions { WriteIndented = true });
            File.WriteAllText(ConfigPath, json);
        }
    }
}
