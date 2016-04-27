using System;

namespace Atesh
{
    public class ArgumentNullOrEmptyException : ArgumentNullException
    {
        public ArgumentNullOrEmptyException() : base(Strings.ArgumentCantBeNullOrEmpty) { }
        public ArgumentNullOrEmptyException(string ParamName) : base(Strings.ArgumentCantBeNullOrEmpty, ParamName) { }
        public ArgumentNullOrEmptyException(string Message, Exception InnerException) : base(Message, InnerException) { }
        public ArgumentNullOrEmptyException(string ParamName, string Message) : base(Message, ParamName) { }
    }
}