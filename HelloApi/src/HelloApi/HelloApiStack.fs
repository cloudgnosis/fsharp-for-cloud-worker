namespace HelloApi

open System
open Amazon.CDK
open Amazon.CDK.AWS.APIGateway
open Amazon.CDK.AWS.DynamoDB
open Amazon.CDK.AWS.Lambda
open Amazon.CDK.AWS.Logs


type HelloApiStack(scope, id, props) as this =
    inherit Stack(scope, id, props)

    let tableProps = TableProps (PartitionKey = Attribute(Name = "path", Type = AttributeType.STRING))
    let table = Table(this, "msgdata-table", tableProps)

    let funcProps =
        FunctionProps(Runtime = Runtime.DOTNET_CORE_3_1,
                      Code = Code.FromAsset("./src/Backend/bin/Debug/netcoreapp3.1/publish"),
                      Handler = "Backend::Backend.Function::FunctionHandler",
                      Description = "Our backend Lambda function",
                      MemorySize = Nullable<float>(256.0),
                      Timeout = Duration.Seconds(10.0),
                      LogRetention = Option.toNullable (Some RetentionDays.ONE_WEEK))

    let backend =
        Function(this, "Backend-function", funcProps)
    do backend.AddEnvironment ("TABLE_NAME", table.TableName) |> ignore
       table.GrantReadWriteData (backend) |> ignore

    let apiProps =
        LambdaRestApiProps
            (Description = "A simple example API backed by lambda using CDK",
             EndpointExportName = "hello-api",
             Handler = backend)

    let gateway = LambdaRestApi(this, "api", apiProps)
