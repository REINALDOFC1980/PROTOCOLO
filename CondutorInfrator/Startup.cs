using Microsoft.Owin;
using Microsoft.Owin.Security.Cookies;
using Owin;
using System;

[assembly: OwinStartup(typeof(CondutorInfrator.Startup))]

namespace CondutorInfrator
{
    public class Startup
    {
        public void Configuration(IAppBuilder app)
        {
            app.UseCookieAuthentication(new CookieAuthenticationOptions
            {
                AuthenticationType = "ApplicationCookie",
                LoginPath = new PathString("/Pessoa/sessaoexpirada"),
                ExpireTimeSpan = TimeSpan.FromMinutes(120),
                SlidingExpiration = true
            });
        }
    }
}