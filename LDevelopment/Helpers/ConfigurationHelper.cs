using System.Configuration;

namespace LDevelopment.Helpers
{
    public static class ConfigurationHelper
    {
        public static string PageSize = ConfigurationManager.AppSettings["PageSize"];
        public static string ReadMore = ConfigurationManager.AppSettings["ReadMore"];

        public static string FacebookUrl = ConfigurationManager.AppSettings["FacebookUrl"];
        public static string LinkedInUrl = ConfigurationManager.AppSettings["LinkedInUrl"];
        public static string GithubUrl = ConfigurationManager.AppSettings["GithubUrl"];
    }
}