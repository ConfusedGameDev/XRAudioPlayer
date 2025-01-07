using UnityEngine;

namespace Synesthesure
{
    [CreateAssetMenu(fileName = "Gradient Library", menuName = "Synesthesure/Gradient Library")]
    public class GradientLibrary : ScriptableObject
    {
        public Gradient[] gradients;
    }
}
