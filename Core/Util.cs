using System;

namespace Core
{

    public static class Util
    {
        public static string GetMessageFromArgs(string[] args, string defaultMessage = "Hello world!")
        {
            return args.Length > 0 ? String.Join(" ", args) : defaultMessage;
        }
    }
}