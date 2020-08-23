#!/usr/bin/env dotnet fsi
// Now with command line arguments!

let hello messages =
    messages |> Array.map (printfn "Hello %s!")

hello fsi.CommandLineArgs.[1..]