
using System.Collections;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using TMPro;


public class Drag_ReadAndSolve : MonoBehaviour, IDragHandler, IBeginDragHandler, IEndDragHandler
{
    [HideInInspector] public RectTransform rectTransform;
    [HideInInspector] public bool isDropped;

    private Canvas canvas;
    private TextMeshProUGUI txt;

    private float _elapsedTime, _desiredDuration = 0.5f;
    public Vector2 _initialPos;
    public Transform parent;


    private void Awake()
    {
        rectTransform = GetComponent<RectTransform>();
        canvas = Utilities.Instance.GET_Canvas();
        txt = GetComponent<TextMeshProUGUI>();

        _initialPos = rectTransform.anchoredPosition;
        parent = transform.parent;
    }


    public void OnBeginDrag(PointerEventData eventData)
    {
        // _initialPos = rectTransform.anchoredPosition;
        txt.raycastTarget = false;
    }


    public void OnDrag(PointerEventData eventData)
    {
        // Update the position of the dragged object based on the mouse position
        rectTransform.anchoredPosition += eventData.delta / canvas.scaleFactor;
    }


    public void OnEndDrag(PointerEventData eventData)
    {
        if (!isDropped)
        {
            StartCoroutine(IENUM_LerpTransform(rectTransform, rectTransform.anchoredPosition, _initialPos));
        }

        txt.raycastTarget = true;
    }


    IEnumerator IENUM_LerpTransform(RectTransform obj, Vector3 currentPosition, Vector3 targetPosition)
    {
        while (_elapsedTime < _desiredDuration)
        {
            _elapsedTime += Time.deltaTime;
            float percentageComplete = _elapsedTime / _desiredDuration;

            obj.anchoredPosition = Vector3.Lerp(currentPosition, targetPosition, percentageComplete);
            yield return null;
        }

        //resetting elapsed time back to zero
        _elapsedTime = 0f;
    }


}
