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
            OAuthWebSecurity.RegisterClient(new GooglePlusClient("984532038251-4eia5k9oelbkcados4lq6k6cg20a7irh.apps.googleusercontent.com", "78ew4q_z4uR9_aC1eKImrTaO"), "Google+", null);
        }
    }
}
