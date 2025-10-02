namespace FactOfHuman.Response
{
    public class AppException : Exception
    {
        public int StatusCode { get; }

        protected AppException(string message, int statusCode) : base(message)
        {
            StatusCode = statusCode;
        }
    }
    // Sai mật khẩu hoặc email
    public class InvalidCredentialsException : AppException
    {
        public InvalidCredentialsException(string message = "Sai mật khẩu hoặc email")
            : base(message, StatusCodes.Status401Unauthorized) { }
    }

    // Email đã tồn tại
    public class EmailAlreadyExistsException : AppException
    {
        public EmailAlreadyExistsException(string message = "Email đã tồn tại")
            : base(message, StatusCodes.Status409Conflict) { }
    }

    // Username đã tồn tại
    public class UsernameAlreadyExistsException : AppException
    {
        public UsernameAlreadyExistsException(string message = "Username đã tồn tại")
            : base(message, StatusCodes.Status409Conflict) { }
    }

    // User hoặc Email không tồn tại
    public class UserNotFoundException : AppException
    {
        public UserNotFoundException(string message = "User hoặc Email không tồn tại")
            : base(message, StatusCodes.Status404NotFound) { }
    }

    // Password sai
    public class WrongPasswordException : AppException
    {
        public WrongPasswordException(string message = "Password sai")
            : base(message, StatusCodes.Status401Unauthorized) { }
    }
    // Chưa kích hoạt tài khoản
    public class UserNotActiveException : AppException
    {
        public UserNotActiveException(string message = "Chưa kích hoạt tài khoản")
            : base(message, StatusCodes.Status403Forbidden) { }
    }
    // Đã kích hoạt tài khoản
    public class UserAlreadyActiveException : AppException
    {
        public UserAlreadyActiveException(string message = "Tài khoản đã được kích hoạt")
            : base(message, StatusCodes.Status400BadRequest) { }
    }
}
