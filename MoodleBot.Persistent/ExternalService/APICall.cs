using MoodleBot.Common;
using MoodleBot.Persistent.Entity;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Threading.Tasks;

namespace MoodleBot.Persistent.ExternalService
{
    public static class APICall
    {
        private static HttpClient GetHttpClient(string url)
        {
            var client = new HttpClient { BaseAddress = new Uri(url) };
            client.DefaultRequestHeaders.Accept.Clear();
            client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
            return client;
        }

        private static async Task<APIResponse<TResult>> GetAsync<TResult>(string url, string urlParameters)
        {
            var result = new APIResponse<TResult>
            {
                Success = false
            };

            try
            {
                using var client = GetHttpClient(url);
                var response = await client.GetAsync(urlParameters);
                if (response.StatusCode == HttpStatusCode.OK)
                {
                    var json = await response.Content.ReadAsStringAsync();
                    result = GetResponseData(result, json);
                }
                else
                {
                    result.Error = GetErrorObject($"Got an error while try to get data from Moodle API.", response.StatusCode.ToString());
                }
            }
            catch (Exception exception)
            {
                result.Error = GetErrorObject(exception.ToString());
            }

            return result;
        }

        private static async Task<APIResponse<TResult>> PostAsync<TResult, TInput>(string url, TInput input, string urlParameters)
        {
            var result = new APIResponse<TResult>
            {
                Success = false
            };

            try
            {
                var formDataContent = GetFormDataFromInputParameter(input);
                using var client = GetHttpClient(url);
                var response = await client.PostAsync(urlParameters, formDataContent);
                if (response.StatusCode == HttpStatusCode.OK)
                {
                    var json = await response.Content.ReadAsStringAsync();
                    result = GetResponseData(result, json);
                }
                else
                {
                    result.Error = GetErrorObject($"Got an error while try to get data from Moodle API.", response.StatusCode.ToString());
                }
            }
            catch (Exception exception)
            {
                result.Error = GetErrorObject(exception.ToString());
            }

            return result;
        }

        private static APIResponse<TResult> GetResponseData<TResult>(APIResponse<TResult> response, string json)
        {
            if (json.Contains("exception"))
            {
                response.Error = JsonConvert.DeserializeObject<Error>(json);
            }
            else
            {
                response.Data = JsonConvert.DeserializeObject<TResult>(json);
                response.Success = true;
            }

            return response;
        }

        private static Error GetErrorObject(string errorMessage, string errorCode = "")
        {
            return new Error {
                Exception = "Internal Server Error",
                ErrorCode = errorCode.IsNullOrEmpty() ? "Internal Server Error" : errorCode,
                Message = errorMessage
            };
        }

        private static MultipartFormDataContent GetFormDataFromInputParameter<TInput>(TInput input)
        {
            var formVariables = JsonConvert.DeserializeObject<Dictionary<string, string>>(JsonConvert.SerializeObject(input));
            var formDataContent = new MultipartFormDataContent();

            foreach (var keyValuePair in formVariables)
            {
                if (keyValuePair.Value != null)
                {
                    formDataContent.Add(new StringContent(keyValuePair.Value), keyValuePair.Key);
                }
            }

            return formDataContent;
        }

        public static async Task<APIResponse<TResult>> RunAsync<TResult, TInput>(string url, HttpMethod httpMethod, TInput input, string urlParameters = "")
        {
            return httpMethod switch
            {
                HttpMethod m when m == HttpMethod.Get => await GetAsync<TResult>(url, urlParameters),
                HttpMethod m when m == HttpMethod.Post => await PostAsync<TResult, TInput>(url, input, urlParameters),
                _ => throw new Exception("Insert valid method type")
            };
        }
    }
}
