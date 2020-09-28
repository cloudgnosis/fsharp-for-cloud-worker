namespace HelloApi

open System
open Amazon.CDK
open Amazon.CDK.AWS.Lambda
open Amazon.CDK.AWS.Logs

type HelloApiStack(scope, id, props) as this =
    inherit Stack(scope, id, props)

    let mutable funcProps = FunctionProps()
    do funcProps.Runtime <- Runtime.DOTNET_CORE_3_1
       funcProps.Code <- Code.FromAsset("./src/Backend/bin/Debug/netcoreapp3.1/publish")
       funcProps.Handler <- "Backend::Backend.Function::FunctionHandler"
       funcProps.Description <- "Our backend Lambda function"
       funcProps.MemorySize <- Nullable<float>(256.0)
       funcProps.LogRetention <- Option.toNullable(Some RetentionDays.ONE_WEEK)
    let backend = Function(this, "Backend-function", funcProps)