namespace Contracts.Dto.GetMenu
{
    public record struct GetMenuData(IReadOnlyCollection<GetMenuItemDto> MenuItems);
}
