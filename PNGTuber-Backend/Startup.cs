using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.HttpsPolicy;
using Microsoft.AspNetCore.SpaServices.AngularCli;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

using DSharpPlus;

namespace PNGTuber
{
    public class Startup
    {
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        public IConfiguration Configuration { get; }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            services.AddControllersWithViews();
            services.AddSignalR();
            services.AddSingleton(GetDiscordClient(Configuration.GetValue<string>("token")));
        }

        private static DiscordClient GetDiscordClient(string token)
        {
            var client = new DiscordClient(new DiscordConfiguration
            {
                Token = token,
                TokenType = TokenType.Bot,
                Intents = DiscordIntents.AllUnprivileged
            });
            client.ConnectAsync().GetAwaiter().GetResult();
            return client;
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }

            app.UseHttpsRedirection();

            app.UseRouting();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapHub<VoiceHub>("/voices");
                endpoints.MapControllers();
            });
        }
    }
}
