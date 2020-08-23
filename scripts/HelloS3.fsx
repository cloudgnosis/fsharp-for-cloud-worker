#!/usr/bin/env dotnet fsi --langversion:preview

// Get the AWS SDK packages needed
#r "nuget: AWSSDK.Core"
#r "nuget: AWSSDK.S3"

open Amazon.S3
open Amazon.S3.Model

let getBucketInfo (bucket: S3Bucket) =
    sprintf "Name: %s created at %O" bucket.BucketName bucket.CreationDate

let listBuckets (s3Client: AmazonS3Client) =
    async {
        let! response = s3Client.ListBucketsAsync() |> Async.AwaitTask
        return response
    }

let helloS3 () =
    let client = new AmazonS3Client()
    let response = (listBuckets client) |> Async.RunSynchronously
    let bucketsInfo = (List.ofSeq response.Buckets) |> List.map getBucketInfo
    for bucketInfo in bucketsInfo do
            printfn "%s" bucketInfo
helloS3()
