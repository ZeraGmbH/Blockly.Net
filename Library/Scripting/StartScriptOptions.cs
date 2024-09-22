using BlocklyNet.Scripting.Engine;

namespace BlocklyNet.Scripting;

/// <summary>
/// 
/// </summary>
public class StartScriptOptions
{
    /// <summary>
    /// 
    /// </summary>
    public Func<bool>? ShouldStopNow { get; set; }

    /// <summary>
    /// 
    /// </summary>
    public ScriptGroupRepeat? GroupResults { get; set; }
}
