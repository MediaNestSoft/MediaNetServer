using System.Linq;
using System.Threading.Tasks;
using Microsoft.Extensions.Configuration;
using TMDbLib.Client;
using TMDbLib.Objects.General;
using TMDbLib.Objects.Search;
using TMDbLib.Objects.Movies;
using TMDbLib.Objects.TvShows;

namespace MediaNetServer.Services
{
    public class TMDbMetadataService
    {
        private readonly TMDbClient _client;

        public TMDbMetadataService(IConfiguration configuration)
        {
            var apiKey = "fee09865c006d213b701f3aef5629d1e";
            _client = new TMDbClient(apiKey)
            {
                DefaultLanguage      = "zh-CN",
                DefaultImageLanguage = "zh-CN"
            };
        }

        /// <summary>
        /// 搜索并获取带 Credits 和 Images 的完整电影信息
        /// </summary>
        public async Task<Movie?> GetMovieMetadataAsync(string title, int? year)
        {
            int safeYear = year ?? 0;
            // 先搜索
            var results = await _client.SearchMovieAsync(title, year: safeYear);
            var first   = results.Results.FirstOrDefault();
            if (first == null) return null;

            // 取到 ID 后再拉详情（含演员和海报等）
            var movie = await _client.GetMovieAsync(
                first.Id,
                MovieMethods.Credits | MovieMethods.Images
            );

            return movie;
        }
        
        /// <summary>
        /// 搜索并获取电视剧详情，含演员、图片和季列表
        /// </summary>
        public async Task<TvShow?> GetTvShowMetadataAsync(string name, int? year)
        {
            int safeYear = year ?? 0;
            // 搜索剧集
            var results = await _client.SearchTvShowAsync(name, firstAirDateYear: safeYear);
            var first   = results.Results.FirstOrDefault();
            if (first == null) return null;
            // 获取剧集详情（含 Credits、Images）
            return await _client.GetTvShowAsync(
                first.Id
            );
        }
        
        public Task<TvSeason> GetSeasonAsync(int tvShowId, int seasonNumber)
            => _client.GetTvSeasonAsync(tvShowId, seasonNumber);

        public Task<TvEpisode> GetEpisodeAsync(int tvShowId, int seasonNumber, int episodeNumber)
            =>  _client.GetTvEpisodeAsync(tvShowId, seasonNumber: seasonNumber, episodeNumber: episodeNumber);

        public Task<SearchContainer<SearchMovie>> SearchMovieAsync(string query) 
            => _client.SearchMovieAsync(query);

        public Task<Movie> GetMovieAsync(int movieId) 
            => _client.GetMovieAsync(movieId);
    }
}