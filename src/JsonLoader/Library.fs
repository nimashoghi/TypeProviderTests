namespace JsonLoader

open System.Collections.Generic
open Newtonsoft.Json


module Parse =
    let parse string =
        JsonConvert.DeserializeObject<Dictionary<string, obj>> string
        |> Seq.toList
        |> List.map (|KeyValue|)


#if !IS_DESIGNTIME
// Put the TypeProviderAssemblyAttribute in the runtime DLL, pointing to the design-time DLL
[<assembly: CompilerServices.TypeProviderAssembly "JsonLoader.DesignTime.dll">]
do ()
#endif
