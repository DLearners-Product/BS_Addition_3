using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using System.Linq;



public class Thumbnail4Controller : MonoBehaviour
{
    public string[] questionTexts;
    public GameObject[] questionPuzzleObjs;
    public GameObject[] optionPuzzleObjs;
    public Sprite[] optionPuzzleSprites;
    public Transform counterPanel;
    public GameObject activityCompleted;


    [Space(10)]
    [Header("PARTICLES---------------------------------------------------------")]
    [SerializeField] private ParticleSystem PS_CorrectAnswer;



    List<string> _questionSTRs;
    List<AudioClip> _questionAudioClip;
    List<GameObject> _optionObjs;
    int totalyAnswered = 0;
    private List<int> _newQindexList;



    #region QA

    private int qIndex;
    public GameObject questionGO;
    public GameObject[] optionsGO;
    public Dictionary<string, Component> additionalFields;
    Component question;
    Component[] options;
    Component[] answers;

    #endregion



    private void PrepareNewQindexList()
    {
        _newQindexList = new List<int>();
        string[] _shuffledQues = new string[12];

        for (int i = 0; i < _shuffledQues.Length; i++)
            _shuffledQues[i] = questionPuzzleObjs[i].GetComponentInChildren<TextMeshProUGUI>().text;

        _newQindexList = _shuffledQues.Select(item => System.Array.IndexOf(questionTexts, item)).ToList();

        for (int i = 0; i < _newQindexList.Count; i++)
        {
            Debug.Log(_newQindexList[i]);
        }
    }



    void Start()
    {
        _questionSTRs = new List<string>(questionTexts);
        _optionObjs = new List<GameObject>(optionPuzzleObjs);


        #region DataSetter
        //Main_Blended.OBJ_main_blended.levelno = 3;
        QAManager.instance.UpdateActivityQuestion();
        qIndex = 0;
        GetData(qIndex);
        GetAdditionalData();
        AssignData();
        #endregion


        SpawnQuestionsOptions();
        LowerCounterPanel();
        UpdateCounter();

        PrepareNewQindexList();
    }


    private void OnEnable()
    {
        ImageDragandDrop.onDragStart += OnBoardDragStart;
        ImageDropSlot.onDropInSlot += OnCardDrop;
    }


    private void OnDisable()
    {
        ImageDragandDrop.onDragStart -= OnBoardDragStart;
        ImageDropSlot.onDropInSlot -= OnCardDrop;

        Utilities.Instance.StopAllSounds();
    }

    void OnDestroy()
    {
        _questionSTRs.Clear();
        _optionObjs.Clear();

        Utilities.Instance.StopAllSounds();
    }


    void OnBoardDragStart(GameObject draggedObj)
    {
        draggedObj.transform.SetAsLastSibling();
    }


    void UpdateCounter()
    {
        counterPanel.GetComponentInChildren<TextMeshProUGUI>().text = $"{totalyAnswered} / {questionPuzzleObjs.Length}";
    }


    void LowerCounterPanel()
    {
        Vector3 endPos = counterPanel.position + Vector3.down * 1.5f;
        Utilities.Instance.ANIM_Move(counterPanel, endPos);
    }


    void OnCardDrop(GameObject dragObj, GameObject dropObj)
    {
        var dropSlotTxt = dropObj.transform.GetComponentInChildren<TextMeshProUGUI>().text.Split('+');
        var dragObjTxt = dragObj.GetComponentInChildren<TextMeshProUGUI>().text;
        bool answerMatch = (int.Parse(dropSlotTxt[0].Trim()) + int.Parse(dropSlotTxt[1].Trim())) == int.Parse(dragObjTxt.Trim());

        //?scoring integration
        for (int i = 0; i < _newQindexList.Count; i++)
            if (dropObj.transform.GetComponentInChildren<TextMeshProUGUI>().text.Equals(questionPuzzleObjs[i].GetComponentInChildren<TextMeshProUGUI>().text))
            {
                GetData(_newQindexList[i]);
                break;
            }

        if (answerMatch)
        {
            //*correct answer
            //?scoring integration
            ScoreManager.instance.RightAnswer(qIndex, questionID: question.id, answer: dragObjTxt);
            qIndex++;

            PlayParticles(dropObj.transform.position);
            Utilities.Instance.PlayCorrect();
            totalyAnswered++;
            UpdateCounter();
            Destroy(dropObj.GetComponentInChildren<TextMeshProUGUI>());
            dropObj.GetComponent<Image>().sprite = dragObj.GetComponent<Image>().sprite;
            Destroy(dragObj);

            if (totalyAnswered == questionTexts.Length)
            {
                Utilities.Instance.PlayChildrenClap();
                Invoke(nameof(EnableActivityCompleted), 2f);
            }
        }
        else
        {
            //!wrong answer
            //?scoring integration
            ScoreManager.instance.WrongAnswer(qIndex, questionID: question.id, answer: dragObjTxt);
            Utilities.Instance.PlayWrong();
        }
    }


    void SpawnQuestionsOptions()
    {
        for (int i = 0; i < questionPuzzleObjs.Length; i++)
        {
            var questionText = GetRandomData<string>(ref _questionSTRs);
            var optionObj = GetRandomData<GameObject>(ref _optionObjs);
            var optionTxts = questionText.Split('+');
            var optionTxt = int.Parse(optionTxts[0].Trim()) + int.Parse(optionTxts[1].Trim());

            questionPuzzleObjs[i].transform.GetComponentInChildren<TextMeshProUGUI>().text = questionText;
            optionObj.GetComponentInChildren<TextMeshProUGUI>().text = $"{optionTxt}";
            optionObj.GetComponent<Image>().sprite = optionPuzzleSprites[i];
        }
    }


    T GetRandomData<T>(ref List<T> arr)
    {
        while (true)
        {
            if (arr.Count == 0) break;
            int questionIndex = Random.Range(0, arr.Count);
            var temp = arr[questionIndex];
            arr.RemoveAt(questionIndex);
            return temp;
        }
        return default(T);
    }


    private void PlayParticles(Vector3 pos)
    {
        PS_CorrectAnswer.transform.position = pos;
        PS_CorrectAnswer.Play();
    }


    void EnableActivityCompleted()
    {
        activityCompleted.SetActive(true);
    }


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
