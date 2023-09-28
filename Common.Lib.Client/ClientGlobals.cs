using Common.Lib.Authentication;
using Common.Lib.Core.Context;
using System.Collections.Generic;
using System;
//using Common.Lib.Babylon;

namespace Common.Lib.Client
{
    public class ClientGlobals
    {
        public static Dictionary<int, Tuple<string, string>> LanguageInfo { get; set; } = new Dictionary<int, Tuple<string, string>>()
        {
            //{LanguageTypes.Spanish, new Tuple<string, string>("Spanish", "img/flags/es.svg")},
            //{LanguageTypes.English, new Tuple<string, string>("English", "img/flags/us.svg")},
            //{LanguageTypes.Catalan, new Tuple<string, string>("Català", "img/flags/cat.png")},
        };

        //public ClientInfo ClientInfo { get; set; }

        public User LogonUser { get; set; }

        public string CurrentHost { get; set; }
    }
}
