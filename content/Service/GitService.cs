using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Threading.Tasks;
using System.Web;
using FileLocateAnalyzer.IService;
using FileLocateAnalyzer.Models;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.AspNetCore.ResponseCaching.Internal;
using Microsoft.Extensions.Options;
using Newtonsoft.Json;

namespace FileLocateAnalyzer.Service
{
    public class GitService : IGitService
    {
        private readonly GitConfiguration _configuration;

        public GitService(IOptions<GitConfiguration> configuration)
        {
            _configuration = configuration.Value;
            HttpClient = RegisterHttpClient();
        }

        private HttpClient HttpClient { get; }

        private HttpClient RegisterHttpClient()
        {
            var client = new HttpClient
            {
                BaseAddress = new Uri(_configuration.Url)
            };
            client.DefaultRequestHeaders.Add("Private-Token", _configuration.PrivateToken);
            client.DefaultRequestHeaders.TryAddWithoutValidation("Content-Type", "application/json");
            client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
            return client;
        }

        private async Task<TResponse> GetAsync<TResponse>(string requestUrl, HttpContent content = null)
        {
            try
            {
                if (content == null) content = new StringContent(string.Empty, Encoding.UTF8, "application/json");
                var response = await SendAsync(HttpMethod.Get, requestUrl, content);

                using (var stream = await response.Content.ReadAsStreamAsync())
                {
                    using (var streamReader = new StreamReader(stream))
                    {
                        using (var jsonReader = new JsonTextReader(streamReader))
                        {
                            return new JsonSerializer().Deserialize<TResponse>(jsonReader);
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        private async Task<HttpResponseMessage> SendAsync(HttpMethod method, string requestUrl, HttpContent content)
        {
            try
            {
                var httpClient = HttpClient;
                var request = new HttpRequestMessage
                {
                    Method = method,
                    RequestUri = new Uri(httpClient.BaseAddress, requestUrl),
                    Content = content
                };

                var response = await httpClient.SendAsync(request);
                response.EnsureSuccessStatusCode();

                return response;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public async Task<ICollection<Project>> GetProjects()
        {
            return await GetAsync<ICollection<Project>>($"/api/v4/projects");
        }

        public async Task<ICollection<Branch>> GetBranches(int projectId)
        {
            if (projectId <= 0) return null;
            return await GetAsync<ICollection<Branch>>($"api/v4/projects/{projectId}/repository/branches");
        }

        public async Task<Collection<string>> GetPaths(int projectId, string branch)
        {
            if (projectId <= 0) return null;
            var paths = new Collection<string>();
            var repositories = await GetRepositories(projectId, branch, true);
            foreach (var repo in repositories)
            {
                if (repo.name.EndsWith(".cshtml") || repo.name.Contains("resources.js") || repo.name.EndsWith(".vue")) paths.Add(repo.path);
                var childPaths = await GetChildByPath(projectId, branch, true, repo.path);
                foreach (var path in childPaths)
                {
                    if (path.name.EndsWith(".cshtml") || path.name.Contains("resources.js") || path.name.EndsWith(".vue")) paths.Add(path.path);
                }
            }
            return paths;
        }

        public async Task<string> ReadPath(int projectId, string branch, string path)
        {
            if (projectId <= 0 || string.IsNullOrEmpty(branch)) return null;
            // required  git lab end code path
            return await GetAsync<string>($"api/v4/projects/{projectId}/repository/files/{HttpUtility.UrlEncode(path)}/raw?ref={branch}");
        }

        private async Task<ICollection<RepositoryTree>> GetChildByPath(int projectId, string branch, bool recursive, string path)
        {
            if (projectId <= 0) return null;
            return await GetAsync<ICollection<RepositoryTree>>( $"api/v4/projects/{projectId}/repository/tree?ref={branch}&path={path}&recursive={recursive}");
        }

        private async Task<ICollection<RepositoryTree>> GetRepositories(int projectId, string branch, bool recursive)
        {
            return await GetAsync<ICollection<RepositoryTree>>($"api/v4/projects/{projectId}/repository/tree?ref={branch}&recursive={recursive}");
        }
    }
}
