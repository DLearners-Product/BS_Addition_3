using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;



public class Thumbnail5Controller : MonoBehaviour
{
    public List<QuestionOptions> questionOptions;
    public TextMeshProUGUI questionText;
    public Transform[] optionObjs;
    public Transform qoDisplayPanel;
    public Transform questionStartPos, questionEndPos;
    public Transform counterPanel;
    public GameObject blockPanel;
    public GameObject activityCompleted;


    [Space(10)]
    [Header("PARTICLES---------------------------------------------------------")]
    [SerializeField] private ParticleSystem PS_CorrectAnswer;


    int currentIndex = 0;



    #region QA

    private int qIndex;
    public GameObject questionGO;
    public GameObject[] optionsGO;
    public Dictionary<string, Component> additionalFields;
    Component question;
    Component[] options;
    Component[] answers;

    #endregion



    void Start()
    {

        #region DataSetter
        //Main_Blended.OBJ_main_blended.levelno = 3;
        QAManager.instance.UpdateActivityQuestion();
        qIndex = 0;
        GetData(qIndex);
        GetAdditionalData();
        AssignData();
        #endregion


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
    
    void EnableActivityCompleted()
    {
        BlendedOperations.instance.NotifyActivityCompleted();
        activityCompleted.SetActive(true);
    }


    void UpdateCounter() => counterPanel.GetComponentInChildren<TextMeshProUGUI>().text = $"{currentIndex + 1}/{questionOptions.Count}";


    void SwitchNextQuestion()
    {
        if (currentIndex == questionOptions.Count)
        {
            EnableActivityCompleted();
            return;
        }

        questionText.text = questionOptions[currentIndex].question;

        for (int i = 0; i < optionObjs.Length; i++)
        {
            optionObjs[i].GetComponentInChildren<TextMeshProUGUI>().text = questionOptions[currentIndex].options[i];
        }
        ResetRightAnswer();
        UpdateCounter();
        DisableBlockPanel();
        Utilities.Instance.ANIM_Move(qoDisplayPanel, new Vector3(0, qoDisplayPanel.position.y, 0), callBack: PlayCurrentQuestionVO);
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
        callBack: () =>
        {
            qoDisplayPanel.transform.position = new Vector3(questionStartPos.position.x, qoDisplayPanel.position.y, questionStartPos.position.z);
            currentIndex++;
            SwitchNextQuestion();
        });
    }


    #region Listeners

    public void OnOptionClickListeners()
    {
        Utilities.Instance.StopVoice();

        var selectedOpt = EventSystem.current.currentSelectedGameObject;
        var selOptTxt = selectedOpt.GetComponentInChildren<TextMeshProUGUI>().text;

        if (questionOptions[currentIndex].IsRightAnswer(selOptTxt))
        {
            //*correct answer
            EnableBLockPanel();
            PlayParticles(selectedOpt.transform.position);
            selectedOpt.transform.GetChild(0).gameObject.SetActive(true);
            Utilities.Instance.PlayCorrect();
            Utilities.Instance.ANIM_CorrectScaleEffect(selectedOpt.transform, callback: ChangePanel);

            //?scoring intetgration
            ScoreManager.instance.RightAnswer(qIndex, questionID: question.id, answerID: GetOptionID(selOptTxt));

            if (qIndex < questionOptions.Count - 1)
                qIndex++;

            GetData(qIndex);

        }
        else
        {
            //!wrong answer
            Utilities.Instance.PlayWrong();
            Utilities.Instance.ANIM_WrongShakeEffect(selectedOpt.transform);

            //?scoring intetgration
            ScoreManager.instance.WrongAnswer(qIndex, questionID: question.id, answerID: GetOptionID(selOptTxt));
        }
    }


    public void PlayCurrentQuestionVO()
    {
        Utilities.Instance.PlayVoice(questionOptions[currentIndex].questionCLip);
    }


    private void PlayParticles(Vector3 pos)
    {
        PS_CorrectAnswer.transform.position = pos;
        PS_CorrectAnswer.Play();
    }


    void OnDisable()
    {
        Utilities.Instance.StopAllSounds();
    }

    void OnDestroy()
    {
        Utilities.Instance.StopAllSounds();
    }


    #endregion



    #region QA

    int GetOptionID(string selectedOption)
    {
        for (int i = 0; i < options.Length; i++)
        {
            if (options[i].text == selectedOption)
            {
                return options[i].id;
            }
        }
        return -1;
    }

    bool CheckOptionIsAns(Component option)
    {
        for (int i = 0; i < answers.Length; i++)
        {
            if (option.text == answers[i].text) { return true; }
        }
        return false;
    }

    void GetData(int questionIndex)
    {
        Debug.Log(">>>>>" + questionIndex);
        question = QAManager.instance.GetQuestionAt(0, questionIndex);
        //if(question != null){
        options = QAManager.instance.GetOption(0, questionIndex);
        answers = QAManager.instance.GetAnswer(0, questionIndex);
        // }
    }

    void GetAdditionalData()
    {
        additionalFields = QAManager.instance.GetAdditionalField(0);
    }

    void AssignData()
    {
        // Custom code
        for (int i = 0; i < optionsGO.Length; i++)
        {
            optionsGO[i].GetComponent<Image>().sprite = options[i]._sprite;
            optionsGO[i].tag = "Untagged";
            Debug.Log(optionsGO[i].name, optionsGO[i]);
            if (CheckOptionIsAns(options[i]))
            {
                optionsGO[i].tag = "answer";
            }
        }
        // answerCount.text = "/"+answers.Length;
    }

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