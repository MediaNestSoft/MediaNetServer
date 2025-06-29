using System.IO;
using System.Linq;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using Emby.Naming.Video;
using MediaNetServer.Data.media.Models;
using MediaNetServer.Data.media.Services;
using TMDbLib.Objects.Movies;

namespace MediaNetServer.Services
{
    public class MovieScan
    {
        private readonly FileScanService      _parser;
        private readonly TMDbMetadataService  _tmdbService;
        private readonly MediaItemService     _repo;

        public MovieScan(
            FileScanService parser,
            TMDbMetadataService tmdbService,
            MediaItemService repo)
        {
            _parser      = parser;
            _tmdbService = tmdbService;
            _repo        = repo;
        }

        public async Task ScanAndSaveAsync(string folderPath)
        {
            foreach (var file in Directory.EnumerateFiles(folderPath, "*.*", SearchOption.AllDirectories))
            {
                var info = _parser.ParseMovieFile(file);
                var raw  = info.Name.Replace('.', ' ').Trim();

                // 检测中文，英文片名
                bool hasChinese = Regex.IsMatch(raw, @"\p{IsCJKUnifiedIdeographs}");

                string? chineseTitle = null;
                if (hasChinese)
                {
                    var cnMatch = Regex.Match(raw, @"[\p{IsCJKUnifiedIdeographs}0-9：:]+");
                    if (cnMatch.Success)
                        chineseTitle = cnMatch.Value.Trim();
                }

                var enMatch = Regex.Match(raw, @"[\p{IsBasicLatin}\-'\s,：:]+");
                string? englishTitle = enMatch.Success
                    ? enMatch.Value.Trim()
                    : null;

                var query = hasChinese && !string.IsNullOrEmpty(chineseTitle)
                    ? chineseTitle
                    : englishTitle;

                if (string.IsNullOrWhiteSpace(query))
                    continue;

                Movie? movie;
                try
                {
                    movie = await _tmdbService.GetMovieMetadataAsync(query, info.Year);
                }
                catch (System.Exception ex)
                {
                    Console.WriteLine($"查询 TMDb 失败：{ex.Message}");
                    continue;
                }

                if (movie == null)
                {
                    Console.WriteLine("未找到匹配的电影。");
                    continue;
                }

                Console.WriteLine($"找到匹配影片：{movie.Title} ({movie.ReleaseDate:yyyy})");

                // 如果已存在则跳过
                //if (await _repo.ExistsAsync(file))
                //    continue;

                // 保存到数据库
                await _repo.CreateMediaItemAsync(new MediaItem
                {
                    TMDbId      = movie.Id,
                    Title       = movie.Title,
                    Type       = "movie",
                    ReleaseDate = movie.ReleaseDate,
                    Rating      = movie.VoteAverage,
                    PosterPath  = movie.PosterPath,
                    LocalPath = folderPath + file,
                    BackdropPath = movie.BackdropPath,
                    Country = movie.ProductionCountries[1].Name
                });
            }
        }
    }
}