namespace PSP.DOMAIN.Exceptions
{
    public abstract class DomainException : Exception
    {
        public string Code { get;}

        protected DomainException(string code , string message) : base(message)  
        {
            Code = code;
        }
    }
}
