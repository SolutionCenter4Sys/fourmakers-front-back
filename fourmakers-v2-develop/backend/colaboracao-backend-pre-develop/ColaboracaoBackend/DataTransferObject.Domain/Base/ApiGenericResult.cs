namespace DataTransferObject.Domain.Base
{
    public class ApiGenericResult<T> : StatusResult
    {
        public T Retorno { get; set; }
    }
    public class ApiGenericResult : StatusResult
    {
    }
}