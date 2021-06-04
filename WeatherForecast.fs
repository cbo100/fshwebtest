namespace fshweb

open System

type WindInformation = 
  { Speed: int
    Direction: string }

type WeatherForecast =
    { Date: DateTime
      TemperatureC: int
      Summary: string
      Wind: WindInformation option}

    member this.TemperatureF =
        32.0 + (float this.TemperatureC / 0.5556)
