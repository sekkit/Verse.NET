namespace Module.Shared
{
    public interface ISystemInfo {
        string oSArchitecture { get; }
        string oSDescription { get; }
        string osPlatform { get; }
        string oSVersion { get; }
        string processArchitecture { get; }
        string appId { get; }
        string appName { get; }
        string appVersion { get; }
        string culture { get; }
        string language { get; } 
        long latestLaunchDate { get; }
        int utcOffset { get; }
    }
}