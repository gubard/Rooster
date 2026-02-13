using Gaia.Models;

namespace Rooster.Contract.Models;

public sealed class AlarmEntity : IId<Guid>
{
    public Guid Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public DateTimeOffset DueDateTime { get; set; }
    public bool IsCompleted { get; set; }
}
