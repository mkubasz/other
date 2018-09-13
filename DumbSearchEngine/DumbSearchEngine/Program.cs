using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DumbSearchEngine
{
    class Program
    {
        static readonly Index _index = new Index();
        static readonly Dictionary<string, IExtractor> _extractors = new Dictionary<string,IExtractor>()
            {
                [".docx"] = new DocxExtractor()
            };
        static void Main(string[] args)
        {

            Console.WriteLine("Hello!");
            Console.WriteLine("Supported commands:");
            Console.WriteLine("index <path>     - indexes a given document or folder (only .docx supported)");
            Console.WriteLine("find <query>     - searches the index");
            Console.WriteLine("q                - exits");

            do
            {
                Console.Write("> ");
                var command = Console.ReadLine().Split(new char[] { ' ' }, 2);
                switch (command[0])
                {
                    case "q":
                        return;
                    case "index":
                        if(command.Length == 1)
                        {
                            Console.WriteLine("Missing argument!");
                            break;
                        }
                        Index(command[1]);
                        break;
                    case "find":
                        if (command.Length == 1)
                        {
                            Console.WriteLine("Missing argument!");
                            break;
                        }
                        foreach (var item in _index.Find(command[1]))
                        {
                            Console.WriteLine("[{0}] {1}", item.Score, item.Document.Path);
                        }
                        break;
                    default:
                        Console.WriteLine("Unsupported command!");
                        break;
                }

            }
            while (true);
        }

        private static void Index(string path)
        {
            try
            {
                var extractor = GetMatchingExtractor(path);
                if (extractor != null)
                {
                    Console.WriteLine("Indexing 1 file.");
                    _index.AddDocument(extractor.Extract(path));
                    Console.WriteLine("Indexed: {0}", path);
                }
                else if (Directory.Exists(path))
                {
                    var pattern = _extractors.Keys.Select(k => $"*{k}").Aggregate((a, b) => $"{a};{b}");
                    var files = Directory.GetFiles(path, pattern);
                    Console.WriteLine("Indexing {0} file(s).", files.Count());
                    foreach (string file in files)
                    {
                        _index.AddDocument(GetMatchingExtractor(file).Extract(file));
                        Console.WriteLine("Indexed: {0}", file);
                    }
                }
                else
                {
                    Console.WriteLine("Not found.");
                }
            }
            catch
            {
                Console.WriteLine("Error.");
            }
        }

        private static IExtractor GetMatchingExtractor(string path)
        {
            KeyValuePair<string,IExtractor>? kv = _extractors.FirstOrDefault(x => path.EndsWith(x.Key));
            return kv?.Value;
        }
    }
}
