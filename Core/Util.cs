using System;

namespace Core
{

    public static class Util
    {
        public static string GetMessageFromArgs(string[] args)
        {
            return args.Length > 0 ? String.Join(" ", args) : "Hello world!";
        }
    }
}