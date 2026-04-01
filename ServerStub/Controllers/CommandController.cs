using Contracts.Dto;
using Contracts.Dto.GetMenu;
using Contracts.Dto.SendOrder;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using ServerStub.Extensions;

namespace ServerStub.Controllers
{
    [ApiController]
    [Route("[controller]")]
    [Authorize]
    public class CommandController : ControllerBase
    {
        private static readonly GetMenuData GenericData = new(
            [
            new("1", "артикль", "им€", 300, true, "path", ["code1", "code2"]),
            new("2", "артикль", "им€", 300, true, "path", ["code1", "code2"]),
            ]);

        private static readonly GetMenuData GenericDataNoPrice = new(
           [
           new("1", "артикль", "им€", 0, true, "path", ["code1", "code2"]),
            new("2", "артикль", "им€", 0, true, "path", ["code1", "code2"]),
            ]);

        private readonly ILogger<CommandController> _logger;

        public CommandController(ILogger<CommandController> logger)
        {
            _logger = logger;
        }

        [HttpPost(Name = "GetCommandResult")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        public async Task<ActionResult> GetCommandResult(CommandDto<object> command)
        {
            //¬ теории здесь все через сервисы делаем, но в рамках заглушки логику оставл€ем в контроллере
            try
            {
                var paramsJson = (System.Text.Json.JsonElement)command.CommandParameters;
                switch (command.Command)
                {
                    case "GetMenu":
                        {
                            var commandParams = paramsJson.ToObject<GetMenuCommandParamsDto>();

                            var res = new CommandResultDto<GetMenuData>
                            {
                                Command = command.Command,
                                Success = true,
                                ErrorMessage = string.Empty,
                                Data = commandParams.WithPrice ? GenericData : GenericDataNoPrice
                            };
                            return Ok(res);
                        }
                    case "SendOrder":
                        {
                            //ѕо сути тут просто валидаци€ запроса, скажем так
                            var commandParams = paramsJson.ToObject<SendOrderCommandParamsDto>();

                            var res = new CommandResultNoDataDto
                            {
                                Command = command.Command,
                                Success = true,
                                ErrorMessage = string.Empty
                            };
                            return Ok(res);
                        }
                    default: throw new Exception("Ќеизвестна€ команда");
                }
            }
            catch (Exception ex)
            {
                var res = new CommandResultNoDataDto
                {
                    Command = command.Command,
                    Success = false,
                    ErrorMessage = ex.Message
                };

                return Ok(res);
            }

        }
    }
}
