#!/usr/bin/env dotnet fsi --langversion:preview

// Get the AWS SDK packages needed
#r "nuget: AWSSDK.Core"
#r "nuget: AWSSDK.EC2"

open System.Environment
open Amazon.EC2
open Amazon.EC2.Model


let getInstances (reservations: Reservation list) =
    reservations |> List.collect (fun x -> List.ofSeq x.Instances)


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
    if response.Reservations.Count = 0 then
        List.empty<Instance>
    else
        (List.ofSeq response.Reservations) |> getInstances

    
showServers (fsi.CommandLineArgs |> Array.skip 1)

(*
  showServers [| "erik" |]
  GetEnvironmentVariable("AWS_PROFILE")
  let mylist = List.ofSeq response.Reservations
  let response = showServers [| "erik" |]
*)
