using Nestor.Db.Models;

namespace Rooster.Contract.Models;

public sealed class RoosterPostRequest : IPostRequest
{
    public EditAlarm[] Edits { get; set; } = [];
    public Guid[] DeleteIds { get; set; } = [];
    public Alarm[] Creates { get; set; } = [];
    public EventEntity[] Events { get; set; } = [];
}
