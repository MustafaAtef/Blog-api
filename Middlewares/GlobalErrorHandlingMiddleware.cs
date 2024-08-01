using BlogApi.Exceptions;
using Microsoft.AspNetCore.Mvc;
using System.Net;
using System.Text.Json;

namespace BlogApi.Middlewares {
    public class GlobalErrorHandlingMiddleware {
        private readonly RequestDelegate _next;

        public GlobalErrorHandlingMiddleware(RequestDelegate next)
        {
            _next = next;
        }

        public async Task InvokeAsync(HttpContext context) {
            try {
                await _next(context);

            } 
            catch(NotAuthenticatedException) {
                context.Response.StatusCode = (int)HttpStatusCode.Unauthorized;
            }
            catch (NotAuthorizedException) {
                context.Response.StatusCode = (int)HttpStatusCode.Forbidden;
            }
            catch(BadRequestException ex) {
                context.Response.StatusCode = (int)HttpStatusCode.BadRequest;
                context.Response.ContentType = "application/json";
                var res = new ProblemDetails { Type="Bad Request",Title = "Invalid Data has been sent",Detail = ex.Message, Status = (int?)HttpStatusCode.BadRequest };
                var resAsJson = JsonSerializer.Serialize(res);
                await context.Response.WriteAsync(resAsJson);
            }

            catch(UniqueEntityException ex) {
                context.Response.StatusCode = (int)HttpStatusCode.BadRequest;
                context.Response.ContentType = "application/json";
                var errors = new Dictionary<string, string[]>();
                foreach (var prop in ex.UniqueProps) {
                    errors.Add(prop, new string[] { $"There is a {ex.EntityName} with this {prop}" });
                }
                var res = new ValidationProblemDetails(errors) { Status = (int)HttpStatusCode.BadRequest};
                var resAsJson = JsonSerializer.Serialize(res);
                await context.Response.WriteAsync(resAsJson);

            } catch (Exception) {
                context.Response.StatusCode = (int)HttpStatusCode.InternalServerError;
            }
        }
    }
}
