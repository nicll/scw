using System;

namespace ScwSvc.Procedures.Interfaces;

[Flags]
public enum TableQuery
{
    DataSet,
    Sheet,
    Own,
    Collaborations
}
