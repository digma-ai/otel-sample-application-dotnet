namespace Sample.MoneyTransfer.API.Utils;

public class SCMUtils
{
    public static string?  GetLocalCommitHash(WebApplicationBuilder builder)
    {
        var projectRoot = builder.Environment.ContentRootPath;
        string repositoryUrl = projectRoot + "/../";
        string commitHash = null;
        //Load commit has locally if available
        if (LibGit2Sharp.Repository.IsValid(repositoryUrl))
        {
            commitHash = new LibGit2Sharp.Repository(repositoryUrl).Head.Tip.Sha;
        }
        return commitHash;
    }
}

