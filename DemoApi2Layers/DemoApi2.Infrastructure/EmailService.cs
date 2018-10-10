using System;
using System.Threading.Tasks;
using APIBlox.NetCore.Attributes;
using DemoApi2.Application.Contracts;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;

namespace DemoApi2.Infrastructure
{
    [InjectableService(ServiceLifetime = ServiceLifetime.Singleton)]
    public class EmailService : IEmailService
    {
        private readonly ILogger<EmailService> _log;

        public EmailService(ILoggerFactory loggerFactory)
        {
            _log = loggerFactory.CreateLogger<EmailService>();
        }

        /// <inheritdoc />
        public async Task SendAsync(string to, string from, string subject, string body)
        {
            await Task.Delay(new Random().Next(250, 1000)).ConfigureAwait(false);
            _log.LogInformation(() => $"Sent email to '{to}', from '{from}', subject '{subject}', body '{body}'");
        }
    }
}
