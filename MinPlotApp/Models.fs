namespace MinPlotApp

open FSharp.Data

module Models = 
    
    type Config = JsonProvider<"ConfigTemplate.json"> 
            
    let read (json:string) = Config.Load(json)
