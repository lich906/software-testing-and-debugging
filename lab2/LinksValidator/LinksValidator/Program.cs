using System;
using System.Threading.Tasks;

namespace LinksValidator
{
    class Program
    {
        private static readonly LinksCollector collector = new LinksCollector();
        static async Task<int> Main(string[] args)
        {
            try
            {
                var uri = ParseArguments(args);
                return await collector.Collect(uri);
            }
            catch (Exception e)
            {
                Console.ForegroundColor = ConsoleColor.Gray;
                Console.BackgroundColor = ConsoleColor.DarkRed;

                Console.WriteLine(e.Message);

                Console.ResetColor();
                return 1;
            }
        }

        static Uri ParseArguments(string[] args)
        {
            if (args.Length != 1)
            {
                throw new ApplicationException($"Invalid arguments count.\nUsage: LinksValidator.exe URL");
            }

            var uri = new Uri(args[0]);
            var baseUri = new Uri(uri.GetLeftPart(System.UriPartial.Authority));

            return baseUri;
        }
    }
}
