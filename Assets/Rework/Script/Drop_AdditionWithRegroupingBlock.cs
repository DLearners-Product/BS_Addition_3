using System.Collections;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using TMPro;


public class Drop_AdditionWithRegroupingBlock : MonoBehaviour, IDropHandler
{
    [SerializeField] private Addition_with_regrouping REF_AdditionWithRegrouping;


    private float _elapsedTime, _desiredDuration = 0.5f;



    public void OnDrop(PointerEventData eventData)
    {
        Debug.Log("OnDrop");

        Drag_AdditionWithRegroupingBlock drag = eventData.pointerDrag.GetComponent<Drag_AdditionWithRegroupingBlock>();

        if (drag != null)
        {
            if (drag.CompareTag(gameObject.tag))
            {
                Utilities.Instance.PlayDrop();
                drag.isDropped = true;
                drag.transform.SetParent(transform.GetChild(0));
                drag.rectTransform.anchoredPosition = Vector2.zero;
                REF_AdditionWithRegrouping.CorrectAnswerBlock(drag.gameObject);
            }
            else
            {
                REF_AdditionWithRegrouping.WrongAnswerBlock();
            }
        }
        else
        {

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


}