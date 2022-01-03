using ScwSvc.Models;

namespace ScwSvc.Procedures.Interfaces;

public interface IGraphQLTableProcedures
{
    /// <summary>
    /// Get the lookup ID (<see cref="Table.LookupName"/>) of a data set.
    /// </summary>
    /// <param name="user">The owner of the data set.</param>
    /// <param name="tableId">The ID of the table.</param>
    /// <returns>The lookup ID.</returns>
    Task<Guid> GetDataSetLookupName(User user, Guid tableId);

    /// <summary>
    /// Get the lookup ID (<see cref="Table.LookupName"/>) of a sheet.
    /// </summary>
    /// <param name="user">The owner of the sheet.</param>
    /// <param name="tableId">The ID of the table.</param>
    /// <returns>The lookup ID.</returns>
    Task<Guid> GetSheetLookupName(User user, Guid tableId);
}
