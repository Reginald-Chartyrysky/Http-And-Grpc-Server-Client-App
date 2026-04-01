using GrpcServerStubCommunication.Classes;
using Contracts.StubCommunicationClasses;

namespace GrpcServerStubCommunication
{
    public static class StubCommunication
    {
        private static readonly CommunicationService _service = new();
        public static ICommunicationService GetCommunicationService(string baseUri) 
        {
            _service.SetSettings(baseUri);
            return _service;
        }
    }
}
