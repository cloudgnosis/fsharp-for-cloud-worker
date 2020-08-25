#!/usr/bin/env dotnet fsi --langversion:preview

// Get the AWS SDK packages needed
#r "nuget: AWSSDK.Core"
#r "nuget: AWSSDK.EC2"

open System
open System.Environment
open Amazon.EC2
open Amazon.EC2.Model

type IpAddressOrDns = 
    | IpAddress of string
    | DnsName of string

let toString v  = 
    match v with
    | IpAddress s -> s
    | DnsName s -> s

type InstanceInfo =
    { InstanceId:         string
      Name:               string
      InstanceType:       string
      PrivateHostAddress: IpAddressOrDns
      LaunchTime:         DateTime
      State:              string }


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
        PrivateHostAddress = IpAddress instance.PrivateIpAddress
        LaunchTime = instance.LaunchTime
        State = instance.State.Name.Value
    }


let yyyymmddhhmmss (dt: DateTime) =
    dt.ToString ("yyyy-MM-dd HH:mm:ss")


let printInstanceInfo (instanceInfos: InstanceInfo list) : unit =
    if instanceInfos.IsEmpty then
        printfn "No instance info available!"
    else
        printfn  "%-20s %-20s %-16s %-21s %-20s %-10s"
            "InstanceId" "Name" "InstanceType" "Private Host Address" "LaunchTime" "State"
        for ii in instanceInfos do
            printfn "%-20s %-20s %-16s %-21s %-20s %-10s"
                ii.InstanceId ii.Name ii.InstanceType (toString ii.PrivateHostAddress) (yyyymmddhhmmss ii.LaunchTime) ii.State


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
    let infolist = 
        if response.Reservations.Count = 0 then
            List.empty<Instance>
        else
            (List.ofSeq response.Reservations) |> getInstances
    infolist |> List.map getInstanceInfo |> printInstanceInfo

    
showServers (fsi.CommandLineArgs |> Array.skip 1)

(*
  showServers [| "erik" |]
  GetEnvironmentVariable("AWS_PROFILE")
  let mylist = List.ofSeq response.Reservations
  let response = showServers [| "erik" |]
  let iis = showServers [| "erik" |] |> List.map getInstanceInfo
  showServers [| "erik" |] |> List.map getInstanceInfo |> printInstanceInfo 
  getInstanceInfo response.[0]
  let instance = response.[0]
  let tags = List.ofSeq instance.Tags
  getTagValue "Name" tags
*)