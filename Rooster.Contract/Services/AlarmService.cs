using Gaia.Services;
using Nestor.Db.Services;
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

public sealed class EmptyAlarmDbCache
    : EmptyDbCache<RoosterPostRequest, RoosterGetResponse>,
        IAlarmDbCache;

public sealed class EmptyAlarmDbService
    : EmptyDbService<
        RoosterGetRequest,
        RoosterPostRequest,
        RoosterGetResponse,
        RoosterPostResponse
    >,
        IAlarmDbService;
