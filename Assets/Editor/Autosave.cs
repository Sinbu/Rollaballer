using UnityEngine;
using UnityEditor;

[InitializeOnLoad]
public class AutosaveOnRun {
    static AutosaveOnRun() {
        EditorApplication.playmodeStateChanged = () => {
            if (EditorApplication.isPlayingOrWillChangePlaymode && !EditorApplication.isPlaying) {
                var activeScene = UnityEditor.SceneManagement.EditorSceneManager.GetActiveScene();
                Debug.Log("Auto-Saving scene before entering Play mode: " + activeScene);

                UnityEditor.SceneManagement.EditorSceneManager.SaveScene(activeScene);
                AssetDatabase.SaveAssets();

            }
        };
    }
}
