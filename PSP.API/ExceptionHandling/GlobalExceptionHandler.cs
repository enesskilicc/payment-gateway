using Microsoft.AspNetCore.Diagnostics;
using Microsoft.AspNetCore.Mvc;
using PSP.DOMAIN.Exceptions;

namespace PSP.API.ExceptionHandling
{
    public sealed class GlobalExceptionHandler : IExceptionHandler
    {
        private readonly ILogger<GlobalExceptionHandler> _logger;
        private readonly IProblemDetailsService _problemDetailsService;
        public GlobalExceptionHandler(ILogger<GlobalExceptionHandler> logger, IProblemDetailsService problemDetailsService)
        {
            _logger = logger;
            _problemDetailsService = problemDetailsService;
        }

        public async ValueTask<bool> TryHandleAsync(HttpContext httpContext, Exception exception, CancellationToken cancellationToken)
        {
            var (statusCode, title) = exception switch
            {
                DomainValidationException => (StatusCodes.Status400BadRequest, "Geçersiz istek!"),
                DomainConflictException => (StatusCodes.Status409Conflict, "İşlem mevcut durumla çakışıyor!"),
                _ => (StatusCodes.Status500InternalServerError, "Sunucu hatası!")

            };

            if (statusCode == StatusCodes.Status500InternalServerError)
                _logger.LogError(exception, "Sunucu hatası!");
            else
                _logger.LogWarning("İstemci hatası ({StatusCode}): {Message}", statusCode, exception.Message);

            var problemDetails = new ProblemDetails
            {
                Status = statusCode,
                Title = title,
                Detail = statusCode == StatusCodes.Status500InternalServerError
                ? "İşleminiz gerçekleştirilirken beklenmeyen bir hata oluştu!"
                : exception.Message
            };

            if (exception is DomainException domainException)
                problemDetails.Extensions["code"] = domainException.Code;

            httpContext.Response.StatusCode = statusCode;

            return await _problemDetailsService.TryWriteAsync(new ProblemDetailsContext
            {
                HttpContext = httpContext,
                ProblemDetails = problemDetails,
                Exception = exception
            });
        }
    }
}
