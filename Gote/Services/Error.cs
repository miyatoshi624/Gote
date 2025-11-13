namespace Gote.Services
{
    internal sealed class Error
    {
        private readonly string _code;
        private readonly string _message;

        public Error(string code, string message)
        {
            _code = code;
            _message = message;
        }

        public string Code => _code;

        public string Message => _message;
    }
}
