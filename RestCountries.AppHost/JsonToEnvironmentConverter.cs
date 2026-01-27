using Microsoft.Extensions.Configuration;
using System.Text;

namespace RestCountries.AppHost;

public class JsonToEnvironmentConverter
{
    // code mainly coming from JsonToEnvironmentConverter by David Gardiner
    // https://github.com/flcdrg/JsonToEnvironmentConverter/blob/68272ce557d5b5b74f3f52181a1e9478f630d296/JsonToEnvironmentConverter/Pages/Index.cshtml.cs
    
    public static IDictionary<string, object> Convert(IConfigurationBuilder builder)
    {
        var environmentVariables = new Dictionary<string, object>();
        try
        {
            var configurationRoot = builder.Build();
            foreach ((string key, string value) in configurationRoot.AsEnumerable()
                .Where(pair => !string.IsNullOrEmpty(pair.Value))
                .OrderBy(pair => pair.Key))
            {
                environmentVariables.Add(key, value);
            }
        }
        catch (System.Text.Json.JsonException e)
        {
            //
        }

        return environmentVariables;
    }
}
