namespace fshweb

open System
open System.Collections.Generic
open System.Linq
open System.Threading.Tasks
open Microsoft.AspNetCore.Builder
open Microsoft.AspNetCore.Hosting
open Microsoft.AspNetCore.HttpsPolicy;
open Microsoft.AspNetCore.Mvc
open Microsoft.Extensions.Configuration
open Microsoft.Extensions.DependencyInjection
open Microsoft.Extensions.Hosting
open Swashbuckle.AspNetCore.Swagger
open Microsoft.OpenApi.Models
open Newtonsoft.Json;
open Microsoft.FSharpLu.Json

type Startup(configuration: IConfiguration) =
    member _.Configuration = configuration

    // This method gets called by the runtime. Use this method to add services to the container.
    member _.ConfigureServices(services: IServiceCollection) =
        // Add framework services.
        let info = OpenApiInfo()
        info.Title <- "My API V1"
        info.Version <- "v1"
        services.AddSwaggerGen(fun config -> 
          config.SwaggerDoc("v1", info)
        ) |> ignore
        services
          .AddControllers()
          .AddNewtonsoftJson(fun options -> 
            let settingsToAdd = Compact.TupleAsArraySettings.settings
            options.SerializerSettings.NullValueHandling <- settingsToAdd.NullValueHandling
            for converter in settingsToAdd.Converters do
             options.SerializerSettings.Converters.Add(converter)
          ) |> ignore
        

    // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
    member _.Configure(app: IApplicationBuilder, env: IWebHostEnvironment) =
        if (env.IsDevelopment()) then
            app.UseDeveloperExceptionPage() |> ignore
            app.UseSwagger() |> ignore
            app.UseSwaggerUI(fun config -> config.SwaggerEndpoint("/swagger/v1/swagger.json", "My API V1")) |> ignore
        app.UseHttpsRedirection()
           .UseRouting()
           .UseAuthorization()
           .UseEndpoints(fun endpoints ->
                endpoints.MapControllers() |> ignore
            ) |> ignore
