using UnityEditor;
using UnityEngine;

public class CubemapCreator : EditorWindow
{
    private string cubemapName = "NewCubemap";
    private Texture2D frontTexture;
    private Texture2D backTexture;
    private Texture2D leftTexture;
    private Texture2D rightTexture;
    private Texture2D upTexture;
    private Texture2D downTexture;

    [MenuItem("Limbik Tools/Cubemap Creator")]
    public static void ShowWindow()
    {
        GetWindow<CubemapCreator>("Cubemap Creator");
    }

    private void OnGUI()
    {
        GUILayout.Label("Cubemap Settings", EditorStyles.boldLabel);

        cubemapName = EditorGUILayout.TextField("Name", cubemapName);

        GUILayout.Label("Assign Textures", EditorStyles.boldLabel);
        frontTexture = (Texture2D)EditorGUILayout.ObjectField("Front", frontTexture, typeof(Texture2D), false);
        backTexture = (Texture2D)EditorGUILayout.ObjectField("Back", backTexture, typeof(Texture2D), false);
        leftTexture = (Texture2D)EditorGUILayout.ObjectField("Left", leftTexture, typeof(Texture2D), false);
        rightTexture = (Texture2D)EditorGUILayout.ObjectField("Right", rightTexture, typeof(Texture2D), false);
        upTexture = (Texture2D)EditorGUILayout.ObjectField("Up", upTexture, typeof(Texture2D), false);
        downTexture = (Texture2D)EditorGUILayout.ObjectField("Down", downTexture, typeof(Texture2D), false);

        if (GUILayout.Button("Create Cubemap"))
        {
            CreateCubemap();
        }
    }

    private void CreateCubemap()
    {
        if (string.IsNullOrEmpty(cubemapName))
        {
            Debug.LogError("Cubemap name cannot be empty.");
            return;
        }

        if (!AreAllTexturesAssigned())
        {
            Debug.LogError("All six textures must be assigned to create a Cubemap.");
            return;
        }

        Cubemap cubemap = new Cubemap(frontTexture.width, TextureFormat.RGBA32, false);

        AssignTextureToCubemap(cubemap, CubemapFace.PositiveX, rightTexture);
        AssignTextureToCubemap(cubemap, CubemapFace.NegativeX, leftTexture);
        AssignTextureToCubemap(cubemap, CubemapFace.PositiveY, upTexture);
        AssignTextureToCubemap(cubemap, CubemapFace.NegativeY, downTexture);
        AssignTextureToCubemap(cubemap, CubemapFace.PositiveZ, frontTexture);
        AssignTextureToCubemap(cubemap, CubemapFace.NegativeZ, backTexture);

        string path = EditorUtility.SaveFilePanelInProject("Save Cubemap", cubemapName, "cubemap", "Specify where to save the Cubemap asset.");

        if (string.IsNullOrEmpty(path))
        {
            Debug.LogWarning("Cubemap creation canceled.");
            return;
        }

        AssetDatabase.CreateAsset(cubemap, path);
        AssetDatabase.SaveAssets();
        AssetDatabase.Refresh();

        Debug.Log($"Cubemap '{cubemapName}' created at {path}");
    }

    private bool AreAllTexturesAssigned()
    {
        return frontTexture != null && backTexture != null && leftTexture != null && rightTexture != null && upTexture != null && downTexture != null;
    }

    private void AssignTextureToCubemap(Cubemap cubemap, CubemapFace face, Texture2D texture)
    {
        Color[] pixels = texture.GetPixels();
        cubemap.SetPixels(pixels, face);
    }
}
