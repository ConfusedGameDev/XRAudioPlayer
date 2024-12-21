using Sirenix.OdinInspector;
using UnityEngine;

public class XRLightController : MonoBehaviour
{

    [SerializeField] private Material _sceneMaterial;
    private const string HighLightAttenuationShaderPropertyName = "_HighLightAttenuation";
    private const string HighLightOpaquenessShaderPropertyName = "_HighlightOpacity";

    [Button]
    private void HighlightSettingsToggled(bool highlightsOn)
    {
        _sceneMaterial.SetFloat(HighLightAttenuationShaderPropertyName, highlightsOn ? 1 : 0);
        _sceneMaterial.SetFloat(HighLightOpaquenessShaderPropertyName, highlightsOn ? 1 : 0);
    }
    [Button]
    private void HighlightSettingssetVal(float highlightsVal,float opaqueness)
    {
        _sceneMaterial.SetFloat(HighLightAttenuationShaderPropertyName, highlightsVal);
        _sceneMaterial.SetFloat(HighLightOpaquenessShaderPropertyName, opaqueness);
    }
    void Start()
    {
        HighlightSettingsToggled(true);
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
