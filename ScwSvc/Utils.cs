using System;

namespace ScwSvc
{
    internal static class Utils
    {
        internal static string GetEnvironmentVariableOrFail(string name)
            => Environment.GetEnvironmentVariable(name) ?? throw new ArgumentException("Environment variable '" + name + "' not set.");

        internal static string? GetEnvironmentVariableOrNull(string name)
            => Environment.GetEnvironmentVariable(name);
    }
}
