using System.Threading.Tasks;

namespace DemoApi2.Application.Contracts
{
    public interface IEmailService
    {
        Task SendAsync(string to, string from, string subject, string body);
    }
}
