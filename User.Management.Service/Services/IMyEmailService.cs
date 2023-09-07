using User.Management.Service.Models;

namespace User.Management.Service.Services
{
    public interface IMyEmailService
    {
        void SendEmail(Message message);
    }
}
