using IdentityServer4;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.IdentityModel.Tokens;

namespace IdentityServer
{
   public class Startup
   {
      // This method gets called by the runtime. Use this method to add services to the container.
      // For more information on how to configure your application, visit https://go.microsoft.com/fwlink/?LinkID=398940
      public void ConfigureServices(IServiceCollection services)
      {
         services.AddMvc();

         services.AddIdentityServer()
            .AddDeveloperSigningCredential()
            .AddInMemoryIdentityResources(Config.GetIdentityResources())
            .AddInMemoryApiResources(Config.GetApiResources())
            .AddInMemoryClients(Config.GetClients())
            .AddTestUsers(Config.GetUsers());

         services.AddAuthentication()
            .AddGoogle("Google", options =>
            {
               options.SignInScheme = IdentityServerConstants.ExternalCookieAuthenticationScheme;

               options.ClientId = "708996912208-9m4dkjb5hscn7cjrn5u0r4tbgkbj1fko.apps.googleusercontent.com";
               options.ClientSecret = "wdfPY6t8H8cecgjlxud__4Gh";
            })
            .AddOpenIdConnect("demoidsrv", "IdentityServer", options =>
            {
               options.SignInScheme = IdentityServerConstants.ExternalCookieAuthenticationScheme;
               options.SignOutScheme = IdentityServerConstants.SignoutScheme;

               options.Authority = "https://demo.kitsdg.ch/";
               options.ClientId = "implicit";
               options.ResponseType = "id_token";
               options.SaveTokens = true;
               options.CallbackPath = new PathString("/signin-idsrv");
               options.SignedOutCallbackPath = new PathString("/signout-callback-idsrv");
               options.RemoteSignOutPath = new PathString("/signout-idsrv");

               options.TokenValidationParameters = new TokenValidationParameters
               {
                  NameClaimType = "name",
                  RoleClaimType = "role"
               };
            });
      }

      // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
      public void Configure(IApplicationBuilder app, IHostingEnvironment env)
      {
         if (env.IsDevelopment())
         {
            app.UseDeveloperExceptionPage();
         }

         app.UseIdentityServer();
         app.UseStaticFiles();
         app.UseMvcWithDefaultRoute();
      }
   }
}
