using Proyecto26;
using RSG;

public class StackAPIManager
{
    private const string apiURL = "https://ga1vqcu3o1.execute-api.us-east-1.amazonaws.com/Assessment/stack";

    public IPromise<StackBlock[]> FetchStackData()
    {
        Promise<StackBlock[]> promise = new();

        RestClient.GetArray<StackBlock>(apiURL).Then(response =>
        {
            StackBlock[] stackBlocks = response;
            promise.Resolve(stackBlocks);
        }).Catch(error =>
        {
            promise.Reject(error);
        });

        return promise;
    }
}