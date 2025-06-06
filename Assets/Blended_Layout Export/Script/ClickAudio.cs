using UnityEngine;
using UnityEngine.EventSystems;



public class ClickAudio : MonoBehaviour, IPointerDownHandler
{
    public AudioClip clip;


    public void OnPointerDown(PointerEventData eventData)
    {
        Utilities.Instance.PlayVoice(clip);
    }

}