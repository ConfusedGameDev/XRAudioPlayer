using UnityEngine;
using UnityEngine.UI;

namespace Synesthesure
{
    public class GradientTextureGenerator : MonoBehaviour
    {
        [SerializeField] GradientLibrary colorsPalettes;
        [SerializeField] int gradient;
        [SerializeField] RawImage image;
        [SerializeField] int width = 256;
        [SerializeField] int height = 256;

        public void SetGradient(float index)
        {
            gradient = (int)index;
            CreateTexture();
        }

        void Start()
        {
            CreateTexture();
        }

        void CreateTexture()
        {
            Texture2D texture = new Texture2D(width, height);
            for (int y = 0; y < height; y++)
            {
                for (int x = 0; x < width; x++)
                {
                    float t = (float)x / (float)width;
                    Color color = colorsPalettes.gradients[gradient].Evaluate(t);
                    texture.SetPixel(x, y, color);
                }
            }
            texture.Apply();
            image.texture = texture;
        }
    }
}
