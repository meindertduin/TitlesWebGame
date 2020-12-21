using System;
using System.Net.Http;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Components.WebAssembly.Hosting;
using Microsoft.Extensions.DependencyInjection;
using TitlesWebGame.WebUi.Services;
using TitlesWebGame.WebUi.ViewModels;

namespace TitlesWebGame.WebUi
{
    public class Program
    {
        public static async Task Main(string[] args)
        {
            var builder = WebAssemblyHostBuilder.CreateDefault(args);
            builder.RootComponents.Add<App>("#app");
            
            builder.Services.AddScoped<GameSocketConnectionManager>();
            builder.Services.AddScoped<GameViewModel>();
            builder.Services.AddTransient<GameSocketServerMessageHandler>();
            builder.Services.AddSingleton<GameSessionState>();
            builder.Services.AddTransient<ITitlesCategoryEmojiFactory, TitlesCategoryEmojiFactory>();

            builder.Services.AddSingleton<ApplicationViewModel>();

            builder.Services.AddScoped(
                sp => new HttpClient {BaseAddress = new Uri(builder.HostEnvironment.BaseAddress)});

            await builder.Build().RunAsync();
        }
    }
}