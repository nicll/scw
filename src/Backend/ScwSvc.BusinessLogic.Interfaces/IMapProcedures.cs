namespace ScwSvc.Procedures.Interfaces;

public interface IMapProcedures
{
    /// <summary>
    /// Converts a user ID to the respective user name.
    /// Alternatively returns <see langword="null"/> if not found.
    /// </summary>
    /// <param name="id">The ID of the user.</param>
    /// <returns>The name of the user or <see langword="null"/>.</returns>
    Task<string?> UserIdToName(Guid id);

    /// <summary>
    /// Converts a user name to the respective user ID.
    /// Alternatively returns <see langword="null"/> if not found.
    /// </summary>
    /// <param name="name">The name of the user.</param>
    /// <returns>The ID of the user or <see langword="null"/>.</returns>
    Task<Guid?> UserNameToId(string name);
}
