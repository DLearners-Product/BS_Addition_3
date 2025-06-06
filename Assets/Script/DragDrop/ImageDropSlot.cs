﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using TMPro;


public class ImageDropSlot : MonoBehaviour, IDropHandler
{

    ImageDragandDrop dragitem;
    public delegate void OnDropInSlotDelegate(GameObject dragedObject, GameObject droppedObj);
    public static OnDropInSlotDelegate onDropInSlot;



    public void OnDrop(PointerEventData eventData)
    {
        dragitem = eventData.pointerDrag.GetComponent<ImageDragandDrop>();
        dragitem._isDropped = true;
        onDropInSlot?.Invoke(eventData.pointerDrag, gameObject);

        Utilities.Instance.MOVE(dragitem.transform, transform.position, 0.5f, DG.Tweening.Ease.OutBack);
    }


    public void SetDropedObject()
    {
        dragitem.transform.SetParent(this.transform);
        dragitem.canvasGroup.alpha = 1f;
        dragitem.transform.localPosition = dragitem.currentPos;
        dragitem.enabled = false;
    }


    public void ResetDropedObjectPosition()
    {
        dragitem.ReturnToOriginalPos();
    }
}

