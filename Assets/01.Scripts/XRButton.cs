using Oculus.Interaction;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Events;

public class XRButton : MonoBehaviour
{
    public PokeInteractable buttonInteractable;
    public UnityEvent onHover,OnSelect;
    void Start()
    {
        buttonInteractable.WhenStateChanged += onStateChanged;
        
    }

    private void onStateChanged(InteractableStateChangeArgs obj)
    {
        Debug.Log(obj.NewState);
        switch (obj.NewState)
        {
            case InteractableState.Normal:
                break;
            case InteractableState.Hover:
                onHover?.Invoke();
                break;
            case InteractableState.Select:
                if (obj.PreviousState != InteractableState.Select)
                    OnSelect?.Invoke();
                break;
            case InteractableState.Disabled:
                break;
        }

    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
