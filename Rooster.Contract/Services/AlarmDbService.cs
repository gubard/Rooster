using System;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Threading;
using System.Threading.Tasks;
using Gaia.Helpers;
using Gaia.Models;
using Gaia.Services;
using Nestor.Db.Helpers;
using Nestor.Db.Models;
using Nestor.Db.Services;
using Rooster.Contract.Helpers;
using Rooster.Contract.Models;

namespace Rooster.Contract.Services;

public interface IAlarmService
    : IService<RoosterGetRequest, RoosterPostRequest, RoosterGetResponse, RoosterPostResponse>;

public interface IAlarmHttpService
    : IAlarmService,
        IHttpService<
            RoosterGetRequest,
            RoosterPostRequest,
            RoosterGetResponse,
            RoosterPostResponse
        >;

public interface IAlarmDbService
    : IAlarmService,
        IDbService<RoosterGetRequest, RoosterPostRequest, RoosterGetResponse, RoosterPostResponse>;

public interface IAlarmDbCache : IDbCache<RoosterPostRequest, RoosterGetResponse>;

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
        return TaskHelper.ConfiguredCompletedTask;
    }

    public ConfiguredValueTaskAwaitable UpdateAsync(RoosterGetResponse source, CancellationToken ct)
    {
        return TaskHelper.ConfiguredCompletedTask;
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
            request.Edits.Select(x => x.ToEditAlarmEntity()).ToArray(),
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
