open Amazon.CDK
open HelloApi

[<EntryPoint>]
let main _ =
    let app = App(null)

    HelloApiStack(app, "HelloApiStack", StackProps()) |> ignore

    app.Synth() |> ignore
    0
