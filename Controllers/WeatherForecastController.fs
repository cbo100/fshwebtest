namespace fshweb.Controllers

open System
open System.Collections.Generic
open System.Linq
open System.Threading.Tasks
open Microsoft.AspNetCore.Mvc
open Microsoft.Extensions.Logging
open fshweb

[<ApiController>]
[<Route("[controller]")>]
type WeatherForecastController (logger : ILogger<WeatherForecastController>) =
    inherit ControllerBase()

    let summaries =
        [|
            "Freezing"
            "Bracing"
            "Chilly"
            "Cool"
            "Mild"
            "Warm"
            "Balmy"
            "Hot"
            "Sweltering"
            "Scorching"
        |]

    [<HttpGet;
      // ProducesResponseType(typeof<WeatherForecastContract list>, 200);
      ProducesResponseType(typeof<WeatherForecast list>, 200);
      ProducesResponseType(404);>]    
    member _.Get() =
        let rng = System.Random()
        [|
            for index in 0..4 ->
                WeatherForecast (
                    None, // Some DateTime.Now.AddDays(float index)
                    rng.Next(-20,55),
                    Some summaries.[rng.Next(summaries.Length)],
                    Some { 
                        Speed = 10 
                        Direction = "south"
                    }
                )
                
        |]


// Options:
// - fake response type contract
// - MapType() for each type
// - Nullable<>