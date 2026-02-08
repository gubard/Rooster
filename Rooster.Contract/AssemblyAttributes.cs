using Nestor.Db.Models;
using Rooster.Contract.Models;

[assembly: SqliteAdo(typeof(AlarmEntity), nameof(AlarmEntity.Id), false)]
[assembly: SourceEntity(typeof(AlarmEntity), nameof(AlarmEntity.Id))]
