 
using System.Linq;
using NLog;
using NLog.Targets;

[Target("UnityConsole")]
public class UnityConsoleTarget : TargetWithLayout
{
    private static string _defaultLayout = "[${level:uppercase=true}] [${callsite}] ${message}${newline}${exception:format=Message, ToString:separator=\n}";

    public UnityConsoleTarget()
    {
        Layout = _defaultLayout;
    }

    public UnityConsoleTarget(string name) : this(name, _defaultLayout)
    {
    }

    public UnityConsoleTarget(string name, string layout)
    {
        Name = name;
        Layout = layout;
    }

    protected override void Write(LogEventInfo logEvent)
    {
        // get game object as context
        UnityEngine.Object context = null;
        if (logEvent.Parameters != null)
        {
            var p = logEvent.Parameters.Where(c => c is UnityEngine.Object).FirstOrDefault();
            if (p != null)
            {
                context = (UnityEngine.Object)p;
            }
        }

        // get message
        var message = this.Layout.Render(logEvent);

        // output by proper level
        if (logEvent.Level >= LogLevel.Error)
        {
            UnityEngine.Debug.LogError(message, context);
        }
        else if (logEvent.Level == LogLevel.Warn)
        {
            UnityEngine.Debug.LogWarning(message, context);
        }
        else
        {
            UnityEngine.Debug.Log(message, context);
        }
    }
}
 
