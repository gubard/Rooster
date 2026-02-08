using System.Collections.Generic;
using Gaia.Models;
using Nestor.Db.Models;

namespace Rooster.Contract.Models;

public sealed class RoosterGetResponse : IResponse
{
    public Alarm[] Alarms { get; set; } = [];
    public List<ValidationError> ValidationErrors { get; } = [];
}
