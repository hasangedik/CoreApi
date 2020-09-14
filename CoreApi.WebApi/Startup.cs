using Autofac;
using AutoMapper;
using CoreApi.Common.Extensions;
using CoreApi.Common.Options;
using CoreApi.Common.Provider;
using CoreApi.Container.Modules;
using CoreApi.Core.Configuration;
using CoreApi.Infrastructure.Context;
using CoreApi.WebApi.Auth;
using CoreApi.WebApi.Middleware;
using JetBrains.Annotations;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.StaticFiles;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Newtonsoft.Json;
using Serilog;

namespace CoreApi.WebApi
{
    public class Startup
    {
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        [UsedImplicitly]
        public IConfiguration Configuration { get; }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            var wallOptions = new WallOptions();
            Configuration.GetSection("CoreApi").Bind(wallOptions);
            
            var connectionString = Configuration.GetConnectionString("CoreApiContext").Decrypt(wallOptions.EKey);

            services.AddDbContext<DataContext>(
                options => options.UseNpgsql(connectionString)
            );
            
            services.AddMvc(x => x.AllowEmptyInputInBodyModelBinding = true);

            services.AddAuthentication(x =>
            {
                x.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
                x.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
            }).AddJwtBearer(x =>
            {
                x.RequireHttpsMetadata = false;
                x.SaveToken = true;
                x.TokenValidationParameters = JwtManager.ValidationParameters;
            });
            
            var contractResolver = new Newtonsoft.Json.Serialization.DefaultContractResolver
            {
                NamingStrategy = new Newtonsoft.Json.Serialization.CamelCaseNamingStrategy()
            };

            services.AddControllers()
                .AddNewtonsoftJson(options =>
                {
                    options.SerializerSettings.ContractResolver = contractResolver;
                    options.SerializerSettings.Formatting = Formatting.Indented;
                    options.SerializerSettings.PreserveReferencesHandling = PreserveReferencesHandling.None;
                    options.SerializerSettings.DefaultValueHandling = DefaultValueHandling.Include;
                    options.SerializerSettings.ReferenceLoopHandling = ReferenceLoopHandling.Ignore;
                    options.SerializerSettings.DateTimeZoneHandling = DateTimeZoneHandling.Local;
                });
            
            services.AddHttpContextAccessor();
            services.AddSingleton(provider => new MapperConfiguration(cfg =>
            {
                cfg.AddProfile(new AutoMapperConfiguration(wallOptions));
            }).CreateMapper());
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }

            var provider = new FileExtensionContentTypeProvider();
            
            app.UseDefaultFiles();
            app.UseStaticFiles(new StaticFileOptions
            {
                ContentTypeProvider = provider
            });
            
            #region Security
            //https://docs.nwebsec.com/en/latest/nwebsec/getting-started.html

            app.UseRedirectValidation(opts =>
            {
                opts.AllowedDestinations("https://github.com/login/oauth/authorize");
            });
            app.UseReferrerPolicy(opts => opts.NoReferrer());
            //app.UseXfo(options => options.SameOrigin());
            app.UseXXssProtection(options => options.EnabledWithBlockMode());
            app.UseXContentTypeOptions();
            app.UseXDownloadOptions();
            app.UseXRobotsTag(options => options.NoIndex().NoFollow());
            app.UseXfo(options => options.SameOrigin());

            #endregion

            app.UseSerilogRequestLogging();
            app.UseRouting();
            app.UseAuthentication();
            app.UseAuthorization();
            app.UseRequestLogMiddleware();
            app.UseExceptionMiddleware();
            app.UseEndpoints(endpoints => { endpoints.MapControllers(); });
        }
        
        [UsedImplicitly]
        public void ConfigureContainer(ContainerBuilder builder)
        {
            builder.RegisterModule(new RepositoryModule());
            builder.RegisterModule(new ServiceModule());

            builder.RegisterType<UserProvider>()
                .As<IUserProvider>()
                .WithParameter("isUserFilterEnabled", true)
                .SingleInstance();
            
            builder.RegisterType<SoftDeleteProvider>()
                .As<ISoftDeleteProvider>()
                .WithParameter("isSoftDeleteFilterEnabled", true)
                .SingleInstance();
        }
    }
}