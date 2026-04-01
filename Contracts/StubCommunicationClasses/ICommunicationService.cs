namespace Contracts.StubCommunicationClasses
{
    public interface ICommunicationService: IDisposable
    {
        public Task<IEnumerable<MenuItem>> GetMenu(bool withPrice);
        public Task SendOrder(Order order);
    }
}
