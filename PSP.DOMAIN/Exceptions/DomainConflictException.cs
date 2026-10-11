namespace PSP.DOMAIN.Exceptions
{
    public sealed class DomainConflictException : DomainException
    {
        public DomainConflictException(string code, string message) : base(code, message)
        {
        }
    }
}
