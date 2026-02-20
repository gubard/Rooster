using System.Runtime.CompilerServices;
using Gaia.Models;
using Gaia.Services;
using Nestor.Db.Helpers;
using Nestor.Db.Models;
using Nestor.Db.Services;
using Rooster.Contract.Helpers;
using Rooster.Contract.Models;
using Rooster.Contract.Services;

namespace Rooster.Db.Services;

public sealed class AlarmDbService
    : DbService<RoosterGetRequest, RoosterPostRequest, RoosterGetResponse, RoosterPostResponse>,
        IAlarmDbService,
        IAlarmDbCache
{
    public AlarmDbService(
        IDbConnectionFactory factory,
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

    protected override ConfiguredValueTaskAwaitable ExecuteAsync(
        Guid idempotentId,
        RoosterPostResponse response,
        RoosterPostRequest request,
        CancellationToken ct
    )
    {
        return ExecuteCore(idempotentId, response, request, ct).ConfigureAwait(false);
    }

    public ConfiguredValueTaskAwaitable UpdateAsync(RoosterPostRequest source, CancellationToken ct)
    {
        return UpdateCore(source, ct).ConfigureAwait(false);
    }

    public ConfiguredValueTaskAwaitable UpdateAsync(RoosterGetResponse source, CancellationToken ct)
    {
        return UpdateCore(source, ct).ConfigureAwait(false);
    }

    private readonly IFactory<DbValues> _dbValuesFactory;
    private readonly IFactory<DbServiceOptions> _factoryOptions;

    private async ValueTask UpdateCore(RoosterGetResponse source, CancellationToken ct)
    {
        await using var session = await Factory.CreateSessionAsync(ct);
        var entities = source.Alarms.Select(x => x.ToAlarmEntity()).ToArray();
        var exists = await session.IsExistsAsync(entities, ct);
        var inserts = entities.Where(x => !exists.Contains(x.Id)).ToArray();
        var allIds = entities.Select(x => x.Id).ToArray();

        var deleteIds = await session.GetGuidAsync(
            new(
                AlarmsExt.SelectIdsQuery + $" WHERE Id NOT IN ({allIds.ToParameterNames("Id")})",
                allIds.ToQueryParameters("Id")
            ),
            ct
        );

        var updateQueries = entities
            .Where(x => exists.Contains(x.Id))
            .Select(x => x.CreateUpdateAlarmsQuery())
            .ToArray();

        if (inserts.Length != 0)
        {
            await session.ExecuteNonQueryAsync(inserts.CreateInsertQuery(), ct);
        }

        foreach (var query in updateQueries)
        {
            await session.ExecuteNonQueryAsync(query, ct);
        }

        if (deleteIds.Length != 0)
        {
            await session.ExecuteNonQueryAsync(deleteIds.CreateDeleteAlarmsQuery(), ct);
        }

        await session.CommitAsync(ct);
    }

    private async ValueTask UpdateCore(RoosterPostRequest source, CancellationToken ct)
    {
        await PostAsync(Guid.NewGuid(), source, ct);
    }

    private async ValueTask ExecuteCore(
        Guid idempotentId,
        RoosterPostResponse response,
        RoosterPostRequest request,
        CancellationToken ct
    )
    {
        var dbValues = _dbValuesFactory.Create();
        var options = _factoryOptions.Create();
        await using var session = await Factory.CreateSessionAsync(ct);

        await session.AddEntitiesAsync(
            dbValues.UserId.ToString(),
            idempotentId,
            options.IsUseEvents,
            request.Creates.Select(x => x.ToAlarmEntity()).ToArray(),
            ct
        );

        await session.EditEntitiesAsync(
            dbValues.UserId.ToString(),
            idempotentId,
            options.IsUseEvents,
            request.Edits.SelectMany(x => x.ToEditAlarmEntities()).ToArray(),
            ct
        );

        await session.DeleteEntitiesAsync(
            dbValues.UserId.ToString(),
            idempotentId,
            options.IsUseEvents,
            request.DeleteIds,
            ct
        );

        await session.CommitAsync(ct);
    }

    private async ValueTask<RoosterGetResponse> GetCore(
        RoosterGetRequest request,
        CancellationToken ct
    )
    {
        await using var session = await Factory.CreateSessionAsync(ct);
        var response = new RoosterGetResponse();
        var alarms = await session.GetAlarmsAsync(ct);

        if (request.IsGetAlarms)
        {
            response.Alarms = alarms.Select(x => x.ToAlarm()).ToArray();
        }

        return response;
    }
}
