#!/usr/bin/env dotnet fsi
  
let hello messages =
    messages |> Array.map (printfn "Hello %s!")

hello (fsi.CommandLineArgs |> Array.skip 1)