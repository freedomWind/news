namespace NewsFramework.Services.Content.Http
{
    public enum ContentHttpErrorKind
    {
        None,
        InvalidRequest,
        EmptyResponse,
        Network,
        Timeout,
        HttpStatus,
        Unauthorized,
        NotFound,
        RateLimited,
        Server
    }
}
