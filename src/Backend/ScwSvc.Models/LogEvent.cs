using System;

namespace ScwSvc.Models;

public abstract class LogEvent
{
    public Guid Id { get; init; }

    public DateTime Timestamp { get; init; }
}

public class UserLogEvent : LogEvent
{
    public Guid UserId { get; init; }

    public UserLogEventType UserAction { get; init; }
}

public class LookupLogEvent : LogEvent
{
    public Guid UserId { get; init; }

    public Guid TableId { get; init; }
}

public class TableLogEvent : LogEvent
{
    public Guid TableId { get; init; }
}

public class TableDefinitionLogEvent : TableLogEvent
{
    public TableType TableType { get; init; }

    public TableDefinitionLogEventType TableAction { get; init; }
}

public class TableCollaboratorLogEvent : TableLogEvent
{
    public Guid UserId { get; init; }

    public TableCollaboratorLogEventType CollaboratorAction { get; init; }
}

public enum UserLogEventType
{
    CreateUser, DeleteUser, Login, Logout, ChangeName, ChangePassword, ChangeRole
}

public enum TableDefinitionLogEventType
{
    CreateTable, DeleteTable, AddColumn, RemoveColumn
}

public enum TableCollaboratorLogEventType
{
    AddCollaborator, RemoveCollaborator
}

[Flags]
public enum LogEventType : ushort
{
    Invalid = 0b00000000_00000000,
    User = 0b00000000_00000001,
    Lookup = 0b00000000_00010000,
    TableDefinition = 0b00000001_00000000,
    TableCollaboration = 0b00000010_00000000,
    Table = TableDefinition | TableCollaboration,
    All = User | Lookup | Table
}
