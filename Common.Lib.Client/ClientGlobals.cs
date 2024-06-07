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

        public static string AlternativeUserId { get; set; } = "System";

        public static string AlternativeUserEmail { get; set; } = "sys@sys.com";
        
        public string CurrentHost { get; set; }



        public static bool IsMaintenanceModeOn
        {
            get
            {
                return isMaintenanceModeOn;
            }
            set
            {
                isMaintenanceModeOn = value;
                SetViewToMaintenanceModeOnOff?.Invoke();

            }
        }
        static bool isMaintenanceModeOn;

        public static string MaintenanceModeMessage
        {
            get
            {
                return maintenanceModeMessage == string.Empty ?
                                                    "Modo mantenimiento espere por favor" :
                                                    maintenanceModeMessage;
            }
            set { maintenanceModeMessage = value; }
        }
        static string maintenanceModeMessage = string.Empty;


        public static Action? SetViewToMaintenanceModeOnOff { get; set; }
    }
}
