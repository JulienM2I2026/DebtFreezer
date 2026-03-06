using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using System.Net.Http.Headers;

namespace Gateway.Filter
{
    [AttributeUsage(AttributeTargets.Class | AttributeTargets.Method)]
    public class RequireValidTokenAttribute : Attribute { }
    public class TokenValidationFilter : IAsyncActionFilter
    {
        private readonly IHttpClientFactory _httpClientFactory;
        private readonly IConfiguration _config;
        ILogger<TokenValidationFilter> _logger;


        public TokenValidationFilter(
            IHttpClientFactory httpClientFactory,
            IConfiguration config,
            ILogger<TokenValidationFilter> logger
            )
        {
            _httpClientFactory = httpClientFactory;
            _config = config;
            _logger = logger;
        }

        public async Task OnActionExecutionAsync(ActionExecutingContext context, ActionExecutionDelegate next)
        {

            var hasAttribute = context.ActionDescriptor.EndpointMetadata.OfType<RequireValidTokenAttribute>().Any();

            if (!hasAttribute)
            {
                await next();
                return;
            }

            var token = context.HttpContext.Request.Headers["Authorization"].FirstOrDefault();

            if (string.IsNullOrWhiteSpace(token))
            {
                context.Result = new UnauthorizedObjectResult(new { error = "Token manquant " });
                return;
            }

            var isValid = await ValidateTokenAsync(token);

            if (!isValid)
            {
                _logger.LogWarning("Token Refusé pour {Action}", context.ActionDescriptor.DisplayName);
                context.Result = new UnauthorizedObjectResult(new { error = "Token invalide ou experé" });
                return;
            }

            await next();

        }


        private async Task<bool> ValidateTokenAsync(string token)
        {
            try
            {
                var baseUrl = _config["AuthService:BaseUrl"] ?? throw new InvalidOperationException("AuthService:BaseUrl non configuré.");

                if (token.StartsWith("Bearer "))
                {
                    token = token.Substring(7);
                }

                var client = _httpClientFactory.CreateClient("AuthService");
                client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);

                var reponse = await client.GetAsync($"{baseUrl}/ping");
                return reponse.IsSuccessStatusCode;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Erreur de la valition du token");
                return false;
            }
        }

    }


}
