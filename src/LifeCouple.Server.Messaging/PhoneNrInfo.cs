using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LifeCouple.Server.Messaging
{
    public class PhoneNrInfo
    {
        public string CountryCode { get; set; }
        public string NationalFormat { get; set; }
        public string PhoneNr { get; set; }
        public TypeOfNumberEnum TypeOfNumber { get; set; }
        public bool IsValid { get; internal set; }

        public enum TypeOfNumberEnum
        {
            Unknown = 0,
            Other = 1,
            Mobile = 2,
        }
    }
}
