namespace Contracts.Dto
{
    public record struct CommandResultNoDataDto(string Command, bool Success, string ErrorMessage): ISuccessResult;
}
