﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using DG.Tweening;
// using System;

public class Utilities : MonoGenericSingleton<Utilities>
{

    [SerializeField] private AudioSource AS_Voice;
    [SerializeField] private AudioSource AS_SFX;
    [SerializeField] private AudioClip AC_Correct;
    [SerializeField] private AudioClip AC_Wrong;
    [SerializeField] private AudioClip AC_Dropped;
    [SerializeField] private AudioClip AC_ChildrenClap;
    [SerializeField] private AudioClip AC_Bubbly;


    private Canvas _canvas;


    protected override void Awake()
    {
        base.Awake();
        _canvas = transform.root.GetComponent<Canvas>();
    }


    public Canvas GET_Canvas() => _canvas;


    public void ANIM_ShowNormal(Transform obj, float duration = 0.5f, TweenCallback callback = null)
    {
        Tween _tween = obj.DOScale(Vector3.one, duration);
        _tween.onComplete += callback;
        _tween.Play();
    }

    public void ScaleObject(Transform obj, float scaleSize = 1.5f, float duration = 0f, TweenCallback callback = null)
    {
        Tween _tween = obj.DOScale(Vector3.one * scaleSize, duration);
        _tween.onComplete += callback;
        _tween.Play();
    }

    public void ANIM_RotateObj(Transform obj, Vector3 rotateDirection, float duration = 0.5f)
    {
        Tween _tween = obj.DORotate(rotateDirection, 0.5f);
        _tween.Play();
    }

    public void ANIM_ShowBounceNormal(Transform obj, float enlargeScaleUpTime = 0.25f, float shrinkUpTime = 0.5f, TweenCallback callback = null)
    {
        Sequence sequence = DOTween.Sequence();
        sequence.Append(obj.DOScale(new Vector3(1.2f, 1.2f, 1.2f), enlargeScaleUpTime));
        sequence.Append(obj.DOScale(Vector3.one, shrinkUpTime));
        sequence.onComplete += callback;
        sequence.Play();
    }

    public void ANIM_ScaleUpDelayScaleDown(Transform obj, float delayTime = 0f)
    {
        Sequence seq = DOTween.Sequence();
        seq.Append(obj.DOScale(Vector3.one * 1.25f, 0.5f));
        seq.AppendInterval(delayTime);
        seq.Append(obj.DOScale(Vector3.one, 0.5f));
        seq.Play();
    }


    public void ANIM_HideNormal(Transform obj) => obj.DOScale(Vector3.zero, 0.5f);


    public void ANIM_HideBounce(Transform obj)
    {
        Sequence sequence = DOTween.Sequence();
        sequence.Append(obj.DOScale(new Vector3(1.2f, 1.2f, 1f), 0.25f));
        sequence.Join(obj.DOScale(new Vector3(0, 0, 0), 0.5f).SetDelay(0.25f));
        sequence.Play();
    }

    public void ANIM_Explode(Transform obj)
    {
        Sequence sequence = DOTween.Sequence();
        sequence.Append(obj.DOScale(Vector3.one, 1f));
        sequence.Join(obj.GetComponent<Image>().DOFade(0, 1f));
        sequence.Play();
    }

    public void ANIM_ImageFill(Image imageObj, float duration = 0.5f, TweenCallback callback = null)
    {
        Tween _tween = imageObj.DOFillAmount(1, duration);
        _tween.onComplete += callback;
        _tween.Play();
    }

    public void ANIM_ImageFade(Image imageObj, float fadeTo = 1f, float duration = 0.5f, TweenCallback callback = null)
    {
        Tween _tween = imageObj.DOFade(fadeTo, duration);
        _tween.onComplete += callback;
        _tween.Play();
    }

    public void ANIM_Move(Transform obj, Vector3 endPos, float movementSpeed = 0.5f, TweenCallback callBack = null)
    {
        var _tween = obj.DOMove(endPos, movementSpeed);
        _tween.onComplete += callBack;
        _tween.Play();
    }

    public void ANIM_MoveWithRandomRotate(Transform obj, Vector3 endPosition, float duration = 0.5f, TweenCallback callback = null)
    {
        Sequence seq = DOTween.Sequence();
        seq.Append(obj.DOMove(endPosition, duration));
        seq.Join(obj.DOShakeRotation(duration));
        seq.onComplete += callback;
        seq.Play();
    }

    public void ANIM_MoveLocal(Transform obj, Vector3 endPos, float movementSpeed = 0.5f, TweenCallback callBack = null)
    {
        var _tween = obj.DOLocalMove(endPos, movementSpeed);
        _tween.onComplete += callBack;
        _tween.Play();
    }

    public void ANIM_MoveWithScaleUp(Transform obj, Vector3 endPos, TweenCallback onCompleteCallBack = null)
    {
        Sequence sequence = DOTween.Sequence();
        sequence.Append(obj.DOScale(Vector3.zero, 0f));
        sequence.Append(obj.DOMove(endPos, 0.25f));
        sequence.Join(obj.DOScale(Vector3.one, 0.5f));
        sequence.onComplete += onCompleteCallBack;
        sequence.Play();
    }

    public void ANIM_MoveWithScaleDown(Transform obj, Vector3 endPos, float moveTime = 0.5f, float scaleTime = 0.5f, TweenCallback onCompleteCallBack = null)
    {
        Sequence sequence = DOTween.Sequence();
        sequence.Append(obj.DOMove(endPos, moveTime));
        sequence.Join(obj.DOScale(Vector3.zero, scaleTime));
        sequence.onComplete += onCompleteCallBack;
        sequence.Play();
    }

    public void ANIM_CorrectScaleEffect(Transform obj, TweenCallback callback = null)
    {
        Sequence seq = DOTween.Sequence();
        seq.Append(obj.DOScale(new Vector3(1.25f, 1.25f, 1), 0.25f));
        seq.Append(obj.DOScale(new Vector3(1, 1, 1), 0.25f));
        seq.SetLoops(3);
        seq.onComplete += callback;
        seq.Play();
    }

    public void ANIM_WrongShakeEffect(Transform obj, TweenCallback callback = null)
    {
        Sequence seq = DOTween.Sequence();
        seq.Append(obj.DOMove(obj.position + new Vector3(0.25f, 0f, 0f), 0.1f));
        seq.Append(obj.DOMove(obj.position - new Vector3(0.25f, 0f, 0f), 0.1f));
        seq.SetLoops(4);
        seq.onComplete += callback;
        seq.Play();
    }

    public void ANIM_RotateAndReveal(Transform rotateObj, TweenCallback callback = null)
    {
        Sequence seq = DOTween.Sequence();
        seq.Append(rotateObj.DORotate(Vector3.up * 90, 0.5f));
        seq.AppendCallback(callback);
        seq.Append(rotateObj.DORotate(Vector3.zero, 0.5f));
        seq.Play();
    }

    public void ANIM_RotateHide(Transform obj, TweenCallback callback = null)
    {
        var tween = obj.DORotate(new Vector3(0, 90, 0), 0.35f);
        tween.onComplete += callback;
    }

    public void ANIM_RotateShow(Transform obj, TweenCallback callback = null)
    {
        var _tween = obj.DORotate(new Vector3(0, 0, 0), 0.35f);
        _tween.onComplete += callback;
    }

    public void ANIM_RotateObjWithCallback(Transform obj, TweenCallback callback = null, TweenCallback callbackOnEnd = null)
    {
        Sequence seq = DOTween.Sequence();
        seq.Append(obj.DORotate(new Vector3(0, 90, 0), 0.35f));
        seq.AppendCallback(callback);
        seq.Append(obj.DORotate(new Vector3(0, 0, 0), 0.35f));
        seq.AppendCallback(callbackOnEnd);
        seq.Play();
    }

    public void ANIM_ShrinkObject(Transform obj, float actionTime = 0.5f, TweenCallback callback = null)
    {
        var tween = obj.DOScale(Vector3.zero, actionTime);
        tween.onComplete += callback;
    }

    public void ANIM_ScaleOnV3(Transform obj, Vector3 shrinkSize, float actionTime = 0.5f, TweenCallback callback = null)
    {
        var tween = obj.DOScale(shrinkSize, actionTime);
        tween.onComplete += callback;
    }

    public void ANIM_BounceEffect(Transform obj, float actionTime = 0.5f, TweenCallback callback = null)
    {
        Sequence seq = DOTween.Sequence();
        seq.Append(obj.DOScaleY(1.5f, 0.25f));
        seq.Append(obj.DOScaleY(0.75f, 0.125f));
        seq.Append(obj.DOScaleY(1.25f, 0.15f));
        seq.Append(obj.DOScaleY(0.85f, 0.125f));
        seq.Append(obj.DOScaleY(1.15f, 0.15f));
        seq.Append(obj.DOScaleY(0.95f, 0.125f));
        seq.Append(obj.DOScaleY(1f, 0.125f));
        seq.OnComplete(callback);
        seq.Play();
    }

    public void ANIM_BoardHangingEffect(Transform obj, TweenCallback callback = null)
    {
        Sequence seq = DOTween.Sequence();
        seq.Append(obj.DORotate(new Vector3(-90, 0, 0), Random.Range(0.7f, 1)));
        seq.Append(obj.DORotate(new Vector3(50, 0, 0), Random.Range(0.7f, 0.8f)));
        seq.Append(obj.DORotate(new Vector3(-40, 0, 0), 0.55f));
        seq.Append(obj.DORotate(new Vector3(30, 0, 0), 0.5f));
        seq.Append(obj.DORotate(new Vector3(-20, 0, 0), 0.4f));
        seq.Append(obj.DORotate(new Vector3(5, 0, 0), 0.25f));
        seq.Append(obj.DORotate(new Vector3(0, 0, 0), 0.15f));
        seq.onComplete += callback;
        seq.SetEase(Ease.InSine).Play();
    }

    public void ANIM_ShakeObj(Transform obj)
    {
        Sequence seq = DOTween.Sequence();
        seq.Append(obj.DORotate(new Vector3(0, 0, 45), 0.35f));
        seq.Append(obj.DORotate(new Vector3(0, 0, -45), 0.25f));
        seq.Append(obj.DORotate(new Vector3(0, 0, 25), 0.15f));
        seq.Append(obj.DORotate(new Vector3(0, 0, -25), 0.05f));
        seq.Append(obj.DORotate(new Vector3(0, 0, 0), 0.05f));
    }

    public void ApplyScaleEffectsToChildObjects(GameObject[] objs, TweenCallback<GameObject> callback = null)
    {
        Sequence seq = DOTween.Sequence();
        for (int i = 0; i < objs.Length; i++)
        {
            var _child = objs[i].transform;
            seq.Append(_child.DOScale(Vector3.one * 1.25f, 0.15f));
            seq.Append(_child.DOScale(Vector3.one, 0.15f));
        }
    }

    public void ANIM_MoveAndReturnToOriginalPos(Transform obj, Vector3 endPosition, TweenCallback callbackOnMid = null, TweenCallback _callbackOnEnd = null, float destReachTime = 1f, float origPosReachTime = 1f)
    {
        Vector3 startPosition = obj.position;
        Sequence sequence = DOTween.Sequence();
        sequence.Append(obj.DOMove(endPosition, destReachTime));
        sequence.AppendCallback(callbackOnMid);
        sequence.Append(obj.DOMove(startPosition, origPosReachTime));
        sequence.AppendCallback(_callbackOnEnd);
        sequence.Play();
    }

    public void ANIM_ScaleEffect(Transform sacleObj, Vector3 scaleSize, TweenCallback callback = null)
    {
        Tween _tween = sacleObj.DOScale(scaleSize, 0.5f);
        _tween.onComplete += callback;
    }

    public void ANIM_WrongEffect(Image obj, float duration = 0.5f, TweenCallback callback = null)
    {
        Sequence seq = DOTween.Sequence();
        seq.Append(obj.DOColor(Color.red, duration));
        seq.Append(obj.DOColor(Color.white, duration));
        seq.SetLoops(3);
        seq.onComplete += callback;
        seq.Play();
    }

    public void ANIM_PlaySeeSaw(Transform obj, Vector3 rotationDirection, TweenCallback callback = null)
    {
        Sequence seq = DOTween.Sequence();
        seq.Append(obj.DORotate(rotationDirection, 0.5f));
        seq.Append(obj.DORotate(-rotationDirection, 0.5f));
        seq.SetLoops(3);
        seq.onComplete += callback;
        seq.Play();
    }

    public void ANIM_FlyIn(Transform obj) => obj.DOMoveY(-1.6f, 2f).SetEase(Ease.OutCirc);
    // public void ANIM_FlyIn(Transform obj) => obj.DOMove(new Vector3(obj.transform.position.x, -1.6f, 0), 2f).SetEase(Ease.OutCirc);



    public void MOVE(Transform obj, Vector3 endPos, float duration, Ease ease = Ease.Linear) => obj.DOMove(endPos, duration).SetEase(ease);

    public void SCALE(Transform obj, float amount, float duration, Ease ease = Ease.Linear) => obj.DOScale(amount, duration).SetEase(ease);

    public void SCALE_X(Transform obj, float endValue) => obj.DOScaleX(endValue, 0.5f);





    public void PlayVoice(AudioClip clip)
    {
        AS_Voice.Stop();
        AS_Voice.clip = clip;
        AS_Voice.Play();
    }


    public void StopVoice() => AS_Voice.Stop();


    public void PlaySFX(AudioClip clip)
    {
        AS_SFX.PlayOneShot(clip);
    }


    public void StopAllSounds()
    {
        AS_Voice.Stop();
        AS_SFX.Stop();
    }


    public void PlayCorrect() => AS_SFX.PlayOneShot(AC_Correct);
    public void PlayWrong() => AS_SFX.PlayOneShot(AC_Wrong);
    public void PlayDrop() => AS_SFX.PlayOneShot(AC_Dropped);
    public void PlayChildrenClap() => AS_SFX.PlayOneShot(AC_ChildrenClap);
    public void PlayBubblyButtonClick() => AS_SFX.PlayOneShot(AC_Bubbly);




}