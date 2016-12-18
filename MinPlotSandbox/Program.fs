// F# の詳細については、http://fsharp.org を参照してください
// 詳細については、'F# チュートリアル' プロジェクトを参照してください。

open MinPlot

[<EntryPoint>]
let main argv =
    [| ("SampleData.json", MinPlot.Models.CsvFormat.Load "SampleData.json") |]
    |> MinPlot.GraphControl.show "Sample.html"
 
    printfn "%A" argv
    0 // 整数の終了コードを返します
