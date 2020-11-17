using System;
using static ScwSvc.Utils;

namespace ScwSvc
{
    /// <summary>
    /// Contains global variables used within this application.
    /// </summary>
    internal static class Globals
    {
        /// <summary>
        /// Contains globals variables for building the connection string for the database.
        /// </summary>
        internal static class DbConnectionString
        {
            public static readonly string Server = GetEnvironmentVariableOrNull("SCW1_DBSERVER") ?? "127.0.0.1";

            public static readonly string Port = GetEnvironmentVariableOrNull("SCW1_DBPORT") ?? "5432";

            public static readonly string User = GetEnvironmentVariableOrFail("SCW1_DBUSER");

            public static readonly string Pass = GetEnvironmentVariableOrFail("SCW1_DBPASS");
        }
    }
}
