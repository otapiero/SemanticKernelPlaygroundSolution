using Microsoft.SemanticKernel;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using LibGit2Sharp;

namespace SemanticKernelPlayground.Plugins
{
    public class GitPlugin
    {

        private string _repositoryPath = string.Empty;


        [KernelFunction]
        [Description("Gets the latest commits from a git repository")]
        public string GetLatestCommits(
            [Description("Number of commits to retrieve")] int commitCount = 5)
        {
            if (string.IsNullOrEmpty(_repositoryPath))
            {
                return "Repository path not set. Please run **SetRepositoryPath** function first.";
            }

            try
            {
                using var repo = new Repository(_repositoryPath);
                var commits = new List<string>();

                int count = 0;
                foreach (var commit in repo.Commits)
                {
                    if (count >= commitCount)
                        break;

                    commits.Add($"- **{commit.MessageShort}**\n  - SHA: `{commit.Sha[..7]}`\n  - Author: {commit.Author.Name}\n  - Date: {commit.Author.When:yyyy-MM-dd HH:mm:ss}\n");

                    count++;
                }

                return string.Join(Environment.NewLine, commits);
            }
            catch (Exception ex)
            {
                return $"Error retrieving commits: {ex.Message}";
            }
        }

        [KernelFunction]
        [Description("Sets the path to the git repository")]
        public string SetRepositoryPath(
            [Description("Path to the git repository")] string path)
        {
            if (!Directory.Exists(path))
            {
                return $"Directory does not exist: {path}";
            }

            if (!Repository.IsValid(path))
            {
                return $"Not a valid git repository: {path}";
            }

            _repositoryPath = path;
            return $"Repository path set to: {path}";
        }
    }
}
