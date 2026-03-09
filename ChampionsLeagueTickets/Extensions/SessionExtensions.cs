using Newtonsoft.Json;

namespace ChampionsLeagueTickets.Extensions;
public static class SessionExtensions{
    public static void SetObject(this ISession session, string key, object? value) {

        var settings = new JsonSerializerSettings {
            TypeNameHandling = TypeNameHandling.Auto
        };

        session.SetString(key, JsonConvert.SerializeObject(value, settings));
    }

    public static T? GetObject<T>(this ISession session, string key) {
        var value = session.GetString(key);

        var settings = new JsonSerializerSettings {
            TypeNameHandling = TypeNameHandling.Auto
        };

        return value == null ? default : JsonConvert.DeserializeObject<T>(value, settings);
        
    }

}

