using Nestor.Db.LiteDb.Models;
using Nestor.Db.Models;
using Rooster.Contract.Models;

[assembly: Ado(typeof(AlarmEntity), nameof(AlarmEntity.Id), false)]
[assembly: AdoSourceEntity(typeof(AlarmEntity), nameof(AlarmEntity.Id))]
[assembly: LiteDb(typeof(AlarmEntity), nameof(AlarmEntity.Id), false)]
[assembly: LiteDbSourceEntity(typeof(AlarmEntity), nameof(AlarmEntity.Id))]
