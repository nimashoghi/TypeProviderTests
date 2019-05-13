open System
open JsonLoader

type X = global.JsonLoader.JsonProvider<"""{"name": "hello"}""">
let g: X = null

[<EntryPoint>]
let main argv =
    printfn "Hello World from F#!"
    0 // return an integer exit code
