using System;
using System.Collections.Generic;
using System.Text;

namespace Server.Config
{
    public class DbEntry
    {
        public string Name { get; set; }

        public string Host { get; set; }

        public int Port { get; set; }

        public string Key { get; set; }

        public int Retry { get; set; }

        public float RetryDelay { get; set; }

        public int ValidTime { get; set; }

        public string Type { get; set; }
    }

}
