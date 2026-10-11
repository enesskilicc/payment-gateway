namespace PSP.DOMAIN.Exceptions
{
    public sealed class DomainValidationException : DomainException
    {
        public DomainValidationException(string code, string message) : base(code, message)
        {
        }
    }
}
