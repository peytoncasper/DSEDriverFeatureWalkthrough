using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Dse;
using System.IO;
using System.Reflection;

namespace CoreCSharpDriver
{
    public class Startup
    {
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;

            String create_keyspace = @"CREATE KEYSPACE IF NOT EXISTS driver_test 
                                          WITH REPLICATION = { 
                                           'class' : 'NetworkTopologyStrategy', 
                                           'DC1' : 1 
                                          } ;
                                        ";
            String create_user_table = @"CREATE TABLE IF NOT EXISTS driver_test.user (
                                            id uuid, 
                                            first_name text, 
                                            last_name text, 
                                            PRIMARY KEY (id)
                                        );";

            IDseCluster cluster = DseCluster.Builder()
                .AddContactPoint("127.0.0.1")
                .Build();
            IDseSession session = cluster.Connect();

            session.Execute(create_keyspace);
            session.Execute(create_user_table);

            session.Dispose();
        }

        public IConfiguration Configuration { get; }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            services.AddMvc();
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IHostingEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }

            app.UseMvc();
        }
    }
}
