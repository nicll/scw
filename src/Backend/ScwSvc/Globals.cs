using System;
using static ScwSvc.Utils.Configuration;

namespace ScwSvc
{
    /// <summary>
    /// Contains global variables and constants used within this application.
    /// </summary>
    internal static class Globals
    {
        /// <summary>
        /// Contains globals variables for building the connection string for the database.
        /// </summary>
        internal static class DbConnectionString
        {
            /// <summary>
            /// Specifies the hostname or IP address used for connecting to the database.
            /// </summary>
            public static readonly string Server = GetEnvironmentVariableOrNull("SCW1_DBSERVER") ?? "127.0.0.1";

            /// <summary>
            /// Specifies the port number used when connecting to the database.
            /// </summary>
            public static readonly string Port = GetEnvironmentVariableOrNull("SCW1_DBPORT") ?? "5432";

            /// <summary>
            /// Specifies the name of the SYS user when connecting to the database.
            /// </summary>
            public static readonly string SysUser = GetEnvironmentVariableOrNull("SCW1_DBUSER_SYS") ?? "scw1_user_sys";

            /// <summary>
            /// Specifies the password of the SYS user when connecting to the database.
            /// </summary>
            public static readonly string SysPass = GetEnvironmentVariableOrFail("SCW1_DBPASS_SYS");

            /// <summary>
            /// Specifies the name of the DYN user when connecting to the database.
            /// </summary>
            public static readonly string DynUser = GetEnvironmentVariableOrNull("SCW1_DBUSER_DYN") ?? "scw1_user_dyn";

            /// <summary>
            /// Specifies the password of the DYN user when connecting to the database.
            /// </summary>
            public static readonly string DynPass = GetEnvironmentVariableOrFail("SCW1_DBPASS_DYN");
        }

        /// <summary>
        /// Defines names of policies for authorization used across the application.
        /// </summary>
        internal static class Authorization
        {
            /// <summary>
            /// This functionality is available for managers only.
            /// </summary>
            public const string ManagerOnly = nameof(ManagerOnly);

            /// <summary>
            /// This functionality is available for administrators only.
            /// </summary>
            public const string AdminOnly = nameof(AdminOnly);

            /// <summary>
            /// This functionality is available for managers and administrators only.
            /// </summary>
            public const string ManagerOrAdminOnly = nameof(ManagerOrAdminOnly);
        }

        /// <summary>
        /// Specifies the base URL of the PostgREST server to redirect to.
        /// </summary>
        public static readonly string PostgrestBaseUrl = GetEnvironmentVariableOrFail("SCW1_PGREST_BASEURL");
    }
}
