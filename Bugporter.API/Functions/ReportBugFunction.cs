using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.Http;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using Bugporter.API.Features.ReportBug.GitHub;
using Bugporter.API.Features.ReportBug;

namespace Bugporter.API.Functions;

public class ReportBugFunction
{
    private readonly ILogger<ReportBugFunction> _logger;
    private readonly CreateGitHubIssueCommand _createGitHubIssueCommand;

    public ReportBugFunction(ILogger<ReportBugFunction> logger, CreateGitHubIssueCommand createGitHubIssueCommand)
    {
        _logger = logger;
        _createGitHubIssueCommand = createGitHubIssueCommand;
    }

    [FunctionName("ReportBugFunction")]
    public async Task<IActionResult> Run(
        [HttpTrigger(AuthorizationLevel.Anonymous, "post", Route = "bugs")] ReportBugRequest request)
    {

        var newBug = new NewBug(request.Summary, request.Description);

        var reportedBug = await _createGitHubIssueCommand.Execute(newBug);

        return new OkObjectResult(new ReportBugResponse()
        {
            Id = reportedBug.Id,
            Summary = reportedBug.Summary,
            Description = reportedBug.Description,
        });
    }
}
