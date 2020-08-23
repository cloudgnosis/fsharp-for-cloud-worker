#!/usr/bin/env dotnet fsi
// This is our first hello cloud script. This is a comment line.
let hello message =
    printfn "Hello %s!" message

hello "cloud"