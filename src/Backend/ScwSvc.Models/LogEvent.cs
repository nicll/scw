using System;

namespace ScwSvc.Models;

/// <summary>
/// Base type for logging events.
/// </summary>
public abstract class LogEvent
{
    /// <summary>
    /// ID of the event.
    /// </summary>
    public Guid Id { get; init; }

    /// <summary>
    /// Timestamp of when the event occurred.
    /// </summary>
    public DateTime Timestamp { get; init; }
}

/// <summary>
/// A user-related event.
/// </summary>
public class UserLogEvent : LogEvent
{
    /// <summary>
    /// User to associate this event with.
    /// </summary>
    public Guid UserId { get; init; }

    /// <summary>
    /// Action performed by the user.
    /// </summary>
    public UserLogEventType UserAction { get; init; }
}

/// <summary>
/// A lookup-related event.
/// </summary>
public class LookupLogEvent : LogEvent
{
    /// <summary>
    /// User that performed the lookup.
    /// </summary>
    public Guid UserId { get; init; }

    /// <summary>
    /// Table that was looked up.
    /// </summary>
    public Guid TableId { get; init; }
}

/// <summary>
/// A table-related event.
/// </summary>
public class TableLogEvent : LogEvent
{
    /// <summary>
    /// Table to associate this event with.
    /// </summary>
    public Guid TableId { get; init; }
}

/// <summary>
/// An event related to table metadata.
/// </summary>
public class TableDefinitionLogEvent : TableLogEvent
{
    /// <summary>
    /// Type of the table.
    /// </summary>
    public TableType TableType { get; init; }

    /// <summary>
    /// Action that was performed on the table.
    /// </summary>
    public TableDefinitionLogEventType TableAction { get; init; }
}

/// <summary>
/// An event related to table collaborators.
/// </summary>
public class TableCollaboratorLogEvent : TableLogEvent
{
    /// <summary>
    /// The ID of the collaborator.
    /// </summary>
    public Guid UserId { get; init; }

    /// <summary>
    /// Action that was performed on the collaborator.
    /// </summary>
    public TableCollaboratorLogEventType CollaboratorAction { get; init; }
}

/// <summary>
/// Kinds of events that can be performed on a user.
/// </summary>
public enum UserLogEventType
{
    CreateUser, DeleteUser, Login, Logout, ChangeName, ChangePassword, ChangeRole
}

/// <summary>
/// Kinds of events that can be performed on table metadata.
/// </summary>
public enum TableDefinitionLogEventType
{
    CreateTable, DeleteTable, AddColumn, RemoveColumn
}

/// <summary>
/// Kinds of events that can be performed on table collaborators.
/// </summary>
public enum TableCollaboratorLogEventType
{
    AddCollaborator, RemoveCollaborator
}

/// <summary>
/// Kinds of log event types.
/// </summary>
/// <remarks>
/// It is not to be assumed that existing values do not change when new ones are added.
/// As such it is important to use the names, as they are always up-to-date.
/// </remarks>
[Flags]
public enum LogEventType : ushort
{
    /// <summary>
    /// An invalid event type.
    /// </summary>
    Invalid = 0b00000000_00000000,

    /// <summary>
    /// A <see cref="UserLogEvent"/>.
    /// </summary>
    User = 0b00000000_00000001,

    /// <summary>
    /// A <see cref="LookupLogEvent"/>.
    /// </summary>
    Lookup = 0b00000000_00010000,

    /// <summary>
    /// A <see cref="TableDefinitionLogEvent"/>.
    /// </summary>
    TableDefinition = 0b00000001_00000000,

    /// <summary>
    /// A <see cref="TableCollaboratorLogEvent"/>.
    /// </summary>
    TableCollaboration = 0b00000010_00000000,

    /// <summary>
    /// Any <see cref="TableLogEvent"/>.
    /// </summary>
    Table = TableDefinition | TableCollaboration,

    /// <summary>
    /// Any <see cref="LogEvent"/>.
    /// A mask of all currently existing event types.
    /// </summary>
    All = User | Lookup | Table
}
