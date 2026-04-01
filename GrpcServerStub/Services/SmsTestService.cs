using Google.Protobuf.WellKnownTypes;
using Grpc.Core;
using static GrpcServerStub.SmsTestService;

namespace GrpcServerStub.Services
{
    public class SmsTestService: SmsTestServiceBase
    {
        private static GetMenuResponse _genericMenuData = new() { Success = true, ErrorMessage = ""};
        private static List<MenuItem> _itemsWithPrice = [
            new() { Id = "1", Article = "articel1", FullPath = "path", IsWeighted = true, Name = "name", Price = 300},
            new() { Id = "2", Article = "articel2", FullPath = "path", IsWeighted = true, Name = "name", Price = 200},
            new() { Id = "3", Article = "articel3", FullPath = "path", IsWeighted = true, Name = "name", Price = 100}];

        private static List<MenuItem> _itemsWithNoPrice = [
            new() { Id = "1", Article = "articel1", FullPath = "path", IsWeighted = true, Name = "name", Price = 0},
            new() { Id = "2", Article = "articel2", FullPath = "path", IsWeighted = true, Name = "name", Price = 0},
            new() { Id = "3", Article = "articel3", FullPath = "path", IsWeighted = true, Name = "name", Price = 0}];

        private readonly ILogger<SmsTestService> _logger;
        public SmsTestService(ILogger<SmsTestService> logger)
        {
            _logger = logger;
        }

        public override Task<GetMenuResponse> GetMenu(BoolValue withPrice, ServerCallContext context)
        {
            var items = withPrice.Value? _itemsWithPrice : _itemsWithNoPrice;
            _genericMenuData.MenuItems.AddRange(items);

            return Task.FromResult(_genericMenuData);
        }

        public override Task<SendOrderResponse> SendOrder(Order order, ServerCallContext context)
        {
            if (order.OrderItems.Any(oi => !_itemsWithPrice.Any(i=> i.Id != oi.Id)))
            {
                return Task.FromResult(new SendOrderResponse() { Success = false, ErrorMessage = "Не все id присутствуют в базе данных" });
            }

            if (order.OrderItems.Any(o=>o.Quantity <= 0))
            {
                return Task.FromResult(new SendOrderResponse() { Success = false, ErrorMessage = "Не все количества были больше 0" });
            }

            return Task.FromResult(new SendOrderResponse() { Success = true, ErrorMessage = ""});
        }
    }
}
