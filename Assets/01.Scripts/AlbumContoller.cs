using UnityEngine;
using Sirenix.OdinInspector;
using UnityEngine.Playables;
using System.Collections.Generic;
public class AlbumContoller : MonoBehaviour
{
    public PlayableDirector rightMovement;

    public List<AlbumMaterialController> materialControllers;
    [Button]
    public void goToNext()
    {
        if (rightMovement.time > 0f) return;
        foreach (var controller in materialControllers)
        {
            controller.rightOffset();
        }
        rightMovement.Play();
        
        
    }

    private void RightMovement_stopped(PlayableDirector obj)
    {
        
    }

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        if (rightMovement != null)
            rightMovement.stopped += RightMovement_stopped;
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
