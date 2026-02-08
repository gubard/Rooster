using System.Collections.Generic;
using Gaia.Models;
using Nestor.Db.Models;

namespace Rooster.Contract.Models;

public sealed class RoosterPostResponse : IPostResponse
{
    public List<ValidationError> ValidationErrors { get; } = [];
    public bool IsEventSaved { get; set; }
}
