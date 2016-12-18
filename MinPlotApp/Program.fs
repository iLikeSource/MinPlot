// F# の詳細については、http://fsharp.org を参照してください
// 詳細については、'F# チュートリアル' プロジェクトを参照してください。

open MinPlotApp

[<EntryPoint>]
let main argv = 
    printfn "%A" argv
    
    let srcPath = argv.[0]
    let dstPath = argv.[1]

    [| (srcPath, ConfigControl.loadConfig (srcPath)) |]
    |> MinPlot.GraphControl.show dstPath

    0 // 整数の終了コードを返します
