using System;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.UI;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using TeslaCamBrowser.Areas.Identity.Data;
using TeslaCamBrowser.Models;

[assembly: HostingStartup(typeof(TeslaCamBrowser.Areas.Identity.IdentityHostingStartup))]
namespace TeslaCamBrowser.Areas.Identity
{
    public class IdentityHostingStartup : IHostingStartup
    {
        public void Configure(IWebHostBuilder builder)
        {
            builder.ConfigureServices((context, services) => {
                services.AddDbContext<TeslaCamBrowserContext>(options =>
                    options.UseSqlite(
                        context.Configuration.GetConnectionString("TeslaCamBrowserContextConnection")));

                services.AddDefaultIdentity<TeslaCamBrowserUser>()
                    .AddEntityFrameworkStores<TeslaCamBrowserContext>();
            });
        }
    }
}