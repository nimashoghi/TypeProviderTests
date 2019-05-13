namespace JsonLoader

open System.Reflection
open FSharp.Core.CompilerServices
open ProviderImplementation.ProvidedTypes

[<AutoOpen>]
module Utils =
    let logIt value = System.IO.File.AppendAllText("/workspaces/TypeProviderTests/log.txt", sprintf "%s\n" value)

module Provider =
    let makeProvidedType assembly ``namespace`` =
        let generator =
            ProvidedTypeDefinition (
                assembly = assembly,
                namespaceName = ``namespace``,
                className = "JsonProvider",
                baseType = Some typeof<obj>
            )

        generator.DefineStaticParameters (
            [
                ProvidedStaticParameter (
                    parameterName = "Data",
                    parameterType = typeof<string>
                )
            ],
            fun name args ->
                let ``type`` =
                    ProvidedTypeDefinition (
                        assembly = assembly,
                        namespaceName = ``namespace``,
                        className = name,
                        baseType = Some typeof<obj>
                    )

                Parse.parse (unbox<string> args.[0])
                |> List.map (
                    fun (key, value) ->
                        ProvidedProperty (
                            propertyName = key,
                            propertyType = value.GetType(),
                            getterCode = (fun _ -> <@@ value @@>),
                            isStatic = true
                        )
                )
                |> List.iter ``type``.AddMember

                ``type``
        )

        generator

[<TypeProvider>]
type JsonTypeProvider (config) as this =
    inherit
        TypeProviderForNamespaces (
            config = config,
            assemblyReplacementMap = ["JsonLoader.DesignTime", "JsonLoader"],
            addDefaultProbingLocation = true
        )

    let ``namespace`` = "JsonLoader"
    let assembly = Assembly.GetExecutingAssembly ()

    do logIt config.ResolutionFolder
    do this.AddNamespace(``namespace``, [Provider.makeProvidedType assembly ``namespace``])

[<assembly: TypeProviderAssembly>]
do ()
