namespace CleanArchTemplate.Api.Middlewares;

public class ClaimsValidationMiddleware
{
    private readonly RequestDelegate _next;

    public ClaimsValidationMiddleware(RequestDelegate next)
    {
        _next = next;
    }

    public async Task InvokeAsync(HttpContext context)
    {
        // Validate that the JWT contains the required claims
        /*if (context.User?.Identity?.IsAuthenticated == true)
        {
            var name = context.User.FindFirst("name")?.Value;
            var email = context.User.FindFirst(ClaimTypes.Email)?.Value;

            if (string.IsNullOrWhiteSpace(name) || string.IsNullOrWhiteSpace(email))
            {
                context.Response.StatusCode = StatusCodes.Status401Unauthorized;
                await context.Response.WriteAsJsonAsync(new ProblemDetails
                {
                    Status = StatusCodes.Status401Unauthorized,
                    Title = "Unauthorized",
                    Detail = "Token JWT must have claims 'name' and 'email'."
                });
                return;
            }
        }*/

        await _next(context);
    }
}
