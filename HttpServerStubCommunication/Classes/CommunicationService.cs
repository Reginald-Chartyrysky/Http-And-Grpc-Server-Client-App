using Contracts.Dto;
using Contracts.Dto.GetMenu;
using Contracts.Dto.SendOrder;
using Contracts.StubCommunicationClasses;
using Mapster;
using System.Net.Http.Headers;
using System.Text;
using System.Text.Json;

namespace HttpServerStubCommunication.Classes
{
    internal class CommunicationService: ICommunicationService
    {
        private static readonly HttpClient _client = new(new SocketsHttpHandler
        {
            PooledConnectionLifetime = TimeSpan.FromMinutes(15) // Для резолва DNS 
        });

        private string? _authenticationString;

        private readonly static JsonSerializerOptions _jsonSerializerOptions = new() { PropertyNameCaseInsensitive = true };

        internal void SetSettings(string authenticationString, string baseUri)
        {
            _client.BaseAddress = new Uri(baseUri);
            _authenticationString = authenticationString;
        }
        public async Task<IEnumerable<MenuItem>> GetMenu(bool withPrice)
        {
            var command = new CommandDto<GetMenuCommandParamsDto>()
            {
                Command = "GetMenu",
                CommandParameters = new GetMenuCommandParamsDto(withPrice)
            };
            var commandString = JsonSerializer.Serialize(command);

            using HttpResponseMessage response = await _client.SendAsync(ConvertToRequest(commandString));
            response.EnsureSuccessStatusCode();
            string responseBody = await response.Content.ReadAsStringAsync();

            var res = JsonSerializer.Deserialize<CommandResultDto<GetMenuData>>(responseBody, _jsonSerializerOptions);

            ValidateResult(res);
            return res.Data.MenuItems.Select(x=> x.Adapt<MenuItem>());
        }

        public async Task SendOrder(Order order)
        {
            var collection = order.MenuItems.Select(x => x.Adapt<SendOrderMenuItemDto>()).ToList().AsReadOnly();

            var command = new CommandDto<SendOrderCommandParamsDto>()
            {
                Command = "SendOrder",
                CommandParameters = new SendOrderCommandParamsDto(order.Id, collection)
            };
            var commandString = JsonSerializer.Serialize(command);

            using HttpResponseMessage response = await _client.SendAsync(ConvertToRequest(commandString));
            response.EnsureSuccessStatusCode();
            string responseBody = await response.Content.ReadAsStringAsync();
            
            var res = JsonSerializer.Deserialize<CommandResultNoDataDto>(responseBody, _jsonSerializerOptions);

            ValidateResult(res);
        }

        private HttpRequestMessage ConvertToRequest(string requestJsonString)
        {
            var httpContent = new ByteArrayContent(Encoding.UTF8.GetBytes(requestJsonString));
            httpContent.Headers.Add("Content-Type", "application/json");

            var base64EncodedAuthenticationString = Convert.ToBase64String(Encoding.UTF8.GetBytes(_authenticationString));

            var requestMessage = new HttpRequestMessage(HttpMethod.Post, $"{_client.BaseAddress}Command");
            requestMessage.Headers.Authorization = new AuthenticationHeaderValue("Basic", base64EncodedAuthenticationString);
            requestMessage.Content = httpContent;

            return requestMessage;
        }

        private static void ValidateResult(ISuccessResult result)
        {
            if (result.Success == false)
            {
                throw new Exception(result.ErrorMessage);
            }
        }

        public void Dispose()
        {
            GC.SuppressFinalize(this);
            _client.Dispose();
        }
    }
}
