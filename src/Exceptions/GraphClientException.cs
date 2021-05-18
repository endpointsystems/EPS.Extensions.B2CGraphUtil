using System;

namespace EPS.Extensions.B2CGraphUtil.Exceptions
{
    public class GraphClientException: ApplicationException
    {
        public GraphClientException(string message, int code) : base(message)
        {
            Code = code;
        }
        public int Code { get; set; }

    }
}
