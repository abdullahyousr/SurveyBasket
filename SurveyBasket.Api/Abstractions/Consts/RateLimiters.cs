namespace SurveyBasket.Api.Abstractions.Consts;

public static class RateLimiters
{
    public const string IpLimit = "iplimit";
    public const string UserLimiter = "userlimit";
    public const string Concurrency = "concurrency";
    public const string TokenBucket = "tokenbucket";
    public const string FixedWindow = "fixed";
    public const string SlidingWindow = "sliding";
}
