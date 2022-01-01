using System;

namespace ScwSvc.Models;

[Flags]
public enum TableQuery : byte
{
    None = 0b00000000,

    DataSet = 0b00000001,
    Sheet = 0b00000010,
    TableTypeMask = 0b00001111,

    Own = 0b00010000,
    Collaborations = 0b00100000,
    TableRelationshipMask = 0b11110000
}
