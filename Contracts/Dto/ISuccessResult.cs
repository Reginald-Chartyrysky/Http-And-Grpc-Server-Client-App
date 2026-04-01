namespace Contracts.Dto
{
    public interface ISuccessResult
    {
        public bool Success { get; }
        public string ErrorMessage { get; }
    }
}
