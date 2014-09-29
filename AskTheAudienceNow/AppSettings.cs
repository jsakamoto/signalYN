using System.Configuration;

namespace AskTheAudienceNow
{
    [System.Diagnostics.DebuggerNonUserCodeAttribute]
    [System.Runtime.CompilerServices.CompilerGeneratedAttribute]
    public static class AppSettings
    {
        public static class Account
        {
            public static string Bitly
            {
                get { return ConfigurationManager.AppSettings["account.bitly"]; }
            }
        }
    }
}

