// Called by setup.sh via Unity batch mode to generate .csproj / .sln files
// for OmniSharp IntelliSense. Not included in any build (Assets/Editor/ is
// stripped by Unity's build pipeline automatically).
using Unity.CodeEditor;
using UnityEditor;

public static class DevContainerSetup
{
    static void GenerateProjectFiles()
    {
        CodeEditor.Editor.CurrentCodeEditor.SyncAll();
        EditorApplication.Exit(0);
    }
}
