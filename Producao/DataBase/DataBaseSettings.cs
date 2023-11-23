using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Producao
{
    public sealed class DataBaseSettings
    {
        private static readonly DataBaseSettings instance = new DataBaseSettings();
        public string? Host { get; set; }
        public string? Database { get; set; }
        public string? Username { get; set; }
        public string? Password { get; set; }

        public static DataBaseSettings Instance => DataBaseSettings.instance;
    }
}
