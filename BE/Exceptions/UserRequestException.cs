namespace TruyenCV;
public sealed class UserRequestException : Exception
{
    public int StatusCode { get; }

    public UserRequestException(string message,string message2 = "", int statusCode = 400) : base($"{message} {message2}")
    {
        StatusCode = statusCode;
    }
}