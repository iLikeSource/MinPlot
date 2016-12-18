// F# の詳細については、http://fsharp.org を参照してください。F# プログラミングのガイダンスについては、
// 'F# チュートリアル' プロジェクトを参照してください。

#I @"bin\Debug"

#r "System.Data"
#r "XPlot.GoogleCharts.dll"
#r "Newtonsoft.Json.dll"
#r "FSharp.Data.dll"

#load "Models.fs"
#load "GraphControl.fs"
open MinPlot

// ここでライブラリ スクリプト コードを定義します

"sample-source.csv"
|> GraphControl.getDataSources
|> Printf.printf "%A"
    