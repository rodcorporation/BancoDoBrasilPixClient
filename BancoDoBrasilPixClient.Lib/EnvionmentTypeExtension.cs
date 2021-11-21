using System.Collections.Generic;

namespace BancoDoBrasilPixClient.Lib
{
    public sealed partial class PixClient
    {
        internal static class EnvironmentTypeExtension
        {
            internal static string GetOAuthUrl(EnvironmentType environmentType)
            {
                var oAuthEndpoints = new Dictionary<EnvironmentType, string>();
                oAuthEndpoints.Add(EnvironmentType.Testing, "https://oauth.hm.bb.com.br");
                oAuthEndpoints.Add(EnvironmentType.Production, "https://oauth.bb.com.br");

                return oAuthEndpoints[environmentType];
            }

            internal static string GetPixUrl(EnvironmentType environmentType)
            {
                var pixEndpoints = new Dictionary<EnvironmentType, string>();
                pixEndpoints.Add(EnvironmentType.Testing, "https://api.hm.bb.com.br");
                pixEndpoints.Add(EnvironmentType.Production, "https://api.bb.com.br");

                return pixEndpoints[environmentType];
            }
        }
    }
}
