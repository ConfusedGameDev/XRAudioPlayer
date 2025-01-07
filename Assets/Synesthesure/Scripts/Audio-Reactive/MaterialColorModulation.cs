using UnityEngine;

namespace Synesthesure
{
    public class MaterialColorModulation : MonoBehaviour
    {
        AudioVisual AV;
        AudioSource audioSource;
        public AudioVisual.DynamicTypes controlSource;
        [SerializeField] ResponseCurve _responseCurve;
        public AnimationCurve responseCurve;

        [Space]
        [SerializeField] Material material;
        [SerializeField] GradientLibrary gradients;
        public int gradientToUse = -1;
        [SerializeField] Gradient gradient;
        int previousGradient;
        [HideInInspector] Color color;

        void Awake()
        {
            if (gradientToUse > -1 && gradients != null) SetGradient(gradientToUse);
            if (_responseCurve != null) responseCurve = _responseCurve.responseCurve.curve;
            else if (responseCurve != null || responseCurve.keys.Length < 2)
            {
                AnimationCurve curve = new AnimationCurve();
                curve.AddKey(0f, 0f); curve.AddKey(1f, 1f);
                responseCurve = curve;
            }
        }

        private void Start()
        {
            AV = AudioVisual.AV;
            if (AV.audioInput != null) audioSource = AV.audioInput;
            else if (audioSource == null) audioSource = AV.GetComponent<AudioSource>();
        }


        void Update()
        {
            if (!audioSource.isPlaying) return;

            if (controlSource != AudioVisual.DynamicTypes.None)
            {
                color = gradient.Evaluate(responseCurve.Evaluate(AV.ReactionValue(controlSource)));
                material.color = color;
            }
        }

        // ------------------------------------------------------

        void OnValidate()
        {
            if (gradientToUse != previousGradient)
            {
                previousGradient = gradientToUse;
                SetGradient(gradientToUse);
            }
        }

        public void SetGradient(int index)
        {
            if (gradients == null) return;
            if (index > -1 && index < gradients.gradients.Length) gradient = gradients.gradients[index];
        }

    }
}
