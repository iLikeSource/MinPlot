namespace MinPlot

open System
open System.IO
open XPlot.GoogleCharts

module GraphControl = 

    ///  列データを指定間隔ごとに存在しなくなるまで取得する
    let getData (data:string array, interval:int, current:int) =
        let rec getDataIter (dst:string list) (data:string array, interval:int, current:int) = 
            if current < data.Length then
                getDataIter (data.[current] :: dst) (data, interval, current + interval)
            else
                dst |> List.rev |> Array.ofList     
        getDataIter [] (data, interval, current)
    
    ///  1行のデータがペア配列になっている状態から、複数のデータシリーズに分割する 
    let splitData (data:(float * float) array array) = 
        //  まずシリーズの個数を推定する
        let seriesCount = data.[0].Length 
        //  個数ごとに分割
        [| 0 .. (seriesCount - 1) |]
        |> Array.map (fun index -> 
            data |> Array.map (fun series ->
                series.[index]        
            )
        )

    let ceiling (value:float) = 
        let splitValue    = (value |> Printf.sprintf "%.1e").Split ([| 'e' |])
        let (real, digit) = (splitValue.[0], splitValue.[1]) 
        let realValue = float real
        Printf.sprintf "%.1fe%s" 
            (if realValue > 0.0 
             then Math.Ceiling (realValue) 
             else Math.Floor   (realValue))
            digit
        |> float
    let ranges (data:float array) = 
        let (min, max) =
            data |> Array.fold (fun (min, max) value -> 
                ((if value < min then value else min), 
                 (if value > max then value else max))    
            ) (0.0, 0.0)
        (ceiling min, ceiling max)

    let xyRanges (data:(float * float) array array) =
        let allData = data |> Array.concat
        let xData   = allData |> Array.map fst
        let yData   = allData |> Array.map snd
        (ranges xData, ranges yData)
            
          
    let getDataSource (srcPath:string, model:Models.CsvFormat.Root) = 
        use reader = new StreamReader (srcPath) 
        let lines  = 
            reader.ReadToEnd()
                  .Split([| Environment.NewLine |], StringSplitOptions.None)
        let dataLines = 
            Array.sub lines model.Skip (lines.Length - model.Skip)
            |> Array.filter (fun line -> line.Trim().Length > 0)
        let pairSources = 
            dataLines |> Array.map (fun line ->
                let data = line.Split([| "," |], StringSplitOptions.None)
                let xData = getData (data, model.Xcolumn.Interval, model.Xcolumn.Start - 1) |> Array.map float
                let yData = getData (data, model.Ycolumn.Interval, model.Ycolumn.Start - 1) |> Array.map float
                Array.map2 (fun x y -> (x, y)) xData yData
            )
            |> splitData 
        pairSources
    
    let getDataSources (models:(string * Models.CsvFormat.Root) array) = 
        models |> Array.map getDataSource |> Array.concat
            
    let getChart (models:(string * Models.CsvFormat.Root) array) = 
        let data  = getDataSources (models)
        let ((xMin, xMax), (yMin, yMax)) = xyRanges (data)
        let hAxis   = new Axis (ticks = [| xMin; xMax |])
        let vAxis   = new Axis (ticks = [| yMin; yMax |])
        let options = new Options (hAxis = hAxis, vAxis = vAxis) 
        let chart =
            data
            |> Chart.Line
            |> Chart.WithWidth  400
            |> Chart.WithHeight 400
            |> Chart.WithOptions (options)
        chart

    let save (savingPath:string) (models:(string * Models.CsvFormat.Root) array) =
        use writer = new StreamWriter (savingPath)
        let chart = getChart (models)
        chart.GetHtml() |> writer.Write 
        savingPath
    
    let show (savingPath:string) (models:(string * Models.CsvFormat.Root) array) = 
        try
            let proc = 
                save (savingPath) (models) 
                |> System.Diagnostics.Process.Start      
            proc.WaitForExit ()
        with _ ->
            // htmlが拡張子関連付けされていない場合
            ()