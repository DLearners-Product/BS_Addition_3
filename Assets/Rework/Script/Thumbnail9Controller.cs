using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;



public class Thumbnail9Controller : MonoBehaviour
{

    public List<T8Questions> questions;
    public TextMeshProUGUI mainQuestion, ques1, ques2, ques3;
    public TextMeshProUGUI answerSlot1, answerSlot2, answerSlot3;
    public Transform boardObj;
    public GameObject counterObj;
    public GameObject activityCompleted;
    int currentIndex = 0;
    Vector3 currentBoardPosition;



    private string _numTens, _numOnes, _denomTens, _denomOnes;


    void Start()
    {
        ShowQuestion();
        PlayQuestionsAudio();
        UpdateCounter();
        ShowCounter();
        currentBoardPosition = boardObj.position;
    }

    private void OnEnable()
    {
        ImageDropSlot.onDropInSlot += OnObjectDrop;
    }


    private void OnDisable()
    {
        ImageDropSlot.onDropInSlot -= OnObjectDrop;
    }


    void OnObjectDrop(GameObject dragObject, GameObject dropSlotObject)
    {
        Debug.Log("OnObjectDrop");

        if (dropSlotObject.transform.parent.name.Equals("N"))
        {
            if (dropSlotObject.CompareTag("ones"))
            {
                _numOnes = dragObject.GetComponentInChildren<TextMeshProUGUI>().text;
            }
            else if (dragObject.CompareTag("tens"))
            {
                _numTens = dragObject.GetComponentInChildren<TextMeshProUGUI>().text;
            }

            answerSlot1.text = _numTens + _numOnes;
            Debug.Log(answerSlot1.text);
        }
        else if (dropSlotObject.transform.parent.name.Equals("D"))
        {
            if (dropSlotObject.CompareTag("ones"))
            {
                _denomOnes = dragObject.GetComponentInChildren<TextMeshProUGUI>().text;
            }
            else if (dropSlotObject.CompareTag("tens"))
            {
                _denomTens = dragObject.GetComponentInChildren<TextMeshProUGUI>().text;
            }

            answerSlot2.text = _denomTens + _denomOnes;
            Debug.Log(answerSlot2.text);
        }
    }


    void ShowCounter() => Utilities.Instance.ANIM_Move(counterObj.transform, counterObj.transform.position + (Vector3.left * 3f));


    void UpdateCounter() => counterObj.GetComponentInChildren<TextMeshProUGUI>().text = $"{currentIndex + 1}/{questions.Count}";


    void ShowQuestion()
    {
        mainQuestion.text = questions[currentIndex].mainQues;
        ques1.text = questions[currentIndex].ques1;
        ques2.text = questions[currentIndex].ques2;
        ques3.text = questions[currentIndex].ques3;
    }


    public void OnSubmitBtnClicked()
    {
        bool ans1IsRight = answerSlot1.text.Equals(questions[currentIndex].ans1);
        bool ans2IsRight = answerSlot2.text.Equals(questions[currentIndex].ans2);
        bool ans3IsRight = answerSlot3.text.Equals(questions[currentIndex].ans3);

        if (ans1IsRight && ans2IsRight && ans3IsRight)
        {
            Invoke(nameof(MoveBoardLeft), 1.5f);
        }
        else ResetAnswers();
    }


    IEnumerator PlayAuido(Transform questionObj, AudioClip questionClip, Action callback = null)
    {
        Utilities.Instance.ANIM_ScaleEffect(questionObj, Vector3.one * 1.15f);
        var audioManager = AudioManager.PlayAudio(questionClip);
        yield return new WaitForSeconds(questionClip.length);
        callback?.Invoke();
        Utilities.Instance.ANIM_ScaleEffect(questionObj, Vector3.one);
    }


    void MoveBoardLeft()
    {
        Utilities.Instance.ANIM_Move(boardObj, currentBoardPosition + (Vector3.left * 20), callBack: () =>
        {
            ChangeQuestion();
            Utilities.Instance.ANIM_Move(boardObj, currentBoardPosition, callBack: PlayQuestionsAudio);
        });
    }


    private void PlayQuestionsAudio()
    {
        StartCoroutine(PlayAuido(
                        mainQuestion.transform,
                        questions[currentIndex].questionClip,
                        callback: () =>
                        StartCoroutine(PlayAuido(
                            ques1.transform,
                            questions[currentIndex].ques1Clip,
                            () => StartCoroutine(PlayAuido(
                                ques2.transform,
                                questions[currentIndex].ques2Clip,
                                () => StartCoroutine(PlayAuido(
                                    ques3.transform,
                                    questions[currentIndex].ques3Clip
                                ))
                            ))
                        ))
                    )
                );
    }


    void ResetAnswers()
    {
        answerSlot1.text = "0";
        answerSlot2.text = "0";
        answerSlot3.text = "0";
    }


    void ChangeQuestion()
    {
        currentIndex++;

        if (currentIndex == questions.Count) { EnableActivityCompleted(); return; }

        UpdateCounter();
        ShowQuestion();
        ResetAnswers();
    }

    void EnableActivityCompleted() => activityCompleted.SetActive(true);
}


[Serializable]
public class T8Questions
{
    public string mainQues;
    public AudioClip questionClip;
    public string ques1, ques2, ques3;
    public AudioClip ques1Clip, ques2Clip, ques3Clip;
    public string ans1, ans2, ans3;
}