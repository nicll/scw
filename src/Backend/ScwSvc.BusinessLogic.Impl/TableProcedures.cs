namespace ScwSvc.Procedures.Impl;

public class TableProcedures : ITableProcedures
{
    /// <summary>
    /// Maximum amount of data sets one user may own at any time.
    /// </summary>
    /// <remarks>
    /// This value is bypassed if additional tables are assigned by an administrator.
    /// </remarks>
    public const int MaxDataSetsPerUser = 20;

    /// <summary>
    /// Maximum amount of sheets one user may own at any time.
    /// </summary>
    /// <remarks>
    /// This value is bypassed if additional tables are assigned by an administrator.
    /// </remarks>
    public const int MaxSheetsPerUser = 20;
}
