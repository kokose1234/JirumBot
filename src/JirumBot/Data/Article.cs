namespace JirumBot.Data
{
    public record struct Article
    {
        public string Title { get; init; }
        public string Url { get; init; }
    }
}