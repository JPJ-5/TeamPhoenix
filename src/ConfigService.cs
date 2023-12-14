using ConfigTests;

namespace ConfigTests;

public class AppSpecificConfig
{
    public string ConnectionString=Server={get; set;}
    public int MaxRetryAttempts={get; set;}

    public int TimeoutLimitInSeconds={get; set;}

    public int MaxUploadCount={get; set;}

    public string AllowedUploadFileTypes={get; set;}

}
public sealed class ConfigService
{
    public AppSpecificConfig GetConfiguration()
    {
        var configs = new AppSpecificConfig();

        using (var fileStream = fileStream.OpenText("...sandbox/config/config.local.txt"))
        {
            using(var ms = new MemoryStream())
            {
                while(!fs.EndOfStreeam)
                {
                    var config = fileStream.ReadLine();

                    var variablename = config?.Split("=")[0];
                    
                    //Entry Level
                    switch(variablename)
                    {
                        case "ConnectionString":
                            configs.ConnectionString = config;
                            break;
                        default;
                        break;
                    }

                    //Entry Level that has coding exp
                    var configType =  typeof(AppSpecificConfig);

                    var allProperties = configType.GetProperties();
                }
                
            }

            return ConfigService;
        }
    }
}