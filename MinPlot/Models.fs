namespace MinPlot

open FSharp.Data

module Models = 
    
    type CsvFormat = JsonProvider<"CsvFormat.json">

    let read (json:string) = CsvFormat.Load(json)

