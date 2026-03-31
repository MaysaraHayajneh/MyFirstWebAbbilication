namespace WebApplication1.CustomExceptions
{
    public class BadRequest : Exception
    {
        public BadRequest(string message) : base(message)
        {

        }
        public BadRequest()
        {

        }
    }
}
