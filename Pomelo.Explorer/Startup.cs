using System.Runtime.Loader;
using System.Resources;
using System.Linq;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Infrastructure;
using Microsoft.AspNetCore.Mvc.ApplicationParts;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using ElectronNET.API;
using ElectronNET.API.Entities;
using Microsoft.Extensions.Hosting;

namespace Pomelo.Explorer
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
            services.AddMvc();
            services.AddSingleton<IActionDescriptorChangeProvider>(PomeloActionDescriptorChangeProvider.Instance);
            services.AddSingleton(PomeloActionDescriptorChangeProvider.Instance);
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }
            else
            {
                app.UseExceptionHandler("/Home/Error");
            }

            app.UseStaticFiles();
            app.UseExtensionStaticResourceMiddleware();
            app.UseVueMiddleware();

            app.UseRouting();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllerRoute("default", "{controller=Home}/{action=Index}/{id?}");
            });

            var partManager = app.ApplicationServices.GetRequiredService<ApplicationPartManager>();
            AddAssembly(@"C:\Users\Yuko\Documents\GitHub\PomeloDBExplorer\electron.net-api-demos-master\Pomelo.Explorer.MySQL\bin\Debug\netcoreapp3.0\Pomelo.Explorer.MySQL.dll", partManager);
            AddAssembly(@"C:\Users\Yuko\Documents\GitHub\PomeloDBExplorer\electron.net-api-demos-master\Pomelo.Explorer.MySQL\bin\Debug\netcoreapp3.0\Pomelo.Explorer.MySQL.Views.dll", partManager);

            if (HybridSupport.IsElectronActive)
            {
                ElectronBootstrap();
            }
        }

        private static void AddAssembly(string assemblyPath, ApplicationPartManager partManager)
        {
            var assembly = AssemblyLoadContext.Default.LoadFromAssemblyPath(assemblyPath);
            if (assembly != null)
            {
                if (assemblyPath.EndsWith(".Views.dll"))
                {
                    partManager.ApplicationParts.Add(new CompiledRazorAssemblyPart(assembly));
                }
                else
                {
                    AssemblyHelper.RegisterExtension(assembly);
                    partManager.ApplicationParts.Add(new AssemblyPart(assembly));
                }
                PomeloActionDescriptorChangeProvider.Instance.HasChanged = true;
                PomeloActionDescriptorChangeProvider.Instance.TokenSource.Cancel();
            }
        }

        public async void ElectronBootstrap()
        {
            var browserWindow = await Electron.WindowManager.CreateWindowAsync(new BrowserWindowOptions
            {
                Width = 1152,
                Height = 864,
                Show = true,
                DarkTheme = true
            }, $"http://localhost:{BridgeSettings.WebPort}/index.html");
            //browserWindow.OnReadyToShow += () => browserWindow.Show();
            browserWindow.SetTitle("Electron.NET API Demos");
        }
    }
}
