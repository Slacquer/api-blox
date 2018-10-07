#region -    Using Statements    -

using System.Threading.Tasks;

#endregion

namespace DemoApi2.Application.Contracts
{
    public interface IEmailService
    {
        Task SendAsync(string to, string from, string subject, string body);
    }
}
