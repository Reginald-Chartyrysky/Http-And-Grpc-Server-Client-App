namespace Contracts.Dto.GetMenu
{
    public record struct GetMenuItemDto(string Id, string Article, string Name, double Price, bool IsWeighted, string FullPath, IReadOnlyCollection<string> BarCodes);
}
