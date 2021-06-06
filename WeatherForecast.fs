namespace fshweb

open System
open System.Runtime.Serialization




module private StringUtils =
    let splitCommaSeparatedString (toSplit: string) =
        toSplit.Split([| ',' |], StringSplitOptions.RemoveEmptyEntries)
        |> Array.map (fun x -> x.Trim())

    let convertToEmptyIfNull (value: string) =
        if isNull value then
            String.Empty
        else
            value


[<AbstractClass>]
type FilterableEntity() =
    let mutable _filteredProperties: Set<string> = Set.empty

    let normalizePropertyName (propertyName: string) : string =
        propertyName.ToUpperInvariant()

    [<IgnoreDataMember>]
    member _.FilteredProperties = _filteredProperties

    member _.SetFilters (commaSeparatedFields: string) =
        _filteredProperties <-
            commaSeparatedFields
            |> StringUtils.convertToEmptyIfNull
            |> StringUtils.splitCommaSeparatedString
            |> Array.map normalizePropertyName
            |> Set.ofArray

    member x.ShouldSerialize (propertyName: string) : bool =
        let shouldSerialize =
            x.FilteredProperties |> Set.isEmpty ||
            x.FilteredProperties |> Set.contains(propertyName |> normalizePropertyName)

        shouldSerialize


type WindInformation =
  struct
    val Speed: int
    val Direction: string
    new(speed: int, direction: string) = { Speed = speed; Direction = direction }
  end

type WeatherForecastContract = 
  {
    Date: DateTime
    TemperatureC: int
    TemperatureF: float
    Summary: string
    Wind: WindInformation
  }

type WeatherForecast (date: DateTime option,
      temperatureC: int,
      summary: string option,
      wind: Nullable<WindInformation>
    ) = 
    inherit FilterableEntity()


    member _.Date = date
    member _.TemperatureF = 32.0 + (float temperatureC / 0.5556)
    member _.TemperatureC = temperatureC
    member _.Summary = summary
    member _.Wind = wind

    member x.ShouldSerializeDate() = x.ShouldSerialize("Date")
    member x.ShouldSerializeTemperatureF() = x.ShouldSerialize("TemperatureF")
    member x.ShouldSerializeTemperatureC() = x.ShouldSerialize("TemperatureC")
    member x.ShouldSerializeSummary() = x.ShouldSerialize("Summary")
    member x.ShouldSerializeWind() = x.ShouldSerialize("Wind")


// Find why this has been reverted: https://github.com/domaindrivendev/Swashbuckle.AspNetCore/blob/3102a86096849e9e43a2aaf40f384710efa03c8c/src/Swashbuckle.AspNetCore.SwaggerGen/Generator/SchemaRegistry.cs
// https://github.com/domaindrivendev/Swashbuckle.AspNetCore/commit/2733cde790df8333189025c52beef6aa67b299bf