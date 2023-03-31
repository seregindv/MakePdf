using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using MakePdf.Helpers;

namespace MakePdf.Galleries.Processors
{
    public struct FileSet
    {
        public string Title { set; get; }
        public IEnumerable<string> Files { set; get; }
    }

    public struct NumberNamePair
    {
        public string Name { set; get; }
        public int Number { set; get; }
    }

    public class FileListProcessor
    {
        public IEnumerable<FileSet> Process(string[] files)
        {
            var listFiles = GetListFiles(files).ToArray();

            if (listFiles.Length > 0)
            {
                var localities = GetLocalityMap(listFiles)
                    .Select((l, i) => new { Index = i, Locality = l })
                    .ToArray();
                var regex = new Regex(@"(\d+)");
                var fileNums = (from file in files
                                select new { Match = regex.Match(file), File = file }
                                    into match
                                    where match.Match.Success
                                    select new NumberNamePair
                                    {
                                        Number = Int32.Parse(match.Match.Groups[1].Value),
                                        Name = match.File
                                    }).ToArray();
                return localities.Select(l => l.Locality.Name).Distinct().SelectMany(
                    localityName =>
                    {
                        var name = localityName;
                        return localities
                            .Where(locality => locality.Locality.Name == name)
                            .Select(locality =>
                                new FileSet
                                {
                                    Files =
                                        fileNums.Where(file =>
                                        {
                                            var lowerNumber = locality.Index == 0
                                                ? 0
                                                : localities[locality.Index - 1].Locality.Number;
                                            var upperNumber = locality.Index == localities.Length - 1
                                                ? Int32.MaxValue
                                                : localities[locality.Index + 1].Locality.Number;
                                            return file.Number > lowerNumber && file.Number < upperNumber && PathHelper.IsImage(file.Name) && File.Exists(file.Name);
                                        }).Select(fileNum => fileNum.Name),
                                    Title = locality.Locality.Name
                                });
                    }
                    );
            }
            return new[]{
                new FileSet
                {
                    Title = String.Empty,
                    Files = files.Where(file => File.Exists(file) && PathHelper.IsImage(file))
                }};
        }

        private IEnumerable<string> GetListFiles(IEnumerable<string> files)
        {
            return files.Where(file => ListFiles.Contains(Path.GetFileName(file)));
        }

        private IEnumerable<NumberNamePair> GetLocalityMap(IEnumerable<string> localityFiles)
        {
            var regex = new Regex(@"^(\d+)\s+(.*)");
            return localityFiles
                .SelectMany(file => File.ReadAllLines(file, Encoding.Default))
                .Select(line => regex.Match(line))
                .Where(match => match.Success)
                .Select(match => new NumberNamePair
                {
                    Name = match.Groups[2].Value,
                    Number = Int32.Parse(match.Groups[1].Value)
                })
                .OrderBy(locality => locality.Number);
        }

        private string[] ListFiles { get { return new[] { "what.txt" }; } }
    }
}
