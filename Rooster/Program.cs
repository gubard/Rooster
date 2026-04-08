using System.Collections.Frozen;
using Nestor.Db.Helpers;
using Nestor.Db.LiteDb.Helpers;
using Rooster.Contract.Helpers;
using Rooster.Contract.Models;
using Rooster.Contract.Services;
using Rooster.Db.Services;
using Zeus.Helpers;

DefaultBsonDocument.AddDefaultBsonDocument(
    nameof(AlarmEntity),
    i => new AlarmEntity { Id = i }.ToBsonDocument()
);

var migration = new Dictionary<int, string>();

foreach (var (key, value) in SqliteMigration.Migrations)
{
    migration.Add(key, value);
}

foreach (var (key, value) in RoosterMigration.Migrations)
{
    migration.Add(key, value);
}

foreach (var (key, value) in IdempotenceMigration.Migrations)
{
    migration.Add(key, value);
}

await WebApplication
    .CreateBuilder(args)
    .CreateAndRunZeusApp<
        IAlarmService,
        AlarmLiteDbService,
        RoosterGetRequest,
        RoosterPostRequest,
        RoosterGetResponse,
        RoosterPostResponse
    >(
        migration.ToFrozenDictionary(),
        "Rooster",
        builder => builder.Services.AddSingleton(RoosterJsonContext.Default.Options)
    );
