
using System.Collections;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using TMPro;


public class Drop_ReadAndSolve : MonoBehaviour, IDropHandler, IPointerEnterHandler, IPointerExitHandler
{
    [SerializeField] private Read_and_solve REF_ReadAndSolve;


    private Image img;
    private float _elapsedTime, _desiredDuration = 0.5f;
    private TextMeshProUGUI txt;


    void Start()
    {
        img = GetComponent<Image>();
        txt = GetComponentInChildren<TextMeshProUGUI>();
    }


    public void OnDrop(PointerEventData eventData)
    {
        Drag_ReadAndSolve drag = eventData.pointerDrag.GetComponent<Drag_ReadAndSolve>();

        if (drag != null)
        {
            drag.isDropped = true;
            drag.transform.SetParent(transform);
            drag.rectTransform.anchoredPosition = Vector2.zero;
            REF_ReadAndSolve.SetDraggedValue(gameObject, drag.gameObject, int.Parse(drag.GetComponent<TextMeshProUGUI>().text));
            Utilities.Instance.MOVE(drag.transform, transform.position, 0.5f);
            Utilities.Instance.PlayDrop();
            txt.text = "";
            GetComponent<Drop_ReadAndSolve>().enabled = false;
        }

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

    public void OnPointerEnter(PointerEventData eventData)
    {
        img.color = Color.gray;
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        img.color = new Color(0.83f, 0.83f, 0.83f, 1);  
    }
}