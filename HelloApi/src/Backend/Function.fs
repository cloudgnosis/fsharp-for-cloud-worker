namespace Backend


open System.Collections.Generic
open System.Net
open Amazon.Lambda.Core
open Amazon.Lambda.APIGatewayEvents
open Amazon.DynamoDBv2
open Amazon.DynamoDBv2.Model


// Assembly attribute to enable the Lambda function's JSON input to be converted into a .NET class.
[<assembly: LambdaSerializer(typeof<Amazon.Lambda.Serialization.SystemTextJson.DefaultLambdaJsonSerializer>)>]
()


type Function(dynamoDbClient: IAmazonDynamoDB) =

    new() = 
        Function(new AmazonDynamoDBClient())

    static member CreateUpdateItemRequest tableName key =
        let updateExpression = "ADD hits :incr"
        let keyAttribute = Dictionary(["path", AttributeValue ( S = key )] |> Map.ofList)
        let eav = Dictionary([":incr", AttributeValue ( N = "1" ) ] |> Map.ofList)
        UpdateItemRequest(TableName = tableName, 
                          Key = keyAttribute, 
                          UpdateExpression = updateExpression, 
                          ExpressionAttributeValues = eav)
    
    static member UpdateTable (dynamoDbClient: IAmazonDynamoDB) (request: UpdateItemRequest) = 
        async {
            let! response = dynamoDbClient.UpdateItemAsync(request) |> Async.AwaitTask
            return response
        }

    member __.FunctionHandler (request: APIGatewayProxyRequest) (context: ILambdaContext) =
        context.Logger.Log(sprintf "HttpMethod: %s, Path: %s" request.HttpMethod request.Path)
        let tableName = System.Environment.GetEnvironmentVariable("TABLE_NAME")
        let response = 
            if request.HttpMethod <> "GET" then
                APIGatewayProxyResponse(StatusCode = 400)
            else
                let dbrequest = 
                    Function.CreateUpdateItemRequest tableName request.Path
                let dbresponse = 
                    (Function.UpdateTable dynamoDbClient dbrequest) 
                    |> Async.RunSynchronously
                let statusCode = dbresponse.HttpStatusCode.ToString()
                context.Logger.Log(sprintf "DynamoDB response code: %s" statusCode)
                APIGatewayProxyResponse(
                    StatusCode = 
                        if dbresponse.HttpStatusCode = HttpStatusCode.OK then 
                            200 
                        else 
                            555)
        context.Logger.Log(sprintf "Response statusCode %d" response.StatusCode)
        response