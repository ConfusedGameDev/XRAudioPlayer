using UnityEngine;

namespace Synesthesure { 
    public class ScaleModulation : MonoBehaviour
    {
        AudioVisual AV;
        AudioSource audioSource;

        [Space]
        [SerializeField] GameObject theObject;

        [Space]
        public float minWidth = .1f;
        public float maxWidth = 1f;
        public AudioVisual.DynamicTypes widthControlSource;
        [SerializeField] ResponseCurve _widthResponseCurve;
        public AnimationCurve widthResponseCurve;

        [Space]
        public float minHeight = .1f;
        public float maxHeight = 1f;
        public AudioVisual.DynamicTypes heightControlSource;
        [SerializeField] ResponseCurve _heightResponseCurve;
        public AnimationCurve heightResponseCurve;

        [Space]
        public float minDepth = .1f;
        public float maxDepth = 1f;
        public AudioVisual.DynamicTypes depthControlSource;
        [SerializeField] ResponseCurve _depthResponseCurve;
        public AnimationCurve depthResponseCurve;

        Vector3 vec3;

        void Awake()
        {
            if (_widthResponseCurve != null) widthResponseCurve = _widthResponseCurve.responseCurve.curve;
            else if (widthResponseCurve == null || widthResponseCurve.keys.Length < 2)
            {
                AnimationCurve curve = new AnimationCurve();
                curve.AddKey(0f, 0f); curve.AddKey(1f, 1f);
                widthResponseCurve = curve;
            }
            if (_heightResponseCurve != null) heightResponseCurve = _heightResponseCurve.responseCurve.curve;
            else if (heightResponseCurve == null || heightResponseCurve.keys.Length < 2)
            {
                AnimationCurve curve = new AnimationCurve();
                curve.AddKey(0f, 0f); curve.AddKey(1f, 1f);
                heightResponseCurve = curve;
            }
            if (_depthResponseCurve != null) depthResponseCurve = _depthResponseCurve.responseCurve.curve;
            else if (depthResponseCurve == null || depthResponseCurve.keys.Length < 2)
            {
                AnimationCurve curve = new AnimationCurve();
                curve.AddKey(0f, 0f); curve.AddKey(1f, 1f);
                depthResponseCurve = curve;
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

            if (widthControlSource != AudioVisual.DynamicTypes.None ||
                heightControlSource != AudioVisual.DynamicTypes.None ||
                depthControlSource != AudioVisual.DynamicTypes.None)
            {
                vec3 = new Vector3(Mathf.Lerp(minWidth, maxWidth, widthResponseCurve.Evaluate(AV.ReactionValue(widthControlSource))),
                Mathf.Lerp(minHeight, maxHeight, heightResponseCurve.Evaluate(AV.ReactionValue(heightControlSource))),
                Mathf.Lerp(minDepth, maxDepth, depthResponseCurve.Evaluate(AV.ReactionValue(depthControlSource))));
                theObject.transform.localScale = vec3;
            }
        }
    }
}
