using System.Text.Json.Serialization;
using System.Text.Json.Serialization.Metadata;
using Gaia.Models;

namespace Rooster.Contract.Models;

[JsonSerializable(typeof(RoosterGetRequest))]
[JsonSerializable(typeof(RoosterPostRequest))]
[JsonSerializable(typeof(RoosterGetResponse))]
[JsonSerializable(typeof(RoosterPostResponse))]
[JsonSerializable(typeof(Alarm))]
[JsonSerializable(typeof(AlreadyExistsValidationError))]
[JsonSerializable(typeof(NotFoundValidationError))]
public partial class RoosterJsonContext : JsonSerializerContext
{
    public static readonly IJsonTypeInfoResolver Resolver;

    static RoosterJsonContext()
    {
        Resolver = Default.WithAddedModifier(typeInfo =>
        {
            if (typeInfo.Type == typeof(ValidationError))
            {
                typeInfo.PolymorphismOptions = new()
                {
                    TypeDiscriminatorPropertyName = "$type",
                    DerivedTypes =
                    {
                        new(
                            typeof(AlreadyExistsValidationError),
                            typeof(AlreadyExistsValidationError).FullName!
                        ),
                        new(
                            typeof(NotFoundValidationError),
                            typeof(NotFoundValidationError).FullName!
                        ),
                    },
                };
            }
        });
    }
}
