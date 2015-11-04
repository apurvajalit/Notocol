using Microsoft.Web.WebPages.OAuth;

namespace Notocol
{
    public static class AuthConfig
    {
        public static void RegisterAuth()
        {
            OAuthWebSecurity.RegisterFacebookClient(
              appId: "875706079161883",
              appSecret: "32aef224208419b3144cf4c5bc3baf90");

            OAuthWebSecurity.RegisterClient(new GooglePlusClient("1001955814097-v1du4enggl2jl5o7n9v0qrjsvtspfn2c.apps.googleusercontent.com", "e2JUiPa9Qv0FFCji47aGOXYW"), "Google+", null);
        }
    }
}
