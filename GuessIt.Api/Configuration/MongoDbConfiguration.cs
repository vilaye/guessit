using GuessIt.Shared.Models;
using MongoDB.Bson;
using MongoDB.Bson.Serialization;
using MongoDB.Bson.Serialization.Serializers;

namespace GuessIt.Api.Configuration;

public static class MongoDbConfiguration
{
    public static void Configure()
    {
        BsonSerializer.RegisterSerializer(new GuidSerializer(GuidRepresentation.Standard));

        BsonClassMap.RegisterClassMap<GameSession>(cm =>
        {
            cm.AutoMap();
            cm.MapIdMember(c => c.SessionId);
            cm.MapMember(c => c.Duration)
              .SetSerializer(new TimeSpanSerializer(BsonType.String));
        });

        BsonClassMap.RegisterClassMap<GuessAttempt>(cm =>
        {
            cm.AutoMap();
            cm.SetIgnoreExtraElements(true);
        });
    }
}
