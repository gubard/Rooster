using System.Collections.Frozen;
using Nestor.Db.Helpers;
using Rooster.Contract.Helpers;
using Rooster.Contract.Models;
using Rooster.Contract.Services;
using Zeus.Helpers;

InsertHelper.AddDefaultInsert(
    nameof(AlarmEntity),
    i => new AlarmEntity[] { new() { Id = i } }.CreateInsertQuery()
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
        AlarmDbService,
        RoosterGetRequest,
        RoosterPostRequest,
        RoosterGetResponse,
        RoosterPostResponse
    >(migration.ToFrozenDictionary(), RoosterJsonContext.Default.Options, "Rooster");
