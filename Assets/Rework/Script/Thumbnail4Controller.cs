using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class Thumbnail4Controller : MonoBehaviour
{
    public string[] questionTexts;
    public GameObject[] questionPuzzleObjs;
    public GameObject[] optionPuzzleObjs;
    public Sprite[] optionPuzzleSprites;
    public Transform counterPanel;
    public GameObject activityCompleted;
    List<string> _questionSTRs;
    List<GameObject> _optionObjs;
    int totalyAnswered = 0;

    void Start()
    {
        _questionSTRs = new List<string>(questionTexts);
        _optionObjs = new List<GameObject>(optionPuzzleObjs);
        SpawnQuestionsOptions();
        LowerCounterPanel();
        UpdateCounter();
    }

    private void OnEnable() {
        ImageDragandDrop.onDragStart += OnBoardDragStart;
        ImageDropSlot.onDropInSlot += OnCardDrop;
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

    void EnableActivityCompleted() => activityCompleted.SetActive(true);

    void OnCardDrop(GameObject dragObj, GameObject dropObj)
    {

        var dropSlotTxt = dropObj.transform.GetComponentInChildren<TextMeshProUGUI>().text.Split('+');
        var dragObjTxt = dragObj.GetComponentInChildren<TextMeshProUGUI>().text;
        bool answerMatch = (int.Parse(dropSlotTxt[0].Trim()) + int.Parse(dropSlotTxt[1].Trim())) == int.Parse(dragObjTxt.Trim());

        if(answerMatch)
        {
            totalyAnswered++;
            UpdateCounter();
            Destroy(dropObj.GetComponentInChildren<TextMeshProUGUI>());
            dropObj.GetComponent<Image>().sprite = dragObj.GetComponent<Image>().sprite;
            Destroy(dragObj);

            if(totalyAnswered == questionTexts.Length) EnableActivityCompleted();
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
            if(arr.Count == 0) break;
            int questionIndex = Random.Range(0, arr.Count);
            var temp = arr[questionIndex];
            arr.RemoveAt(questionIndex);
            return temp;
        }
        return default(T);
    }
}
