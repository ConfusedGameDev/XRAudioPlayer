using UnityEngine;
using Sirenix.OdinInspector;
using UnityEngine.Playables;
using System.Collections.Generic;
using System.Collections;
public class AlbumContoller : SerializedMonoBehaviour
{
    public PlayableDirector rightMovement, leftMovement, downMovement,upMovement;

    public List<AlbumMaterialController> materialControllers;

    public AnimationController animationController;

    public bool isPlaying=false;
    public AudioSource audioSource;

    public int currentAlbum;
    public Dictionary<int, AudioClip> clips;
    public Dictionary<int, GameObject> lightContoller;

    [Button]
    public void goToNext()
    {
        currentAlbum++;
        if (currentAlbum >6)
            currentAlbum = -6;

        if (isPlaying || downMovement.time > 0 || upMovement.time > 0f || rightMovement.time > 0 || leftMovement.time > 0f) return;
       
        if (lastDirector != null && lastDirector != rightMovement)
        {
            StartCoroutine(playDirectorBackwards(leftMovement, 0.5f));
            updateTexturesNext = false;
            updateTexturesPrev = true;
        }
        else
        {
            if (updateTexturesNext)
            {
                for (int i = 0; i < materialControllers.Count; i++)
                {
                    if (i == materialControllers.Count - 1)
                    {
                        materialControllers[materialControllers.Count - 1].offset = materialControllers[0].offset;
                    }
                    else
                    {
                        materialControllers[i].offset = materialControllers[i + 1].offset;

                    }
                    materialControllers[i].updateOffset();

                }
            }
            else
            {
                updateTexturesNext = true;
            }
            rightMovement.Play();
            lastDirector = rightMovement;
        }
         
        
    }
    public PlayableDirector lastDirector;
    bool updateTexturesNext = true;
    bool updateTexturesPrev = true;


    public IEnumerator playDirectorBackwards(PlayableDirector director, float duration)
    {
        var delta = 0f;
        director.timeUpdateMode = DirectorUpdateMode.Manual;
        while(delta < duration)
        {
            yield return null;
            director.time = Mathf.Lerp(0.5f, 0f, delta / duration);
            director.Evaluate();
            delta += Time.deltaTime;

        }
        director.timeUpdateMode = DirectorUpdateMode.GameTime;
        director.Stop();
        lastDirector = null;
    }
    [Button]
    public void goToPrev()
    {
        currentAlbum--;
        if (currentAlbum < -6)
            currentAlbum = 6;
        if (isPlaying || downMovement.time > 0 || upMovement.time > 0f || rightMovement.time > 0 || leftMovement.time > 0f) return;
       
        if (lastDirector != null && lastDirector!= leftMovement)
        {
            StartCoroutine(playDirectorBackwards(rightMovement, 0.5f));
            updateTexturesPrev = false;
            updateTexturesNext = true;
        }
        else
        {
            if (updateTexturesPrev)
            {

                for (int i = materialControllers.Count - 1; i >= 0; i--)
                {
                    if (i == 0)
                    {
                        materialControllers[0].offset = materialControllers[materialControllers.Count - 1].offset;
                    }

                    else
                    {
                        materialControllers[i].offset = materialControllers[i - 1].offset;

                    }
                    materialControllers[i].updateOffset();

                }
            }
            else
            {
                updateTexturesPrev=true;
            }
            leftMovement.Play();
            lastDirector = leftMovement;
        }
        


    }
    public bool canGoUp { get; set; }
    [Button]
    public void goUp()
    {
        if (lightContoller.ContainsKey(currentAlbum))
        {
            lightContoller[currentAlbum].SetActive(false);
        }
        Debug.Log("TRYING TO GO UP " + canGoUp);
        if (!canGoUp) return;
        if(downMovement.state== PlayState.Playing)
            downMovement.Stop();
        upMovement.Play();
        isPlaying = false;
        animationController.closeSongMenu();
        audioSource.Stop();
        canGoUp=false;
    }
    [Button]
    public void goDown()
    {
        if (isPlaying || downMovement.time > 0 || upMovement.time > 0f || rightMovement.time > 0 || leftMovement.time > 0f) return;

       

        downMovement.Play();
        isPlaying = true;
        animationController.openSongMenu();
        StartCoroutine(playAfterDelay());
        StartCoroutine(canGoUpEnable());    

    }
    public IEnumerator canGoUpEnable()
    {
        yield return new WaitForSeconds(3.316667f);
        canGoUp = true;
    }

    
    public IEnumerator playAfterDelay()
    {
        yield return new WaitForSeconds(3.316667f);
        if (clips.ContainsKey(currentAlbum))
        {
            audioSource.clip = clips[currentAlbum];
        }
        if (lightContoller.ContainsKey(currentAlbum))
        {
            lightContoller[currentAlbum].SetActive(true);
        }
        audioSource.Play();
    }

    private void RightMovement_stopped(PlayableDirector obj)
    {
        
    }

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        canGoUp = false;
        if (rightMovement != null)
            rightMovement.stopped += RightMovement_stopped;
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
