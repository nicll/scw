using System;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;

namespace ScwSvc.Models
{
    /// <summary>
    /// Lists different roles users can have.
    /// </summary>
    [JsonConverter(typeof(StringEnumConverter))]
    public enum UserRole
    {
        /// <summary>
        /// A regular user with no special permissions.
        /// </summary>
        Common,

        /// <summary>
        /// A trusted user, basically an admin with readonly rights.
        /// Users of this kind may view any user's table.
        /// </summary>
        Manager,

        /// <summary>
        /// An administrator.
        /// Users of this kind may edit any user's tables.
        /// </summary>
        Admin
    }
}
