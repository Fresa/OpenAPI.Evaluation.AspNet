namespace OpenAPI.Evaluation.AspNet;

/// <summary>
/// Evaluation options for the <see cref="OpenApiEvaluationMiddleware"/>
/// </summary>
public sealed record OpenApiEvaluationMiddlewareOptions
{
    /// <summary>
    /// Throws a <see cref="OpenApiEvaluationException"/> on failed request evaluation rather than returning a <see cref="EvaluationHttpResponseMessage"/>
    /// Does nothing if <see cref="EvaluateRequests"/> is set to false.
    /// Default: false
    /// </summary>
    public bool ThrowOnRequestEvaluationFailure { get; init; } = false;
    /// <summary>
    /// Throws a <see cref="OpenApiEvaluationException"/> on failed response evaluation rather than returning a <see cref="EvaluationHttpResponseMessage"/>
    /// If response headers have already been sent this exception should be caught further downstream as nothing else can be written to the response.
    /// Does nothing if <see cref="EvaluateResponses"/> is set to false.
    /// Default: true
    /// </summary>
    public bool ThrowOnResponseEvaluationFailure { get; init; } = true;
    /// <summary>
    /// Evaluates <see cref="HttpRequestMessage"/>
    /// Default: true
    /// </summary>
    public bool EvaluateRequests { get; init; } = true;
    /// <summary>
    /// Evaluates <see cref="HttpResponseMessage"/>. Will fall back on throwing if response headers have already been sent.
    /// Default: false
    /// </summary>
    public bool EvaluateResponses { get; init; } = false;
}