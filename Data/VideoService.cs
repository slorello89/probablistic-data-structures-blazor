using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Extensions.Configuration;
using Newtonsoft.Json.Linq;
using StackExchange.Redis;

namespace testRedisBloom.Data
{
    public class VideoService
    {
        private readonly HttpClient _client;
        private readonly IDatabase _db;
        private readonly IConfiguration _config;
        
        public VideoService(HttpClient client, IConnectionMultiplexer multiplexer, IConfiguration config)
        {
            _client = client;
            _db = multiplexer.GetDatabase();
            _config = config;
        }

        public async Task<IEnumerable<VideoData>> GetVideoData(int offset = 0)
        {
            var videoIds = _db.ListRange("videos", offset, 5);
            var missedIds = new List<string>();
            var res = new List<VideoData>();
            foreach (string id in videoIds)
            {
                if (await _db.KeyExistsAsync(id))
                {
                    var hash = await _db.HashGetAllAsync(id);
                    res.Add(new VideoData()
                    {
                        Id = id,
                        Thumbnail = hash.First(x=>x.Name == "Thumbnail").Value,
                        Title = hash.First(x=>x.Name == "Title").Value,
                        UniqueViews =  (int)await _db.HyperLogLogLengthAsync($"hll:{id}"),
                        TotalViews = (int)await _db.ScriptEvaluateAsync(Scripts.QueryCms, new {KeyName="total-view-sketch", Item=id})
                    });
                }
                else
                {
                    missedIds.Add(id);
                }
            }

            if (missedIds.Any())
            {
                var requestUri = new StringBuilder($"https://youtube.googleapis.com/youtube/v3/videos?part=snippet&key={_config["YoutubeApiKey"]}");
                foreach (var id in missedIds)
                {
                    requestUri.Append($"&id={id}");
                }
                var youtubeResult = JObject.Parse(await _client.GetStringAsync(requestUri.ToString()));
                var arr = (JArray)youtubeResult["items"];
                foreach (var o in arr)
                {
                    var obj = (JObject) o;
                    var id = obj["id"].ToString();
                    var title = obj["snippet"]["title"].ToString();
                    var thumbnail = obj["snippet"]["thumbnails"]["medium"]["url"].ToString();

                    res.Add(new()
                    {
                        Id = id,
                        Title = title,
                        Thumbnail = thumbnail,
                        TotalViews = 0,
                        UniqueViews = 0
                    });
                    _db.HashSet(id, new[]
                    {
                        new HashEntry("Id", id),
                        new HashEntry("Title", title),
                        new HashEntry("Thumbnail", thumbnail)
                    });
                }
            }

            return res;
        }

        public async Task<VideoData> GetData(string id)
        {
            var hash = await _db.HashGetAllAsync(id);
            return new()
            {
                Id = id,
                Thumbnail = hash.First(x => x.Name == "Thumbnail").Value,
                Title = hash.First(x => x.Name == "Title").Value,
                UniqueViews = (int) await _db.HyperLogLogLengthAsync($"hll:{id}"),
                TotalViews = (int) await _db.ScriptEvaluateAsync(Scripts.QueryCms,
                    new {KeyName = "total-view-sketch", Item = id})
            };
        }
    }
}