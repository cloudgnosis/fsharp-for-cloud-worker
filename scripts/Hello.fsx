#!/usr/bin/env dotnet fsi
// Now with command line arguments!

let hello messages =
    for message in messages do
        printfn "Hello %s!" message

hello fsi.CommandLineArgs.[1..]