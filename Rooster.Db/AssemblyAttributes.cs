using Nestor.Db.LiteDb.Models;
using Rooster.Contract.Models;

[assembly: LiteDb(typeof(AlarmEntity), nameof(AlarmEntity.Id), false)]
[assembly: LiteDbSourceEntity(typeof(AlarmEntity), nameof(AlarmEntity.Id))]
