namespace Application.Core
{
    // Result will be used for any entity (e.g. Activity), need a generic type <T>
    public class Result<T>
    {
        public bool IsSuccess { get; set; }
        public T Value { get; set; }
        public string Error { get; set; }

        // add 2 static methods that will return result of type <T>: used for API Error Handling (400, 404 etc.)
        public static Result<T> Success(T value) => new Result<T> { IsSuccess = true, Value = value };
        public static Result<T> Failure(string error) => new Result<T> { IsSuccess = false, Error = error };
    }
}