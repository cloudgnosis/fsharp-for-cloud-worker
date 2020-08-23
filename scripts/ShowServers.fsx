#!/usr/bin/env dotnet fsi --langversion:preview

// Get the AWS SDK packages needed
#r "nuget: AWSSDK.Core"
#r "nuget: AWSSDK.EC2"

open System.Environment
open Amazon.EC2
open Amazon.EC2.Model

let describeInstances (client: AmazonEC2Client) =
    async {
        let! response = client.DescribeInstancesAsync() |> Async.AwaitTask
        return response
    }
    
let showServers (args: string[]) =
    if args.Length > 0 then
        do SetEnvironmentVariable ("AWS_PROFILE", args.[0])
    let client = new AmazonEC2Client()
    let response = (describeInstances client) |> Async.RunSynchronously
    response
    
showServers (fsi.CommandLineArgs |> Array.skip 1)

(*
  showServers [| "erik" |]
  GetEnvironmentVariable("AWS_PROFILE")
  let response = showServers [| "erik" |]
*)
