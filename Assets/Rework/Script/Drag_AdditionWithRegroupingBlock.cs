using System.Collections;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using TMPro;


public class Drag_AdditionWithRegroupingBlock : MonoBehaviour, IDragHandler, IBeginDragHandler, IEndDragHandler
{
    [HideInInspector] public RectTransform rectTransform;
    [HideInInspector] public bool isDropped;

    private Canvas canvas;
    private Image img;
    private float _elapsedTime, _desiredDuration = 0.5f;
    private Vector2 _initialPos;
    [HideInInspector] public Transform _originalParent;
    [HideInInspector] public Vector2 _widthAndHeight;


    private void Awake()
    {
        rectTransform = GetComponent<RectTransform>();
        canvas = Utilities.Instance.GET_Canvas();
        img = GetComponent<Image>();

        _initialPos = rectTransform.anchoredPosition;
        _originalParent = transform.parent;
        _widthAndHeight = GetComponent<RectTransform>().sizeDelta;
    }


    public void OnBeginDrag(PointerEventData eventData)
    {
        img.raycastTarget = false;
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

        img.raycastTarget = true;
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
