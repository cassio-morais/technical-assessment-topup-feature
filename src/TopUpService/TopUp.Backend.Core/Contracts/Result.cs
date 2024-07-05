namespace Backend.TopUp.Core.Contracts
{
    public class Result<T>
    {
        public bool HasError => ErrorMessage is not null;

        public string? ErrorMessage { get; private set; }

        public T? Data { get; private set; }

        private Result(T data, string errorMessage)
        {
            Data = data;
            ErrorMessage = errorMessage;
        }

        public static Result<T> Ok(T data)
        {
            return new Result<T>(data, null!);
        }

        public static Result<T> Error(string errorMessage)
        {
            return new Result<T>(default!, errorMessage);
        }
    }
}
