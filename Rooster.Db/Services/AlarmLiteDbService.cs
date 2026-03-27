using System.Runtime.CompilerServices;
using Gaia.Models;
using Gaia.Services;
using Nestor.Db.LiteDb.Services;
using Nestor.Db.Models;
using Rooster.Contract.Helpers;
using Rooster.Contract.Models;
using Rooster.Contract.Services;
using UltraLiteDB;

namespace Rooster.Db.Services;

public sealed class AlarmLiteDbService
    : LiteDbService<RoosterGetRequest, RoosterPostRequest, RoosterGetResponse, RoosterPostResponse>,
        IAlarmDbService,
        IAlarmDbCache
{
    public AlarmLiteDbService(
        IDatabaseFactory factory,
        IFactory<DbValues> dbValuesFactory,
        IFactory<DbServiceOptions> factoryOptions
    )
        : base(factory, nameof(AlarmEntity))
    {
        _dbValuesFactory = dbValuesFactory;
        _factoryOptions = factoryOptions;
    }

    public override ConfiguredValueTaskAwaitable<RoosterGetResponse> GetAsync(
        RoosterGetRequest request,
        CancellationToken ct
    )
    {
        return GetCore(request, ct).ConfigureAwait(false);
    }

    public ConfiguredValueTaskAwaitable UpdateAsync(RoosterPostRequest source, CancellationToken ct)
    {
        return UpdateCore(source, ct).ConfigureAwait(false);
    }

    public ConfiguredValueTaskAwaitable UpdateAsync(RoosterGetResponse source, CancellationToken ct)
    {
        return UpdateCore(source, ct).ConfigureAwait(false);
    }

    protected override ConfiguredValueTaskAwaitable ExecuteAsync(
        Guid idempotentId,
        RoosterPostResponse response,
        RoosterPostRequest request,
        CancellationToken ct
    )
    {
        return ExecuteCore(idempotentId, response, request, ct).ConfigureAwait(false);
    }

    private readonly IFactory<DbValues> _dbValuesFactory;
    private readonly IFactory<DbServiceOptions> _factoryOptions;

    private async ValueTask ExecuteCore(
        Guid idempotentId,
        RoosterPostResponse response,
        RoosterPostRequest request,
        CancellationToken ct
    )
    {
        var dbValues = _dbValuesFactory.Create();
        var options = _factoryOptions.Create();
        using var database = await Factory.CreateAsync(ct);

        database.AddEntities(
            dbValues.UserId.ToString(),
            idempotentId,
            options.IsUseEvents,
            request.Creates.Select(x => x.ToAlarmEntity()).ToArray()
        );

        database.EditEntities(
            dbValues.UserId.ToString(),
            idempotentId,
            options.IsUseEvents,
            request.Edits.SelectMany(x => x.ToEditAlarmEntities()).ToArray()
        );

        database.DeleteEntities(
            dbValues.UserId.ToString(),
            idempotentId,
            options.IsUseEvents,
            request.DeleteIds
        );

        await database.SaveChangesAsync(ct);
    }

    private async ValueTask UpdateCore(RoosterGetResponse source, CancellationToken ct)
    {
        using var database = await Factory.CreateAsync(ct);
        var collection = database.GetAlarmEntityCollection();
        var entities = source.Alarms.Select(x => x.ToAlarmEntity()).ToArray();

        var exists = entities
            .Where(x => collection.Exists(Query.EQ("_id", x.Id)))
            .Select(x => x.Id)
            .ToArray();

        var inserts = entities
            .Where(x => !exists.Contains(x.Id))
            .Select(x => x.ToBsonDocument())
            .ToArray();

        var allIds = entities.Select(x => x.Id).ToArray();

        var deleteIds = collection
            .Find(Query.Not(Query.In("_id", allIds.Select(x => new BsonValue(x)))))
            .Select(x => x["_id"])
            .ToArray();

        var updates = entities
            .Where(x => exists.Contains(x.Id))
            .Select(x => x.ToBsonDocument())
            .ToArray();

        if (inserts.Length != 0)
        {
            collection.Insert(inserts);
        }

        if (updates.Length != 0)
        {
            collection.Update(updates);
        }

        if (deleteIds.Length != 0)
        {
            collection.Delete(Query.In("_id", deleteIds));
        }

        await database.SaveChangesAsync(ct);
    }

    private async ValueTask UpdateCore(RoosterPostRequest source, CancellationToken ct)
    {
        await ExecuteAsync(Guid.NewGuid(), new(), source, ct);
    }

    private async ValueTask<RoosterGetResponse> GetCore(
        RoosterGetRequest request,
        CancellationToken ct
    )
    {
        using var database = await Factory.CreateAsync(ct);
        var collection = database.GetAlarmEntityCollection();
        var response = new RoosterGetResponse();
        var alarms = collection.FindAll().Select(x => x.ToAlarmEntity());

        if (request.IsGetAlarms)
        {
            response.Alarms = alarms.Select(x => x.ToAlarm()).ToArray();
        }

        return response;
    }
}
