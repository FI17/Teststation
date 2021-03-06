﻿using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Teststation.Models;

[assembly: HostingStartup(typeof(Teststation.Areas.Identity.IdentityHostingStartup))]
namespace Teststation.Areas.Identity
{
    public class IdentityHostingStartup : IHostingStartup
    {
        public void Configure(IWebHostBuilder builder)
        {
            builder.ConfigureServices((context, services) =>
            {
                services.AddDbContext<Database>(options =>
                    options.UseSqlite(
                        context.Configuration.GetConnectionString("TeststationContextConnection")));

                services.AddDefaultIdentity<IdentityUser>()
                    .AddEntityFrameworkStores<Database>();
            });
        }
    }
}