using System;
using System.Collections.Generic;
using HtmlAgilityPack;

namespace LinksValidator
{
    class LinksParser
    {
        public List<Uri> Parse(string html, Uri baseUri)
        {
            if (html.Length == 0)
            {
                return new List<Uri>();
            }

            List<string> allFoundLinks = GetLinksFromHtmlDom(html);
            return FilterLinks(allFoundLinks, baseUri);
        }

        private List<string> GetLinksFromHtmlDom(string html)
        {
            HtmlDocument document = new HtmlDocument();
            document.LoadHtml(html);

            var allFoundLinks = new List<string>();

            var linkNodes = document.DocumentNode.SelectNodes("//a[@href]");
            if (linkNodes != null) // If document have links
            {
                foreach (HtmlNode link in linkNodes)
                {
                    HtmlAttribute att = link.Attributes["href"];
                    allFoundLinks.Add(att.Value);
                }
            }

            return allFoundLinks;
        }

        private List<Uri> FilterLinks(List<string> links, Uri baseUri)
        {
            List<Uri> internalLinks = new List<Uri>();

            foreach (var rawLink in links)
            {
                try
                {
                    var link = RemoveQueryStringAndAnchor(rawLink);

                    if (link.Length == 0)
                    {
                        continue;
                    }
                    else
                    {
                        Uri uri;
                        if (TryCreateUri(baseUri, link, out uri))
                        {
                            internalLinks.Add(uri);

                            Console.ForegroundColor = ConsoleColor.DarkGreen;
                            Console.WriteLine(uri);
                            Console.ResetColor();
                        }
                        else
                        {
                            Console.ForegroundColor = ConsoleColor.Red;
                            Console.WriteLine(link);
                            Console.ResetColor();
                        }
                    }
                }
                catch (UriFormatException e)
                {
                    Console.ForegroundColor = ConsoleColor.Gray;
                    Console.BackgroundColor = ConsoleColor.DarkRed;
                    Console.WriteLine(e.Message);
                    Console.ResetColor();
                }
            }

            return internalLinks;
        }

        private string RemoveQueryStringAndAnchor(string uri)
        {
            var uri1 = uri.Split('#')[0];
            var uri2 = uri1.Split('?')[0];
            return uri2;
        }

        bool TryCreateUri(Uri baseUri, string candidateUri, out Uri? result)
        {
            // Trying create uri assuming it's absolute path
            Uri.TryCreate(candidateUri, UriKind.Absolute, out result);

            if (result == null)
            {
                // Trying create uri assuming it's relative path
                Uri.TryCreate(baseUri, candidateUri, out result);

                if (result == null)
                {
                    // If failed both attempts to create
                    return false;
                }
            }
            else if (baseUri.Host != result.Host) // If we created uri from absolute path but it belongs to different host
            {
                result = null;
                return false;
            }

            return true;
        }
    }
}
