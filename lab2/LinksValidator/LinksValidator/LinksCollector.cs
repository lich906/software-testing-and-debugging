using System;
using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;

namespace LinksValidator
{
    class LinksCollector
    {
        private static readonly string ValidLinksFileName = "valid-links.txt";
        private static readonly string BrokenLinksFileName = "broken-links.txt";

        private readonly StreamWriter ValidLinksFileStream = new StreamWriter(ValidLinksFileName);
        private readonly StreamWriter BrokenLinksFileStream = new StreamWriter(BrokenLinksFileName);

        private uint ValidLinksCount;
        private uint BrokenLinksCount;

        private readonly NetworkClient networkClient = new NetworkClient();
        private readonly LinksParser linksParser = new LinksParser();
        readonly HashSet<string> visitedLinks = new HashSet<string>();

        public async Task<int> Collect(Uri baseUri)
        {
            var (content, statusCode) = await networkClient.SendRequest(baseUri);

            if (!CheckAndWriteLinksToFile(baseUri.ToString(), statusCode))
            {
                return 0;
            }

            List<Uri> parsedUris = linksParser.Parse(content, baseUri);
            Queue<Uri> queue = new Queue<Uri>();
            EnqueueUnvisitedLinks(parsedUris, queue, baseUri);

            do
            {
                var currLink = queue.Dequeue();
                var (response, status) = await networkClient.SendRequest(currLink);
                if (CheckAndWriteLinksToFile(currLink.ToString(), status))
                {
                    parsedUris = linksParser.Parse(response, baseUri);
                    EnqueueUnvisitedLinks(parsedUris, queue, baseUri);
                }
            } while (queue.Count != 0);

            ValidLinksFileStream.WriteLine($"Total links: {ValidLinksCount}");
            ValidLinksFileStream.WriteLine("Last checked: " + DateTime.Now.ToString());
            BrokenLinksFileStream.WriteLine($"Total links: {BrokenLinksCount}");
            BrokenLinksFileStream.WriteLine("Last checked: " + DateTime.Now.ToString());

            ValidLinksFileStream.Close();
            BrokenLinksFileStream.Close();

            return 0;
        }

        bool CheckAndWriteLinksToFile(string link, System.Net.HttpStatusCode status)
        {
            if (((int)status) >= 200 && ((int)status) <= 299)
            {
                ValidLinksFileStream.WriteLine(((int)status).ToString() + '\t' + link.ToString());
                ValidLinksCount++;
                return true;
            }
            else
            {
                BrokenLinksFileStream.WriteLine(((int)status).ToString() + '\t' + link.ToString());
                BrokenLinksCount++;
                return false;
            }
        }

        private void EnqueueUnvisitedLinks(List<Uri> uris, Queue<Uri> queue, Uri baseUri)
        {
            foreach (var uri in uris)
            {
                if (!baseUri.IsBaseOf(uri))
                {
                    continue;
                }

                var link = uri.ToString();
                if (!visitedLinks.Contains(link))
                {
                    visitedLinks.Add(link);
                    queue.Enqueue(uri);
                }
            }
        }
    }
}
