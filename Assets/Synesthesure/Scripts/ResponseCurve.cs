using UnityEngine;
namespace Synesthesure
{
    [CreateAssetMenu(fileName = "Response Curve", menuName = "Synesthesure/Response Curve")]
    public class ResponseCurve : ScriptableObject
    {
        [System.Serializable]
        public struct ResponseCurveInfo
        {
            public string description;
            public AnimationCurve curve;
        }
        public ResponseCurveInfo responseCurve;
    }
}