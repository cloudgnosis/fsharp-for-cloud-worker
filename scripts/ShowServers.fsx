#!/usr/bin/env dotnet fsi --langversion:preview

// Get the AWS SDK packages needed
#r "nuget: AWSSDK.Core"
#r "nuget: AWSSDK.EC2"

open System
open System.Environment
open Amazon.EC2
open Amazon.EC2.Model


type InstanceInfo =
    { InstanceId:       string
      Name:             string
      InstanceType:     string
      PrivateIpAddress: string
      LaunchTime:       DateTime
      State:            string }

      
let rec getTagValue key (tags: Tag list) =
    match tags with
    | [] -> ""
    | head :: tail -> if head.Key = key then head.Value  else getTagValue key tail


let getInstanceInfo (instance: Instance) =
    let tags = List.ofSeq instance.Tags
    {
        InstanceId = instance.InstanceId
        Name = getTagValue "Name" tags
        InstanceType = instance.InstanceType.Value
        PrivateIpAddress = instance.PrivateIpAddress
        LaunchTime = instance.LaunchTime
        State = instance.State.Name.Value
    }


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
  let iis = showServers [| "erik" |] |> List.map getInstanceInfo
  getInstanceInfo response.[0]
  let instance = response.[0]
  let tags = List.ofSeq instance.Tags
  getTagValue "Name" tags
*)
