using Contracts.StubCommunicationClasses;
using Grpc.Net.Client;
namespace GrpcServerStubCommunication.Classes
{
    internal class CommunicationService: ICommunicationService
    {
        private GrpcChannel? _channel = null;

        internal void SetSettings(string baseUri)
        {
            _channel = GrpcChannel.ForAddress(baseUri);
        }
        public async Task<IEnumerable<Contracts.StubCommunicationClasses.MenuItem>> GetMenu(bool withPrice)
        {
            var client = new SmsTestService.SmsTestServiceClient(_channel);

            var res = await client.GetMenuAsync(new Google.Protobuf.WellKnownTypes.BoolValue { Value = withPrice });

            if (res.Success == false)
            {
                throw new Exception(res.ErrorMessage);
            }

            return res.MenuItems.Select(x => new Contracts.StubCommunicationClasses.MenuItem() { 
                Id= x.Id, 
                Article = x.Article, 
                BarCodes = [.. x.Barcodes], 
                FullPath = x.FullPath, 
                IsWeighted = x.IsWeighted, 
                Name = x.Name, 
                Price = x.Price});
        }

        public async Task SendOrder(Contracts.StubCommunicationClasses.Order order)
        {
            var client = new SmsTestService.SmsTestServiceClient(_channel);

            var gRpcOrder = new Order() 
            { 
                Id = order.Id.ToString()
            };
            gRpcOrder.OrderItems.AddRange(order.MenuItems.Select(x => new OrderItem() { Id = x.Id, Quantity = x.Quantity }));

            var res = await client.SendOrderAsync(gRpcOrder);

            if(res.Success == false)
            {
                throw new Exception(res.ErrorMessage);
            }
        }

        public void Dispose()
        {
            GC.SuppressFinalize(this);
            _channel?.Dispose();
        }
    }
}
