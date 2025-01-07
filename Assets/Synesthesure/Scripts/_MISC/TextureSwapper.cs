// CHANGES THE MAIN TEXTURE ON A GAME OBJECT.

using UnityEngine;

namespace Synesthesure
{
    public class TextureSwapper : MonoBehaviour
    {
        [SerializeField] Texture2D[] textures;
        Renderer rend;

        void Awake()
        {
            rend = GetComponent<Renderer>();
        }

        public void SwapTexture(int index)
        {
            if (textures == null) return;
            if (textures.Length == 0 || index < 0 || index >= textures.Length) return;
            rend.material.mainTexture = textures[index];
        }
    }
}