using System;

namespace Rooster.Contract.Models;

public sealed class EditAlarm
{
    public Guid Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public bool IsEditName { get; set; }
    public DateTimeOffset DueDateTime { get; set; }
    public bool IsEditDueDateTime { get; set; }
}
