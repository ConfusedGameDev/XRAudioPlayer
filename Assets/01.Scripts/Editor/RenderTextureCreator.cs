using UnityEditor;
using UnityEngine;

public class RenderTextureCreator : EditorWindow
{
    private string textureName = "NewRenderTexture";
    private int width = 256;
    private int height = 256;
    private int depth = 24;
    private RenderTextureFormat format = RenderTextureFormat.Default;
    private RenderTextureReadWrite readWrite = RenderTextureReadWrite.Default;

    [MenuItem("Limbik Tools/RenderTexture Creator")]
    public static void ShowWindow()
    {
        GetWindow<RenderTextureCreator>("RenderTexture Creator");
    }

    private void OnGUI()
    {
        GUILayout.Label("RenderTexture Settings", EditorStyles.boldLabel);

        textureName = EditorGUILayout.TextField("Name", textureName);
        width = EditorGUILayout.IntField("Width", width);
        height = EditorGUILayout.IntField("Height", height);
        depth = EditorGUILayout.IntField("Depth", depth);
        format = (RenderTextureFormat)EditorGUILayout.EnumPopup("Format", format);
        readWrite = (RenderTextureReadWrite)EditorGUILayout.EnumPopup("Read/Write", readWrite);

        if (GUILayout.Button("Create RenderTexture"))
        {
            CreateRenderTexture();
        }
    }

    private void CreateRenderTexture()
    {
        if (string.IsNullOrEmpty(textureName))
        {
            Debug.LogError("RenderTexture name cannot be empty.");
            return;
        }

        RenderTexture renderTexture = new RenderTexture(width, height, depth, format, readWrite);
        string path = EditorUtility.SaveFilePanelInProject("Save RenderTexture", textureName, "renderTexture", "Specify where to save the RenderTexture asset.");

        if (string.IsNullOrEmpty(path))
        {
            Debug.LogWarning("RenderTexture creation canceled.");
            return;
        }

        AssetDatabase.CreateAsset(renderTexture, path);
        AssetDatabase.SaveAssets();
        AssetDatabase.Refresh();

        Debug.Log($"RenderTexture '{textureName}' created at {path}");
    }
}
