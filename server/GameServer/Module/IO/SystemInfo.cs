using System.Globalization;
using System.Runtime.InteropServices;
using Module.Shared;

namespace Module.IO;

public class SystemInfo {
    // e.g. Arm, X32, Arm64, X64
    public string oSArchitecture { get; set; } = "" + RuntimeInformation.OSArchitecture;
    // On Win 10 => "Microsoft Windows 10.0.16299"
    // On macOS High Sierra 10.13.4 => "Darwin 17.5.0 Darwin Kernel Version 17.5.0 ..."
    public string oSDescription { get; set; } = RuntimeInformation.OSDescription;
    // On Win 10 => "Win32NT"
    // On macOS High Sierra 10.13.4 => "Unix"
    public string osPlatform { get; set; } = "" + Environment.OSVersion.Platform;
    // On Win 10 => "6.2.9200.0"
    // On macOS High Sierra 10.13.4 => "17.5.0.0"
    public string oSVersion { get; set; } = "" + Environment.OSVersion.Version;
    // e.g. Arm, X32, Arm64, X64
    public string processArchitecture { get; set; } = "" + RuntimeInformation.ProcessArchitecture;
    public string appId { get; set; } = "" + System.Reflection.Assembly.GetExecutingAssembly().GetName().Name;
    public string appName { get; set; } = "" + AppDomain.CurrentDomain.FriendlyName;
    public string appVersion { get; set; } = "" + System.Reflection.Assembly.GetExecutingAssembly().GetName().Version;
    public string culture { get; set; } = "" + CultureInfo.CurrentCulture;
    public string language { get; set; } = "" + CultureInfo.CurrentCulture.EnglishName;
    public long latestLaunchDate { get; set; } = DateTime.UtcNow.ToUnixTimestampUtc();
    public int utcOffset { get; set; } = (int)TimeZoneInfo.Local.GetUtcOffset(DateTime.UtcNow).TotalHours; 

}