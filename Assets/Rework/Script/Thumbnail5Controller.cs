using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;

public class Thumbnail5Controller : MonoBehaviour
{
    public List<QuestionOptions> questionOptions;
    public TextMeshProUGUI questionText;
    public Transform[] optionObjs;
    public Transform qoDisplayPanel;
    public Transform questionStartPos, questionEndPos;
    public Transform counterPanel;
    public GameObject blockPanel;
    public AudioClip rightSFX, wrongSFX;
    public GameObject activityCompleted;
    int currentIndex = 0;

    void Start()
    {
        SwitchNextQuestion();
        UpdateCounter();
        LowerCounterDisplay();
        DisableActivityCompleted();
    }

    private void LowerCounterDisplay()
    {
        Utilities.Instance.ANIM_Move(counterPanel, counterPanel.transform.position + (Vector3.down * 2.5f));
    }

    void EnableBLockPanel() => blockPanel.SetActive(true);
    void DisableBlockPanel() => blockPanel.SetActive(false);
    void DisableActivityCompleted() => activityCompleted.SetActive(false);
    void EnableActivityCompleted() => activityCompleted.SetActive(true);

    void UpdateCounter() => counterPanel.GetComponentInChildren<TextMeshProUGUI>().text = $"{currentIndex + 1}/{questionOptions.Count}";

    void SwitchNextQuestion()
    {
        if(currentIndex == questionOptions.Count) { EnableActivityCompleted(); return; }

        questionText.text = questionOptions[currentIndex].question;

        for (int i = 0; i < optionObjs.Length; i++)
        {
            optionObjs[i].GetComponentInChildren<TextMeshProUGUI>().text = questionOptions[currentIndex].options[i];
        }
        ResetRightAnswer();
        UpdateCounter();
        DisableBlockPanel();
        Utilities.Instance.ANIM_Move(qoDisplayPanel, new Vector3(0,qoDisplayPanel.position.y,0), callBack: PlayCurrentQuestionVO);
    }

    void ResetRightAnswer()
    {
        foreach (var optObj in optionObjs)
        {
            optObj.transform.GetChild(0).gameObject.SetActive(false);
        }
    }

    void ChangePanel()
    {
        Utilities.Instance.ANIM_Move(qoDisplayPanel, new Vector3(questionEndPos.position.x, qoDisplayPanel.position.y, questionEndPos.position.z), 
        callBack: () => {
            qoDisplayPanel.transform.position = new Vector3(questionStartPos.position.x, qoDisplayPanel.position.y, questionStartPos.position.z);
            currentIndex++;
            SwitchNextQuestion();
        });
    }

#region Listeners

    public void OnOptionClickListeners()
    {
        var selectedOpt = EventSystem.current.currentSelectedGameObject;
        var selOptTxt = selectedOpt.GetComponentInChildren<TextMeshProUGUI>().text;
        if(questionOptions[currentIndex].IsRightAnswer(selOptTxt))
        {
            EnableBLockPanel();
            selectedOpt.transform.GetChild(0).gameObject.SetActive(true);
            AudioManager.PlayAudio(rightSFX);
            Utilities.Instance.ANIM_CorrectScaleEffect(selectedOpt.transform, callback: ChangePanel);
        }else{
            AudioManager.PlayAudio(wrongSFX);
            Utilities.Instance.ANIM_WrongShakeEffect(selectedOpt.transform);
        }
    }

    public void OnSpeakerBTNClicked() => PlayCurrentQuestionVO();

    void PlayCurrentQuestionVO() => AudioManager.PlayAudio(questionOptions[currentIndex].questionCLip);

#endregion

}

[Serializable]
public class QuestionOptions
{
    public string question;
    public AudioClip questionCLip;
    public List<int> rightOptionsIndex;
    public List<string> options;

    public bool IsRightAnswer(string answerSTR) => rightOptionsIndex.Contains(options.IndexOf(answerSTR));
}