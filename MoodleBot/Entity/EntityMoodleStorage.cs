using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Net;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Bot.Builder;
using Microsoft.EntityFrameworkCore;
using MoodleBot.Models;
using System.Text.Json;
using Newtonsoft.Json;
using Microsoft.Extensions.Configuration;
using MoodleBot.Common;
using JsonSerializer = System.Text.Json.JsonSerializer;

namespace MoodleBot.Entity
{

    public class BotEntity
    {
        public string RealId { get; set; }
        public string Document { get; set; }
    }


    /// <summary>
    /// Implements an EntityFramework based storage provider for a bot.
    /// </summary>
    public class EntityMoodleStorage : IStorage
    {
        #region Properties
        private static readonly JsonSerializerSettings _jsonSettings = new() { TypeNameHandling = TypeNameHandling.All, MaxDepth = int.MaxValue };
        private bool _checkedConnection;
        private readonly EntityFrameworkStorageOptions _storageOptions;
        private readonly IConfiguration _configuration;
        #endregion

        #region Constructor
        public EntityMoodleStorage(IConfiguration configuration)
        {
            _configuration = configuration;
        }
        #endregion

        public async Task<IDictionary<string, object>> ReadAsync(string[] keys, CancellationToken cancellationToken = default)
        {

            var storeItems = new Dictionary<string, object>(keys.Length);

            using (var client = new HttpClient(new HttpClientHandler { AutomaticDecompression = DecompressionMethods.GZip | DecompressionMethods.Deflate }))
            {
                var baseURL = _configuration.GetMoodleAPIConfig("BaseUrl");
                var wsFunction = _configuration.GetMoodleAPIConfig("GetBotDataEntity");
                var token = _configuration.GetMoodleConfig("AuthToken");

                var content = new MultipartFormDataContent();

                content.Add(new StringContent(keys[0]), String.Format("\"{0}\"","realid"));

                client.BaseAddress = new Uri(baseURL);
                HttpResponseMessage response = await client.PostAsync($"server.php?moodlewsrestformat=json&wstoken={token}&wsfunction={wsFunction}",content);
                string result = response.Content.ReadAsStringAsync().Result;
                BotEntity botEntity = JsonSerializer.Deserialize<BotEntity>(result, new JsonSerializerOptions
                {
                    PropertyNameCaseInsensitive = true
                });

                if (botEntity.RealId != "" ){
                    var jObject = JsonConvert.DeserializeObject<object>(botEntity.Document, _jsonSettings);

                    storeItems.Add(botEntity.RealId, jObject);
                }

            }
            
            return storeItems;
        }

        public async Task WriteAsync(IDictionary<string, object> changes, CancellationToken cancellationToken = default)
        {
            using (var client = new HttpClient(new HttpClientHandler { AutomaticDecompression = DecompressionMethods.GZip | DecompressionMethods.Deflate }))
            {
                var baseURL = _configuration.GetMoodleAPIConfig("BaseUrl");
                var wsFunction = _configuration.GetMoodleAPIConfig("UpdateBotDataEntity");
                var token = _configuration.GetMoodleConfig("AuthToken");

                foreach (var change in changes){

                    var jsonDocument = JsonConvert.SerializeObject(change.Value, _jsonSettings);
                    var timeNow = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss");
                    var content = new MultipartFormDataContent();

                    content.Add(new StringContent(change.Key), String.Format("\"{0}\"", "realid"));
                    content.Add(new StringContent(jsonDocument, Encoding.UTF8, "application/json"), String.Format("\"{0}\"", "document"));
                    content.Add(new StringContent(timeNow), String.Format("\"{0}\"", "createdtime"));
                    content.Add(new StringContent(timeNow), String.Format("\"{0}\"", "timestamp"));

                    client.BaseAddress = new Uri(baseURL);
                    HttpResponseMessage response = await client.PostAsync($"server.php?moodlewsrestformat=json&wstoken={token}&wsfunction={wsFunction}", content);
                    response.EnsureSuccessStatusCode();
                    string result = response.Content.ReadAsStringAsync().Result;
                    Console.WriteLine(result);
                }           
            }

            return;
        }
        
        public async Task DeleteAsync(string[] keys, CancellationToken cancellationToken = default)
        {

            using (var client = new HttpClient(new HttpClientHandler { AutomaticDecompression = DecompressionMethods.GZip | DecompressionMethods.Deflate }))
            {
                var baseURL = _configuration.GetMoodleAPIConfig("BaseUrl");
                var wsFunction = _configuration.GetMoodleAPIConfig("DeleteBotDataEntity");
                var token = _configuration.GetMoodleConfig("AuthToken");

                var content = new MultipartFormDataContent();

                content.Add(new StringContent(keys[0]), String.Format("\"{0}\"", "realid"));

                client.BaseAddress = new Uri(baseURL);
                HttpResponseMessage response = await client.PostAsync($"server.php?moodlewsrestformat=json&wstoken={token}&wsfunction={wsFunction}", content);
                string result = response.Content.ReadAsStringAsync().Result;
                BotEntity botEntity = JsonSerializer.Deserialize<BotEntity>(result, new JsonSerializerOptions
                {
                    PropertyNameCaseInsensitive = true
                });

            }

            return;
        }
    }
}
