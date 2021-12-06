using System;

namespace ScwSvc.BusinessLogic.Interfaces;

[Flags]
public enum TableQuery
{
    DataSet,
    Sheet,
    Own,
    Collaborations
}
