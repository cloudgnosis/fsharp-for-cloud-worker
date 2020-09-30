namespace Backend


open Amazon.Lambda.Core
open Amazon.Lambda.APIGatewayEvents


// Assembly attribute to enable the Lambda function's JSON input to be converted into a .NET class.
[<assembly: LambdaSerializer(typeof<Amazon.Lambda.Serialization.SystemTextJson.DefaultLambdaJsonSerializer>)>]
()


type Function() =
    member __.FunctionHandler (request: APIGatewayProxyRequest) (context: ILambdaContext) =
        context.Logger.Log(sprintf "HttpMethod: %s, Path: %s" request.HttpMethod request.Path)
        let response = APIGatewayProxyResponse(StatusCode = if request.HttpMethod = "GET" then 200 else 400)
        context.Logger.Log(sprintf "Response statusCode %d" response.StatusCode)
        response