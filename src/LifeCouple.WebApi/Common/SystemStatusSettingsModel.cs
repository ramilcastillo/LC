using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace LifeCouple.WebApi.Common
{
    /// <summary>
    /* AppSettings.json sample:
  "SystemStatusAccessKeys": {
    "ClientKeys": [
      {
        "Client": "Client1",
        "Key": "clientOnekey"
      },
      {
        "Client": "Client2",
        "Key": "clientTwokey"
      }
    ]
  }
  */
    /// </summary>
    public class SystemStatusSettingsModel
    {
        public List<Keys> ClientKeys { get; set; }

        public bool IsValidKey(string key)
        {
            return (!string.IsNullOrWhiteSpace(key) && ClientKeys?.SingleOrDefault(e => e.Key == key) != null);
        }

        public class Keys
        {
            public string Client { get; set; }
            public string Key { get; set; }
        }

        internal bool IsValid()
        {
            return (ClientKeys?.Count > 0);
        }
    }
}
