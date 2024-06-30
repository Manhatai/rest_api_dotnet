using Serilog;

namespace MechanicsWorkshopApi.Helpers
{
    public static class ErrorHandlingHelper
    {
        public static async Task HandleGlobalErrors(HttpContext context, Func<Task> next)
        {
            try
            {
                await next.Invoke();
            }
            catch (Exception e)
            {
                await HandleExceptionAsync(context, e);
            }
        }

        private static Task HandleExceptionAsync(HttpContext context, Exception exception)
        {
            context.Response.StatusCode = 500;
            context.Response.ContentType = "application/json";
            Log.Information($"Unhandled error caught. Details => <{exception.Message}> [500]");

            var result = "Unhandled error caught. Please provide logs and use case to the development team.";
            return context.Response.WriteAsJsonAsync(result);
        }
    }
}
