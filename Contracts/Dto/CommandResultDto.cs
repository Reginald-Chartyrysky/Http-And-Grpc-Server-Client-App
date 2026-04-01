namespace Contracts.Dto
{
    public record struct CommandResultDto<T>(string Command, bool Success, string ErrorMessage, T? Data): ISuccessResult;
}
