namespace MinPlotApp

open System.Text.RegularExpressions

module ConfigControl =
    
    let loadExtMap () =
        let directory = "Templates"
        System.IO.Directory.GetFiles (directory)
        |> Array.toList 
        |> List.map (fun path -> 
            let x = MinPlotApp.Models.Config.Load (path)
            (x.Ext, x.Template)
        )
    
    let getDataFormat (map:(string * string) list) (dataPath:string) =
        let rec getDataFormatIter (src:(string * string) list) =
            match src with
            | (key, value) :: tl ->
                let regex = new Regex (key)
                if regex.IsMatch (dataPath) 
                then Some (value)
                else getDataFormatIter tl
            | _ -> 
                None
        getDataFormatIter map         

    let loadConfig (dataPath:string) =
        getDataFormat (loadExtMap ()) dataPath
        |> function
        | Some (path) -> MinPlot.Models.CsvFormat.Load (path)
        | None        -> MinPlot.Models.CsvFormat.GetSample ()
