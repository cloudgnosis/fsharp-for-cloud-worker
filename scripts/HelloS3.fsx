#!/usr/bin/env dotnet fsi --langversion:preview

// Get the AWS SDK packages needed
#r "nuget: AWSSDK.Core"
#r "nuget: AWSSDK.S3"

open Amazon.S3

let getBucketsInfo (s3Client: AmazonS3Client) =
    [| "Bucket 1 info"; "Bucket 2 info" |]

let helloS3 () =
    let client = new AmazonS3Client()
    let bucketsInfo = getBucketsInfo client
    for bucketInfo in bucketsInfo do
        printfn "%s" bucketInfo

helloS3()