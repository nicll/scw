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

            public static readonly string SysUser = GetEnvironmentVariableOrNull("SCW1_DBUSER_SYS") ?? "scw1_user_sys";

            public static readonly string SysPass = GetEnvironmentVariableOrFail("SCW1_DBPASS_SYS");

            public static readonly string DynUser = GetEnvironmentVariableOrNull("SCW1_DBUSER_DYN") ?? "scw1_user_dyn";

            public static readonly string DynPass = GetEnvironmentVariableOrFail("SCW1_DBPASS_DYN");
        }

        public static readonly string PostgrestBaseUrl = GetEnvironmentVariableOrFail("SCW1_PGREST_BASEURL");
    }
}
