namespace Contracts.Dto.SendOrder
{
    public record struct SendOrderCommandParamsDto(Guid OrderId, IReadOnlyCollection<SendOrderMenuItemDto> MenuItems);
}
