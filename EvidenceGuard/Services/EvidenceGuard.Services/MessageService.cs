using EvidenceGuard.Services.Interfaces;

namespace EvidenceGuard.Services
{
    public class MessageService : IMessageService
    {
        public string GetMessage()
        {
            return "Hello from the Message Service";
        }
    }
}
