using Sirenix.OdinInspector;
using TMPro;
using UnityEngine;

public class XRUIController : MonoBehaviour
{
    public Transform brightnessUiLines;
    [Range(0f, 100f)]
    public float currentBrightness;
    public GameObject brightnessHolder;
    public TextMeshProUGUI brightnessLabel;
    public float minimumBrightness=50f, maximumBrightness=-50f;

    public Transform audioUILines ;
    [Range(0f, 100f)]
    public float currentVolume;
    public GameObject audioHolder;
    public TextMeshProUGUI audioLabel;
    public float minVolume= 50f, maxVolume= -50f;

    public AudioSource audioSource;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        UpdateVolume(audioSource.volume);
        deactivateUI();
        
    }

    [Button]
    public void deactivateUI()
    {
        brightnessHolder.SetActive(false);
        audioHolder.SetActive(false);
    }
    [Button]
    public void UpdateBrightness(float newBrightness)
    {
        brightnessHolder.SetActive(true);
        audioHolder.SetActive(false);
        currentBrightness =newBrightness;
        Vector3 minPos= brightnessUiLines.transform.localPosition;
        minPos.x = minimumBrightness;
        Vector3 maxPos= brightnessUiLines.transform.localPosition;
        maxPos.x= maximumBrightness;
        brightnessUiLines.localPosition = Vector3.Lerp(minPos, maxPos, currentBrightness / 100f);

        

        brightnessLabel.text = $"{Mathf.RoundToInt( currentBrightness)}%";


    }

    [Button]
    public void UpdateVolume(float newVolume)
    {
        brightnessHolder.SetActive(false);
        audioHolder.SetActive(true);


        currentVolume = newVolume;
        Vector3 minPos = audioUILines.transform.localPosition;
        minPos.y = minVolume;
        Vector3 maxPos = audioUILines.transform.localPosition;
        maxPos.y = maxVolume;
        audioUILines.localPosition = Vector3.Lerp(minPos, maxPos, currentVolume / 100f);

        

        audioLabel.text = $"{Mathf.RoundToInt( currentVolume)}%";


    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
