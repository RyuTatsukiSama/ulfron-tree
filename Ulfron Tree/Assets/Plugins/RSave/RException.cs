using System;

namespace RException
{

    public class LoadFileNotFound : Exception
    {
        public LoadFileNotFound()
        {

        }

        public LoadFileNotFound(string message)
            : base(message)
        {

        }

        public LoadFileNotFound(string message, Exception inner)
            : base(message, inner)
        {

        }
    }
}
