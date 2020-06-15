using System;
using System.Runtime.Serialization;

namespace ApplyFunctional.Logic.Common
{
    [Serializable]
    public class BusinessException : Exception
    {
        public BusinessException()
        {
        }

        public BusinessException(string message) : base(message)
        {
        }

        public BusinessException(string message, Exception exception) : base(message, exception)
        {
        }

        protected BusinessException(SerializationInfo serializationInfo, StreamingContext streamingContext) :
            base(serializationInfo, streamingContext)
        {
        }
    }
}