namespace Develix.AzureDevOps.Connector.Model;

public record PullRequest
{
    public int Id { get; init; } = -1;

    public PullRequestStatus Status { get; init; }

    private string author = "Author unknown";
    public string Author
    {
        get { return author; }
        init { author = value ?? throw new ArgumentNullException(nameof(Author)); }
    }

    private string title = "Name unknown";
    public string Title
    {
        get { return title; }
        init { title = value ?? throw new ArgumentNullException(nameof(Title)); }
    }
}
