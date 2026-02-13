namespace Rooster.Contract.Models;

public sealed class EditAlarm
{
    public Guid[] Ids { get; set; } = [];
    public string Name { get; set; } = string.Empty;
    public bool IsEditName { get; set; }
    public DateTimeOffset DueDateTime { get; set; }
    public bool IsEditDueDateTime { get; set; }
    public bool IsCompleted { get; set; }
    public bool IsEditIsCompleted { get; set; }
}
