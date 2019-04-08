using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace LifeCouple.WebApi.Common
{
    public class JWTModel
    {
        public string Token { get; set; }
        public DateTime Expires { get; set; }
    }
}
