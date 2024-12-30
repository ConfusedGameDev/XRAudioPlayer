using UnityEngine;
using Sirenix.OdinInspector;
public class AlbumMaterialController : MonoBehaviour
{
    [SerializeField] private Vector2 offset; // The offset you want to apply
    private Renderer meshRenderer;          // Renderer for the mesh
    private MaterialPropertyBlock propertyBlock;
    [Button]
    void OnValidate()
    {
        // Get the Renderer component of the GameObject
        meshRenderer = GetComponent<Renderer>();
        if (meshRenderer == null)
        {
            Debug.LogError("Renderer not found on the GameObject!");
            return;
        }

        // Initialize the MaterialPropertyBlock
        propertyBlock = new MaterialPropertyBlock();

        // Set the _Offset property
        propertyBlock.SetVector("_Offset", offset);

        // Apply the property block to the renderer
        meshRenderer.SetPropertyBlock(propertyBlock);
    }

    [Button]
    // Optional: Update the offset dynamically
    public void UpdateOffset(Vector2 newOffset)
    {
        if (propertyBlock == null || meshRenderer == null) return;

        offset = newOffset;
        propertyBlock.SetVector("_Offset", offset);
        meshRenderer.SetPropertyBlock(propertyBlock);
    }
    public void rightOffset()
    {
        if (propertyBlock == null || meshRenderer == null) return;
        offset.x += 0.25f;
        if (offset.x >= 1f)
        {
            offset.x = 0f;
            offset.y += 0.25f;
        }
        propertyBlock.SetVector("_Offset", offset);
        meshRenderer.SetPropertyBlock(propertyBlock);
    }
    public void updateOffset()
    {
        if (propertyBlock == null || meshRenderer == null) return;
        propertyBlock.SetVector("_Offset", offset);
        meshRenderer.SetPropertyBlock(propertyBlock);
    }
}
