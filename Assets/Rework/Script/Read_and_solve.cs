using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;


public class Read_and_solve : MonoBehaviour
{

    #region user input
    //==================================================================================================

    public string[] Num;
    public string[] Denom;
    public string[] Tot;

    //!end of region - unity reference variables 
    //XXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXX
    #endregion



    #region unity reference variables
    //==================================================================================================


    [Space(10)]
    [Header("AUDIO---------------------------------------------------------")]
    [SerializeField] private AudioClip[] ACA_Question;
    [SerializeField] private AudioClip[] ACA_Num;
    [SerializeField] private AudioClip[] ACA_Denom;
    [SerializeField] private AudioClip[] ACA_Tot;


    [Space(10)]
    [Header("TEXTMESHPRO---------------------------------------------------------")]
    [SerializeField] private TextMeshProUGUI TXT_CurrentCounter;


    [Space(10)]
    [Header("BUTTON---------------------------------------------------------")]
    [SerializeField] private Button BTN_Check;
    [SerializeField] private Button BTN_Clear;


    [Space(10)]
    [Header("GAME OBJECT---------------------------------------------------------")]
    [SerializeField] private GameObject G_TransparentScreen;
    [SerializeField] private GameObject G_ActivityCompleted;


    [Space(10)]
    [Header("TRANSFORM---------------------------------------------------------")]
    [SerializeField] private Transform[] TA_Objects;


    [Space(10)]
    [Header("PARTICLES---------------------------------------------------------")]
    [SerializeField] private ParticleSystem PS_CorrectAnswer;


    //!end of region - unity reference variables
    //XXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXX
    #endregion



    #region local variables
    //==================================================================================================
    private int _currentIndex;
    private int _numTens, _numOnes,
                _denomTens, _denomOnes,
               _totTens, _totOnes,
               _carryOver;

    private List<GameObject> _draggedObjects;

    //!end of region - local variables
    //XXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXX
    #endregion



    #region QA

    private int qIndex;
    public GameObject questionGO;
    public GameObject[] optionsGO;
    public Dictionary<string, Component> additionalFields;
    Component question;
    Component[] options;
    Component[] answers;

    #endregion



    #region gameplay logic
    //==================================================================================================


    void Start()
    {
        _currentIndex = -1;
        _draggedObjects = new List<GameObject>();


        #region DataSetter
        //Main_Blended.OBJ_main_blended.levelno = 3;
        QAManager.instance.UpdateActivityQuestion();
        qIndex = 0;
        GetData(qIndex);
        GetAdditionalData();
        AssignData();
        #endregion


        StartCoroutine(IENUM_ShowQuestion(0.5f));
    }


    IEnumerator IENUM_ShowQuestion(float delay)
    {
        yield return new WaitForSeconds(delay);

        if (_currentIndex != -1)
        {
            Utilities.Instance.ANIM_HideBounce(TA_Objects[_currentIndex]);
        }

        _currentIndex++;

        yield return new WaitForSeconds(0.65f);

        if (_currentIndex >= TA_Objects.Length)
        {
            yield return new WaitForSeconds(1f);
            ShowActivityCompleted();
            yield break;
        }

        Utilities.Instance.ANIM_ShowBounceNormal(TA_Objects[_currentIndex]);

        G_TransparentScreen.SetActive(false);
        UpdateCounter();
        Reset();
    }


    public void SetDraggedValue(GameObject dropObj, GameObject draggedObj, int value)
    {
        switch (dropObj.name)
        {
            case "NT":
                _numTens = value;
                break;
            case "NO":
                _numOnes = value;
                break;
            case "DT":
                _denomTens = value;
                break;
            case "DO":
                _denomOnes = value;
                break;
            case "TT":
                _totTens = value;
                break;
            case "TO":
                _totOnes = value;
                break;
            case "C":
                _carryOver = value;
                break;
        }

        _draggedObjects.Add(draggedObj);
        BTN_Clear.interactable = true;

        if ((_currentIndex == 1 || _currentIndex == 5))
        {
            if (_numTens != -1 && _numOnes != -1 && _denomTens != -1 && _denomOnes != -1 && _totTens != -1 && _totOnes != -1 && _carryOver != -1)
            {
                BTN_Check.interactable = true;
            }
            else
            {
                BTN_Check.interactable = false;
            }
        }
        else
        {
            if (_numTens != -1 && _numOnes != -1 && _denomTens != -1 && _denomOnes != -1 && _totTens != -1 && _totOnes != -1)
            {
                BTN_Check.interactable = true;
            }
            else
            {
                BTN_Check.interactable = false;
            }
        }
    }


    private void Reset()
    {
        _numTens = -1;
        _numOnes = -1;
        _denomTens = -1;
        _denomOnes = -1;
        _totTens = -1;
        _totOnes = -1;
        _carryOver = -1;

        for (int i = 0; i < TA_Objects[_currentIndex].childCount; i++)
        {
            if (_currentIndex == 1 || _currentIndex == 5)
            {
                if (i == 10)
                {
                    TA_Objects[_currentIndex].GetChild(i).GetComponentInChildren<TextMeshProUGUI>().text = "";
                    TA_Objects[_currentIndex].GetChild(i).GetComponent<Drop_ReadAndSolve>().enabled = true;
                    TA_Objects[_currentIndex].GetChild(i).GetComponent<Image>().color = new Color(0.83f, 0.83f, 0.83f, 1);
                }
            }

            if (i == 2 || i == 3 || i == 5 || i == 6 || i == 8 || i == 9)
            {
                TA_Objects[_currentIndex].GetChild(i).GetComponentInChildren<TextMeshProUGUI>().text = "";
                TA_Objects[_currentIndex].GetChild(i).GetComponent<Drop_ReadAndSolve>().enabled = true;
                TA_Objects[_currentIndex].GetChild(i).GetComponent<Image>().color = new Color(0.83f, 0.83f, 0.83f, 1);
            }
        }

        foreach (GameObject obj in _draggedObjects)
        {
            obj.transform.SetParent(obj.GetComponent<Drag_ReadAndSolve>().parent);
            obj.GetComponent<RectTransform>().anchoredPosition = Vector2.zero;
        }

        _draggedObjects.Clear();
        G_TransparentScreen.SetActive(false);
        BTN_Check.interactable = false;
        BTN_Clear.interactable = false;
    }


    public void BUT_Check() => StartCoroutine(IENUM_Check());
    IEnumerator IENUM_Check()
    {
        G_TransparentScreen.SetActive(true);

        if ((_numTens.ToString() + _numOnes.ToString()) == Num[_currentIndex] &&
           (_denomTens.ToString() + _denomOnes.ToString()) == Denom[_currentIndex] &&
           (_totTens.ToString() + _totOnes.ToString()) == Tot[_currentIndex])
        {
            //* Correct answer
            //?scoring integration
            ScoreManager.instance.RightAnswer(qIndex, questionID: question.id, answer: _totTens.ToString() + _totOnes.ToString());

            if (qIndex < TA_Objects.Length - 1)
                qIndex++;

            GetData(qIndex);

            PS_CorrectAnswer.Play();
            Utilities.Instance.PlayCorrect();

            for (int i = 0; i < TA_Objects[_currentIndex].childCount; i++)
            {
                if (_currentIndex == 1 || _currentIndex == 5)
                {
                    if (i == 10)
                    {
                        TA_Objects[_currentIndex].GetChild(i).GetComponent<Image>().color = Color.green;
                    }
                }

                if (i == 2 || i == 3 || i == 5 || i == 6 || i == 8 || i == 9)
                {
                    TA_Objects[_currentIndex].GetChild(i).GetComponent<Image>().color = Color.green;
                }
            }

            yield return new WaitForSeconds(1f);

            StartCoroutine(IENUM_ShowQuestion(1f));
        }
        else
        {
            //!Incorrect answer
            //?scoring integration
            ScoreManager.instance.WrongAnswer(qIndex, questionID: question.id, answer: _totTens.ToString() + _totOnes.ToString());

            Utilities.Instance.PlayWrong();
            yield return new WaitForSeconds(0.5f);
            G_TransparentScreen.SetActive(false);
        }
    }


    public void BUT_Clear()
    {
        Utilities.Instance.PlayBubblyButtonClick();
        Reset();
    }


    public void PLAY_QuestionVO() => Utilities.Instance.PlayVoice(ACA_Question[_currentIndex]);

    public void PLAY_NumVO() => Utilities.Instance.PlayVoice(ACA_Num[_currentIndex]);

    public void PLAY_DenomVO() => Utilities.Instance.PlayVoice(ACA_Denom[_currentIndex]);

    public void PLAY_TotVO() => Utilities.Instance.PlayVoice(ACA_Tot[_currentIndex]);


    private void UpdateCounter()
    {
        TXT_CurrentCounter.text = (_currentIndex + 1).ToString();
    }


    public void ShowActivityCompleted()
    {
        G_ActivityCompleted.SetActive(true);
    }


    void OnDisable()
    {
        Utilities.Instance.StopAllSounds();
    }


    void OnDestroy()
    {
        Utilities.Instance.StopAllSounds();
    }



    //!end of region - gameplay logic
    //XXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXX
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