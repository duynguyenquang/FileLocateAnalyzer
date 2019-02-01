using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Threading.Tasks;
using System.Web;
using FileLocateAnalyzer.Models;
using Microsoft.Extensions.Options;
using Newtonsoft.Json;

namespace FileLocateAnalyzer.IService
{
    public interface IGitService
    {
        Task<ICollection<Branch>> GetBranchs(string projectName);
        Task<Branch> GetBranchInfor(int projectId, string branch);
        Task<IEnumerable<Project>> GetProjectInfo(string projectName);
        Task<ICollection<string>> Find(string repo, string branch, string pattern);
        Task<GitFile> ReadFilePath(int projectId, string path, string branch);
        Task<ICollection<RepositoryTree>> GetRepoTree(int projectId, string branch, bool recursive);
        Task<ICollection<RepositoryTree>> FindByPath(int projectId, string branch, bool recursive, string path);
        Task<ICollection<string>> Find(string proj, string pattern);
        Task<string> Read(string proj, string fileName);
    }

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

        public Task<IEnumerable<Project>> GetProjectInfo(string projectName)
        {
            return GetAsync<IEnumerable<Project>>($"/api/v4/projects?search={projectName}");
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

        public Task<ICollection<string>> Find(string repo, string branch, string pattern)
        {
            throw new NotImplementedException();
        }

        public async Task<ICollection<Branch>> GetBranchs(int projectId)
        {
            //var projects = await GetProjectInfo(projectName);
            //if (projects != null && projects.Any())
            //    foreach (var project in projects)
            //        if (project.name.Equals(projectName))
            //            return await GetAsync<List<Branch>>($"api/v4/projects/{project.id}/repository/branches");
            return null;
        }

        public async Task<Branch> GetBranchInfor(int projectId, string branch)
        {
            if (projectId <= 0 || string.IsNullOrEmpty(branch)) return null;
            return await GetAsync<Branch>($"api/v4/projects/{projectId}/repository/branches/{branch}");
        }

        public async Task<GitFile> ReadFilePath(int projectId, string path, string branch)
        {
            if (projectId <= 0 || string.IsNullOrEmpty(branch)) return null;
            // required  git lab end code path
            return await GetAsync<GitFile>(
                $"api/v4/projects/{projectId}/repository/files/{HttpUtility.UrlEncode(path)}?ref={branch}");
        }

        public async Task<ICollection<RepositoryTree>> GetRepoTree(int projectId, string branch, bool recursive)
        {
            return await GetAsync<ICollection<RepositoryTree>>(
                $"api/v4/projects/{projectId}/repository/tree?ref={branch}&recursive={recursive}");
        }

        public async Task<ICollection<RepositoryTree>> FindByPath(int projectId, string branch, bool recursive, string path)
        {
            return await GetAsync<ICollection<RepositoryTree>>(
                $"api/v4/projects/{projectId}/repository/tree?ref={branch}&path={path}&recursive={recursive}");
        }

        public Task<ICollection<Branch>> GetBranchs(string projectName)
        {
            throw new NotImplementedException();
        }

        public Task<ICollection<string>> Find(string proj, string pattern)
        {
            throw new NotImplementedException();
        }

        public Task<string> Read(string proj, string fileName)
        {
            throw new NotImplementedException();
        }
    }
}
