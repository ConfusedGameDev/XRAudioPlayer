using UnityEngine;
using Sirenix.OdinInspector;
using UnityEngine.Playables;
using System.Collections.Generic;
using System.Collections;
public class AlbumContoller : MonoBehaviour
{
    public PlayableDirector rightMovement, leftMovement, downMovement,upMovement;

    public List<AlbumMaterialController> materialControllers;

    public AnimationController animationController;

    public bool isPlaying=false;
    public AudioSource audioSource;
    [Button]
    public void goToNext()
    {
        if (isPlaying || downMovement.time > 0 || upMovement.time > 0f || rightMovement.time > 0 || leftMovement.time > 0f) return;
        for (int i = 0; i < materialControllers.Count; i++)
        {
            if (i == materialControllers.Count - 1)
            {
                materialControllers[materialControllers.Count - 1].offset = materialControllers[0].offset;
            }
            else
            {
                materialControllers[i].offset = materialControllers[i+1].offset;

            }
            materialControllers[i].updateOffset();

        }
        rightMovement.Play();
        
        
    }
    [Button]
    public void goToPrev()
    {
        if (isPlaying || downMovement.time > 0 || upMovement.time > 0f || rightMovement.time > 0 || leftMovement.time > 0f) return;
        for (int i = materialControllers.Count-1; i >= 0; i--)
        {
            if (i == 0)
            {
                materialControllers[0].offset = materialControllers[materialControllers.Count - 1].offset;
            }
            
            else
            {
                materialControllers[i].offset = materialControllers[i -1 ].offset;

            }
            materialControllers[i].updateOffset();

        }
        leftMovement.Play();


    }
    [Button]
    public void goUp()
    {
        if (!isPlaying || downMovement.time > 0 || upMovement.time > 0f || rightMovement.time > 0 || leftMovement.time > 0f) return;
        upMovement.Play();
        isPlaying = false;
        animationController.closeSongMenu();
        audioSource.Stop();
    }
    [Button]
    public void goDown()
    {
        if (isPlaying || downMovement.time > 0 || upMovement.time > 0f || rightMovement.time > 0 || leftMovement.time > 0f) return;

        downMovement.Play();
        isPlaying = true;
        animationController.openSongMenu();
        StartCoroutine(playAfterDelay());

    }

    public IEnumerator playAfterDelay()
    {
        yield return new WaitForSeconds(2f);
        audioSource.Play();
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
