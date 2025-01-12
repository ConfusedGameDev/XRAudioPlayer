using UnityEngine;
using Sirenix.OdinInspector;
public class AlbumMaterialController : MonoBehaviour
{
    public Vector2 offset; // The offset you want to apply
    private Renderer meshRenderer;          // Renderer for the mesh
     [Button]
    void Awake()
    {
        
        // Get the Renderer component of the GameObject
        meshRenderer = GetComponent<Renderer>();
        if (meshRenderer == null)
        {
            Debug.LogError("Renderer not found on the GameObject!");
            return;
        }

        // Initialize the MaterialPropertyBlock
        

        // Apply the property block to the renderer
        meshRenderer.material.SetVector("_Offset", offset);
    }


    [Button]
    // Optional: Update the offset dynamically
    public void UpdateOffset(Vector2 newOffset)
    {
        if (meshRenderer == null) return;

        offset = newOffset;
         meshRenderer.material.SetVector("_Offset", offset);
    }
    public void rightOffset()
    {
        if (meshRenderer == null) return;
        offset.x += 0.25f;
        if (offset.x >= 1f)
        {
            offset.x = 0f;
            offset.y -= 0.25f;
        }
        meshRenderer.material.SetVector("_Offset", offset);
    }

    public void UpdateOffsets()
    {
        meshRenderer.material.SetVector("_Offset", offset);
    }
    public void leftOffset()
    {
        if (meshRenderer == null) return;
        offset.x -= 0.25f;
        if (offset.x <= -1f)
        {
            offset.x = 0f;
            offset.y += 0.25f;
        }
        meshRenderer.material.SetVector("_Offset", offset);
    }
    public void updateOffset()
    {
        if ( meshRenderer == null) return;
        meshRenderer.material.SetVector("_Offset", offset);
    }
}
