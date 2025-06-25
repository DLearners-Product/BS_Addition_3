using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;


public class Addition_with_regrouping : MonoBehaviour
{

    #region user input
    //==================================================================================================

    //!end of region - unity reference variables 
    //XXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXX
    #endregion



    #region unity reference variables
    //==================================================================================================


    [Space(10)]
    [Header("AUDIO---------------------------------------------------------")]
    [SerializeField] private AudioClip[] ACA_Questions;
    [SerializeField] private AudioClip[] ACA_Answers;


    [Space(10)]
    [Header("TEXTMESHPRO---------------------------------------------------------")]
    [SerializeField] private TextMeshProUGUI TXT_CurrentCounter;


    [Space(10)]
    [Header("BUTTON---------------------------------------------------------")]
    [SerializeField] private Button BTN_ClearLeft;
    [SerializeField] private Button BTN_ClearRight;
    [SerializeField] private Button BTN_Next;


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
    [SerializeField] private ParticleSystem PS_CorrectAnswerBlock;


    //!end of region - unity reference variables
    //XXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXX
    #endregion



    #region local variables
    //==================================================================================================
    private int _currentIndex,
                _ansCount,
                _blockCount;
    private bool _isAnsDone,
                _isBlockDone;
    private List<int> _blockCountList = new List<int>() { 16, 20, 20, 22, 21, 20 };
    private List<TMP_InputField> _inputFieldList;
    private List<GameObject> _dragList;

    private string _result = "",
                    _carry = "";

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
        _ansCount = 0;
        _blockCount = 0;
        _isAnsDone = false;
        _isBlockDone = false;
        _inputFieldList = new List<TMP_InputField>();
        _dragList = new List<GameObject>();


        #region DataSetter
        //Main_Blended.OBJ_main_blended.levelno = 3;
        QAManager.instance.UpdateActivityQuestion();
        qIndex = 0;
        GetData(qIndex);
        GetAdditionalData();
        AssignData();
        #endregion


        ShowQuestion();
    }


    private void ShowQuestion()
    {
        if (_currentIndex != -1)
        {
            TA_Objects[_currentIndex].gameObject.SetActive(false);
        }

        _currentIndex++;

        if (_currentIndex >= TA_Objects.Length)
        {
            ShowActivityCompleted();
            return;
        }

        _inputFieldList.Clear();
        for (int i = 2; i < 5; i++)
            _inputFieldList.Add(TA_Objects[_currentIndex].GetChild(i).GetComponent<TMP_InputField>());

        TA_Objects[_currentIndex].gameObject.SetActive(true);
        _isAnsDone = false;
        _isBlockDone = false;
        _ansCount = 0;
        _blockCount = 0;
        StartCoroutine(IENUM_UpdateCounter());
        G_TransparentScreen.SetActive(false);

        _inputFieldList[0].onValueChanged.AddListener(delegate { CheckAnswer1(); });
        _inputFieldList[1].onValueChanged.AddListener(delegate { CheckAnswer2(); });
        _inputFieldList[2].onValueChanged.AddListener(delegate { CheckAnswer3(); });
        _dragList.Clear();
    }


    private void CheckAnswer1()
    {
        if (_inputFieldList[0].text.Length > 0)
        {
            BTN_ClearLeft.interactable = true;

            if (_inputFieldList[0].text.Equals(TA_Objects[_currentIndex].GetChild(2).name))
            {
                _ansCount++;
                Utilities.Instance.PlayCorrect();

                if (_ansCount == 3)
                {
                    CorrectAnswer();
                }
            }
            else
            {
                WrongAnswer();
            }
        }
    }


    private void CheckAnswer2()
    {
        if (_inputFieldList[1].text.Length > 0)
        {
            BTN_ClearLeft.interactable = true;

            if (_inputFieldList[1].text.Equals(TA_Objects[_currentIndex].GetChild(3).name))
            {
                _ansCount++;
                Utilities.Instance.PlayCorrect();

                if (_ansCount == 3)
                {
                    CorrectAnswer();
                }
            }
            else
            {
                WrongAnswer();
            }
        }
    }


    private void CheckAnswer3()
    {
        if (_inputFieldList[2].text.Length > 0)
        {
            BTN_ClearLeft.interactable = true;

            if (_inputFieldList[2].text.Equals(TA_Objects[_currentIndex].GetChild(4).name))
            {
                _ansCount++;
                Utilities.Instance.PlayCorrect();

                if (_ansCount == 3)
                {
                    CorrectAnswer();
                }
            }
            else
            {
                WrongAnswer();
            }
        }
    }


    private void CorrectAnswer()
    {
        BTN_ClearLeft.interactable = false;
        PS_CorrectAnswer.Play();
        _isAnsDone = true;
        Utilities.Instance.PlayVoice(ACA_Answers[_currentIndex]);


        //extracting ans
        _result = _inputFieldList[0].text + _inputFieldList[1].text;
        _carry = _inputFieldList[2].text;

        string ans = "Result : " + _result + " | Carry : " + _carry;


        //?scoring integration
        ScoreManager.instance.RightAnswer(qIndex, questionID: question.id, answer: ans);

        if (qIndex < ACA_Questions.Length - 1)
            qIndex++;

        GetData(qIndex);


        if (_isAnsDone && _isBlockDone)
        {
            if (_currentIndex == TA_Objects.Length - 1)
                BTN_Next.GetComponentInChildren<TextMeshProUGUI>().text = "Exit";

            Utilities.Instance.ANIM_ShowBounceNormal(BTN_Next.transform);

            G_TransparentScreen.SetActive(true);
        }
    }


    public void WrongAnswer()
    {
        Utilities.Instance.PlayWrong();


        //extracting ans
        _result = _inputFieldList[0].text + _inputFieldList[1].text;
        _carry = _inputFieldList[2].text;

        string ans = "Result : " + _result + " | Carry : " + _carry;


        //?scoring integration
        ScoreManager.instance.WrongAnswer(qIndex, questionID: question.id, answer: ans);

    }


    public void CorrectAnswerBlock(GameObject drag)
    {
        _blockCount++;
        _dragList.Add(drag);
        BTN_ClearRight.interactable = true;

        if (_blockCount == _blockCountList[_currentIndex])
        {
            BTN_ClearRight.interactable = false;
            Utilities.Instance.PlayCorrect();
            PS_CorrectAnswerBlock.Play();
            _isBlockDone = true;

            if (_isAnsDone && _isBlockDone)
            {
                if (_currentIndex == TA_Objects.Length - 1)
                    BTN_Next.GetComponentInChildren<TextMeshProUGUI>().text = "Exit";

                Utilities.Instance.ANIM_ShowBounceNormal(BTN_Next.transform);

                G_TransparentScreen.SetActive(true);
            }
        }
    }


    public void WrongAnswerBlock()
    {

    }


    public void BUT_ClearLeft()
    {
        Utilities.Instance.PlayBubblyButtonClick();

        _inputFieldList[0].text = "";
        _inputFieldList[1].text = "";
        _inputFieldList[2].text = "";

        BTN_ClearLeft.interactable = false;
        _ansCount = 0;
        _isAnsDone = false;
    }


    public void BUT_ClearRight()
    {
        Utilities.Instance.PlayBubblyButtonClick();

        for (int i = 0; i < _dragList.Count; i++)
        {
            _dragList[i].GetComponent<Drag_AdditionWithRegroupingBlock>().isDropped = false;
            _dragList[i].transform.SetParent(_dragList[i].GetComponent<Drag_AdditionWithRegroupingBlock>()._originalParent.transform);

            RectTransform rectTransform = _dragList[i].GetComponent<RectTransform>();
            rectTransform.anchoredPosition = Vector2.zero;
            rectTransform.sizeDelta = _dragList[i].GetComponent<Drag_AdditionWithRegroupingBlock>()._widthAndHeight;
            rectTransform.anchorMin = new Vector2(0.5f, 0.5f);
            rectTransform.anchorMax = new Vector2(0.5f, 0.5f);
        }

        BTN_ClearRight.interactable = false;
        _blockCount = 0;
        _isBlockDone = false;
    }


    public void BUT_Next()
    {
        Utilities.Instance.PlayBubblyButtonClick();
        Utilities.Instance.ANIM_HideBounce(BTN_Next.transform);
        BUT_ClearLeft();
        BUT_ClearRight();

        ShowQuestion();
    }


    private IEnumerator IENUM_UpdateCounter()
    {
        Utilities.Instance.SCALE_X(TXT_CurrentCounter.transform.parent, 0);
        yield return new WaitForSeconds(0.5f);
        TXT_CurrentCounter.text = (_currentIndex + 1) + "/" + TA_Objects.Length;
        Utilities.Instance.SCALE_X(TXT_CurrentCounter.transform.parent, 1);
    }


    public void PlayQuestionVO()
    {
        Utilities.Instance.PlayVoice(ACA_Questions[_currentIndex]);
    }


    private void ShowActivityCompleted()
    {
        BlendedOperations.instance.NotifyActivityCompleted();
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