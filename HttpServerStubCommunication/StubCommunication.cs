using HttpServerStubCommunication.Classes;
using Contracts.StubCommunicationClasses;

namespace HttpServerStubCommunication
{
    public static class StubCommunication
    {
        private static readonly CommunicationService _service = new();
        public static ICommunicationService GetCommunicationService(string userName, string password, string baseUri) 
        {
            _service.SetSettings($"{userName}:{password}", baseUri);
            return _service;
        }
    }
}
