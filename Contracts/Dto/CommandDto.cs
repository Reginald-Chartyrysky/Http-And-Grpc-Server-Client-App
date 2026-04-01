namespace Contracts.Dto
{
    public record struct CommandDto<T>(string Command, T CommandParameters);
}
