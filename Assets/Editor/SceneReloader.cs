using UnityEditor;
using UnityEditor.SceneManagement;

public class SceneReloader
{
    [MenuItem("Tools/Reload Current Scene %#r")] // Ctrl+Shift+R (Cmd+Shift+R on Mac)
    static void ReloadScene()
    {
        var activeScene = EditorSceneManager.GetActiveScene();
        EditorSceneManager.OpenScene(activeScene.path);
    }
}
