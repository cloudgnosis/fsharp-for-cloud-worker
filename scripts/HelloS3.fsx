#!/usr/bin/env dotnet fsi --langversion:preview

// Get the AWS SDK packages needed
#r "nuget: AWSSDK.Core"
#r "nuget: AWSSDK.S3"

open Amazon.S3

let helloS3 () =
    let client = new AmazonS3Client()
    "dummy"

helloS3()