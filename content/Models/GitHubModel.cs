using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Newtonsoft.Json;

namespace FileLocateAnalyzer.Models
{
    public class GitCommitRequest
    {
        public string object_kind { get; set; }
        public string before { get; set; }
        public string after { get; set; }

        [JsonProperty("ref")]
        public string Ref { get; set; }
        public string checkout_sha { get; set; }
        public int user_id { get; set; }
        public string user_name { get; set; }
        public string user_username { get; set; }
        public string user_email { get; set; }
        public string user_avatar { get; set; }
        public int project_id { get; set; }
        public Project project { get; set; }
        public Repository repository { get; set; }
        public Commit[] commits { get; set; }
        public int total_commits_count { get; set; }
    }

    public class Project
    {
        public int id { get; set; }
        public string name { get; set; }
        public string description { get; set; }
        public string web_url { get; set; }
        public object avatar_url { get; set; }
        public string git_ssh_url { get; set; }
        public string git_http_url { get; set; }
        public string _namespace { get; set; }
        public int visibility_level { get; set; }
        public string path_with_namespace { get; set; }
        public string default_branch { get; set; }
        public string homepage { get; set; }
        public string url { get; set; }
        public string ssh_url { get; set; }
        public string http_url { get; set; }
    }

    public class Repository
    {
        public string name { get; set; }
        public string url { get; set; }
        public string description { get; set; }
        public string homepage { get; set; }
        public string git_http_url { get; set; }
        public string git_ssh_url { get; set; }
        public int visibility_level { get; set; }
    }

    public class Commit
    {
        public string id { get; set; }
        public string message { get; set; }
        public DateTime timestamp { get; set; }
        public string url { get; set; }
        public Author author { get; set; }
        public string[] added { get; set; }
        public string[] modified { get; set; }
        public object[] removed { get; set; }
    }

    public class Author
    {
        public string name { get; set; }
        public string email { get; set; }
    }


    public class Branch
    {
        public string name { get; set; }
        public Commit commit { get; set; }
        public bool merged { get; set; }
        public bool _protected { get; set; }
        public bool developers_can_push { get; set; }
        public bool developers_can_merge { get; set; }
    }

    public class GitFile
    {
        public string file_name { get; set; }
        public string file_path { get; set; }
        public string size { get; set; }
        public string encoding { get; set; }
        public string content { get; set; }

        [JsonProperty("ref")]
        public string Ref { get; set; }
        public string blob_id { get; set; }
        public string commit_id { get; set; }
        public string last_commit_id { get; set; }

    }
    public class RepositoryTree
    {
        public string id { get; set; }
        public string name { get; set; }
        public string type { get; set; }
        public string path { get; set; }
        public string mode { get; set; }
    }

}
