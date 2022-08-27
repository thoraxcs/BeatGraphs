namespace BeatGraphs
{
    // Loop resolution methods
    public enum Method
    {
        Standard,
        Iterative,
        Weighted
    }

    // Log levels for text output
    public enum LogLevel
    {
        verbose,
        special,
        info,
        warning,
        error
    }

    // File path to use when working with directories
    public enum BasePath
    {
        settings,
        file
    }
}
