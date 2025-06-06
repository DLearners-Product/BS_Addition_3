using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;



public class Thumbnail3Controller : MonoBehaviour
{

    [SerializeField] private GameObject G_TransparentScreen;


    public AudioClip sucessSFX, wrongSFx;


    [Header("Activity 1")]
    public AudioClip activity1InstructionVO;
    public string[] questionTexts;
    public AudioClip[] questionsClip;
    public Transform[] questionSpawnPositions;
    public GameObject questionSpawnPrefab;
    public Transform questionSpawnParent;
    public Button nextBTN;
    public int totalAnswerCount;
    int totalAnsweredCount = 0;
    List<GameObject> spawnedQuestionObjects;



    [Header("Activity 2")]
    public AudioClip activity2InstructionVO;
    public Transform position1;
    public Transform position2;
    public Transform answerSpawnPosition;
    public Transform marketShaft;
    public Transform basket1, basket2;
    public Transform vegetableSpawnPoint1, vegetableSpawnPoint2, vegetableSpawnPoint3;
    public GameObject vegetableSpawnPrefab;
    public Sprite[] vegetableSprites;
    public string[] questionSTR;
    public GameObject symbol1, symbol2;
    public Transform answerContainPanel;
    public GameObject[] countDisplayPanels;
    public Button validateBTN;
    public Button activity2NextBTN;
    int currentIndex = 0;
    Vector3 basket1InitialPosition, basket2InitialPosition, answerPanelIntialPosition;



    [Header("Activity 3")]
    public AudioClip activity3InstructionVO;
    public Transform optionsSpawnParent;
    public Transform[] questionPanels, optionsSpawnPositions;
    public Transform[] questionSpawnPoints;
    public Transform[] baskets;
    public GameObject optionPrefab;
    public string[] optionTexts;
    public GameObject activityCompleted;
    Vector3[] questionInitialPosition;
    int act3AnsweredCount = 0;


    void Start()
    {
        G_TransparentScreen.SetActive(true);
        spawnedQuestionObjects = new List<GameObject>();
        nextBTN.interactable = false;
        SpawnQuestion();
        ResetActivity2();
        // OpenShopShaft();
        ResetActivity3();
        // StartCoroutine(ShowQuestionPanel());
    }


    private void OnEnable()
    {
        ImageDragandDrop.onDrag += OnOptionDrag;
        ImageDragandDrop.onDragEnd += OnOptionDragEnd;
        ImageDropSlot.onDropInSlot += OnOptionDrop;
    }

    private void OnDisable()
    {
        ImageDragandDrop.onDrag -= OnOptionDrag;
        ImageDragandDrop.onDragEnd -= OnOptionDragEnd;
        ImageDropSlot.onDropInSlot -= OnOptionDrop;

        Utilities.Instance.StopAllSounds();
    }


    void OnDestroy()
    {
        Utilities.Instance.StopAllSounds();
    }


    #region ACTIVITY_1

    void SpawnQuestion(int spawnIndex = 0)
    {
        if (spawnIndex == questionTexts.Length)
        {
            AudioManager.PlayAudio(activity1InstructionVO);
            Invoke(nameof(DisableTransparentScreen), activity1InstructionVO.length);
            return;
        }

        GameObject spawnedQuestion = Instantiate(questionSpawnPrefab, questionSpawnParent);
        spawnedQuestionObjects.Add(spawnedQuestion);

        spawnedQuestion.AddComponent<HoverAudio>().clip = questionsClip[spawnIndex];

        spawnedQuestion.transform.GetComponentInChildren<TextMeshProUGUI>().text = questionTexts[spawnIndex];
        spawnedQuestion.AddComponent<Button>().onClick.AddListener(OnQuestionPanelClicked);
        Utilities.Instance.ANIM_MoveWithScaleUp(spawnedQuestion.transform, questionSpawnPositions[spawnIndex].position, onCompleteCallBack: () =>
        {
            SpawnQuestion(++spawnIndex);
        });
    }


    void OnQuestionPanelClicked()
    {
        var selectedObj = EventSystem.current.currentSelectedGameObject;
        var clickedObjText = selectedObj.transform.GetComponentInChildren<TextMeshProUGUI>().text;
        string[] splitedText = clickedObjText.Split('+');
        bool sumIs10 = Int32.Parse(splitedText[0]) + Int32.Parse(splitedText[1]) == 10;

        if (sumIs10)
        {
            Utilities.Instance.ANIM_CorrectScaleEffect(selectedObj.transform, callback: () =>
            {
                selectedObj.transform.GetChild(0).gameObject.SetActive(true);
                totalAnsweredCount++;
                if (totalAnsweredCount == totalAnswerCount) nextBTN.interactable = true;
            });
            AudioManager.PlayAudio(sucessSFX);
        }
        else
        {
            AudioManager.PlayAudio(wrongSFx);
            Utilities.Instance.ANIM_WrongShakeEffect(selectedObj.transform);
        }
    }


    public void OnNextBtnClick()
    {
        ShrinkQuestion();
    }


    void ShrinkQuestion(int index = 1)
    {
        if (index > spawnedQuestionObjects.Count) { nextBTN.gameObject.SetActive(false); OpenShopShaft(); return; }

        Utilities.Instance.ANIM_MoveWithScaleDown(spawnedQuestionObjects[spawnedQuestionObjects.Count - index].transform, Vector3.zero, 0.25f, 0.25f, onCompleteCallBack: () =>
        {
            spawnedQuestionObjects[spawnedQuestionObjects.Count - index].SetActive(false);
            ShrinkQuestion(++index);
        });
    }


    #endregion



    #region ACTIVITY_2

    void ResetActivity2()
    {
        basket1InitialPosition = basket1.position;
        basket2InitialPosition = basket2.position;
        answerPanelIntialPosition = answerContainPanel.position;

        Utilities.Instance.ANIM_ShrinkObject(symbol1.transform, 0f);
        Utilities.Instance.ANIM_ShrinkObject(symbol2.transform, 0f);
        Utilities.Instance.ANIM_ShrinkObject(validateBTN.transform, 0f);

        for (int i = 0; i < countDisplayPanels.Length; i++)
        {
            Utilities.Instance.ANIM_ShrinkObject(countDisplayPanels[i].transform, 0f);
            countDisplayPanels[i].transform.GetComponentInChildren<TextMeshProUGUI>().text = "0";
        }
        Utilities.Instance.ANIM_ScaleOnV3(marketShaft.transform, new Vector3(1, 0, 1), 0f);
    }


    void OpenShopShaft()
    {
        Utilities.Instance.ANIM_BounceEffect(marketShaft, callback: SpawnActivity2Question);
    }


    void SpawnActivity2Question() => MoveBasket(basket1, position1);


    void MoveBasket(Transform basketObj, Transform moveTransform, int dropCount = 0)
    {
        if (dropCount == 2) { SpawnSymbols(symbol1.transform); return; }
        Utilities.Instance.ANIM_Move(basketObj, moveTransform.position, callBack: () =>
        {
            MoveBasket(basket2, position2, ++dropCount);
        });
    }


    void StartSpawnVegetablesOnAllBasket()
    {
        int[] questionCount = GetQuestionInt();

        // Debug.Log($"Question Count 1 :: {questionCount[0]}");
        // Debug.Log($"Question Count 2 :: {questionCount[1]}");

        StartCoroutine(SpawnVegetables(vegetableSpawnPoint1, basket1, countDisplayPanels[0].transform, questionCount[0], () =>
        {
            StartCoroutine(SpawnVegetables(vegetableSpawnPoint2, basket2, countDisplayPanels[1].transform, questionCount[1], () =>
            {
                Utilities.Instance.ANIM_ShowNormal(validateBTN.transform, callback: () =>
                {
                    Utilities.Instance.ANIM_ScaleOnV3(countDisplayPanels[countDisplayPanels.Length - 1].transform, Vector3.one * 1.15f, callback: () =>
                    {
                        countDisplayPanels[countDisplayPanels.Length - 1].GetComponentInChildren<TMP_InputField>().ActivateInputField();
                        if (currentIndex == 0)
                            AudioManager.PlayAudio(activity2InstructionVO);
                    });
                });
            }));
        }));
    }


    void UpdateVegetableDisplayCounter(Transform updateObj)
    {
        updateObj.GetComponentInChildren<TextMeshProUGUI>().text = $"{Int16.Parse(updateObj.GetComponentInChildren<TextMeshProUGUI>().text) + 1}";
    }


    IEnumerator SpawnVegetables(Transform spawnParent, Transform basketObj, Transform counterObj, int spawnCount, Action func = null)
    {
        // Debug.Log($"SPawn Count :: {spawnCount}");
        // Debug.Log($"Baskete :: {basketObj.name}");

        int count = 0;
        while (count < spawnCount)
        {
            var spawnedVegetable = Instantiate(vegetableSpawnPrefab, spawnParent);
            spawnedVegetable.transform.position = new Vector3(
                spawnParent.position.x + UnityEngine.Random.Range(-1f, 1f),
                spawnParent.position.y,
                spawnParent.position.z
            );
            spawnedVegetable.GetComponent<Image>().sprite = vegetableSprites[currentIndex];
            spawnedVegetable.GetComponent<Image>().preserveAspect = true;

            var endPosition = new Vector3(
                spawnedVegetable.transform.position.x,
                position1.position.y,
                position1.position.z
            );

            spawnedVegetable.transform.parent = basketObj;
            spawnedVegetable.transform.SetSiblingIndex(1);

            Utilities.Instance.ANIM_MoveWithRandomRotate(spawnedVegetable.transform, endPosition, callback:
                () =>
                {
                    if (counterObj != null)
                        UpdateVegetableDisplayCounter(counterObj);
                }
            );
            yield return new WaitForSeconds(0.5f);
            count++;
        }
        if (func != null) func();
    }


    void SpawnSymbols(Transform symbolTransform, int index = 0)
    {
        if (index == 2) { SpawnAnswerPanel(); return; }
        Utilities.Instance.ANIM_ShowBounceNormal(symbolTransform, callback: () =>
        {
            SpawnSymbols(symbol2.transform, ++index);
        });
    }


    void SpawnAnswerPanel()
    {
        Debug.Log("Came to SpawnAnswerPanel...");
        Utilities.Instance.ANIM_Move(answerContainPanel, answerSpawnPosition.position, callBack: () => EnableCountDisplayPanel());
    }


    void EnableCountDisplayPanel()
    {
        for (int i = 0; i < countDisplayPanels.Length; i++)
        {
            Utilities.Instance.ANIM_ShowBounceNormal(countDisplayPanels[i].transform);
            if (i == (countDisplayPanels.Length - 1))
            {
                StartSpawnVegetablesOnAllBasket();
            }
        }
    }


    int[] GetQuestionInt()
    {
        int[] tmp = new int[2];
        string[] quesSplit = questionSTR[currentIndex].Split('+');

        tmp[0] = Int32.Parse(quesSplit[0]);
        tmp[1] = Int32.Parse(quesSplit[1]);

        return tmp;
    }


    public void OnValidateBTNClick()
    {

        if (countDisplayPanels[countDisplayPanels.Length - 1].GetComponentInChildren<TMP_InputField>().text.Equals("")) return;

        int answerInt = Int16.Parse(countDisplayPanels[countDisplayPanels.Length - 1].GetComponentInChildren<TMP_InputField>().text);

        if (answerInt == 0) return;

        StartCoroutine(SpawnVegetables(vegetableSpawnPoint3, answerContainPanel, null, answerInt, EvaluateAnswer));
    }


    void EvaluateAnswer()
    {
        int answerInt = Int16.Parse(countDisplayPanels[countDisplayPanels.Length - 1].GetComponentInChildren<TMP_InputField>().text);
        int q1TextInt = Int16.Parse(countDisplayPanels[0].GetComponentInChildren<TextMeshProUGUI>().text);
        int q2TextInt = Int16.Parse(countDisplayPanels[1].GetComponentInChildren<TextMeshProUGUI>().text);
        if (answerInt == (q1TextInt + q2TextInt))
        {
            AudioManager.PlayAudio(sucessSFX);

            if (currentIndex == questionSTR.Length - 1)
            {
                EnableNextBTN();
            }
            else
            {
                Utilities.Instance.ANIM_CorrectScaleEffect(answerContainPanel, callback: MoveUpAndChangeQuestion);
            }
        }
        else
        {
            AudioManager.PlayAudio(wrongSFx);

            Utilities.Instance.ANIM_WrongShakeEffect(answerContainPanel, callback: () => DestroySpawnedVegetableChildObjs(answerContainPanel));
        }
    }

    void MoveUpAndChangeQuestion()
    {
        Utilities.Instance.ANIM_Move(basket1, basket1InitialPosition, callBack: () => DestroySpawnedVegetableChildObjs(basket1));
        Utilities.Instance.ANIM_Move(basket2, basket2InitialPosition, callBack: () => DestroySpawnedVegetableChildObjs(basket2));
        Utilities.Instance.ANIM_Move(answerContainPanel, answerPanelIntialPosition, callBack: () =>
        {
            currentIndex++;
            DestroySpawnedVegetableChildObjs(answerContainPanel);
            ResetCounters();
        });
    }


    void DestroySpawnedVegetableChildObjs(Transform parentObj)
    {
        int answeCount = parentObj.childCount;
        for (int i = 0; i < answeCount; i++)
        {
            if (!parentObj.GetChild(i).name.ToLower().Contains("basket"))
            {
                Destroy(parentObj.GetChild(i).gameObject);
            }
        }
    }


    void ResetCounters(int index = 0)
    {
        if (index == (countDisplayPanels.Length - 1))
        {
            Utilities.Instance.ANIM_ShowBounceNormal(countDisplayPanels[index].transform, callback: () =>
            {
                countDisplayPanels[index].transform.GetComponentInChildren<TMP_InputField>().text = "";
                SpawnActivity2Question();
            });
        }
        else
        {
            Utilities.Instance.ANIM_ShowBounceNormal(countDisplayPanels[index].transform, callback: () =>
            {
                countDisplayPanels[index].transform.GetComponentInChildren<TextMeshProUGUI>().text = "0";
                ResetCounters(++index);
            });
        }
    }


    void EnableNextBTN()
    {
        Utilities.Instance.ANIM_ShrinkObject(validateBTN.transform, callback: () =>
        {
            Utilities.Instance.ANIM_ShowNormal(activity2NextBTN.transform);
        });
    }


    public void OnAct2NextBTNClick()
    {
        Utilities.Instance.ANIM_ShrinkObject(symbol1.transform);
        Utilities.Instance.ANIM_ShrinkObject(symbol2.transform);
        Utilities.Instance.ANIM_ShrinkObject(activity2NextBTN.transform);
        Utilities.Instance.ANIM_ShrinkObject(countDisplayPanels[0].transform);
        Utilities.Instance.ANIM_ShrinkObject(countDisplayPanels[1].transform);
        Utilities.Instance.ANIM_ShrinkObject(countDisplayPanels[2].transform);

        Utilities.Instance.ANIM_Move(basket1, basket1InitialPosition, callBack: () => DestroySpawnedVegetableChildObjs(basket1));
        Utilities.Instance.ANIM_Move(basket2, basket2InitialPosition, callBack: () => DestroySpawnedVegetableChildObjs(basket2));
        Utilities.Instance.ANIM_Move(answerContainPanel, answerPanelIntialPosition, callBack: () =>
        {
            DestroySpawnedVegetableChildObjs(answerContainPanel);
            Utilities.Instance.ANIM_ScaleOnV3(marketShaft, new Vector3(1, 0, 1), callback: () => StartCoroutine(ShowQuestionPanel()));
        });
    }


    #endregion



    #region ACTIVITY_3

    void ResetActivity3()
    {
        questionInitialPosition = new Vector3[questionSpawnPoints.Length];
        for (int i = 0; i < questionPanels.Length; i++)
        {
            questionInitialPosition[i] = questionPanels[i].position;
            Utilities.Instance.ANIM_Move(questionPanels[i], questionSpawnPoints[i].position, 0f);
        }
    }


    IEnumerator ShowQuestionPanel(int index = 0)
    {
        if (index == questionPanels.Length)
        {
            StartCoroutine(SpawnOption());
            yield break;
        }

        Utilities.Instance.ANIM_Move(questionPanels[index], questionInitialPosition[index]);

        yield return new WaitForSeconds(0.25f);

        StartCoroutine(ShowQuestionPanel(++index));
    }


    IEnumerator SpawnOption(int index = 0)
    {
        if (index == optionsSpawnPositions.Length)
        {
            AudioManager.PlayAudio(activity3InstructionVO);
            yield break;
        }

        var spawnedOpt = Instantiate(optionPrefab, optionsSpawnParent);
        spawnedOpt.transform.position = optionsSpawnPositions[index].position;
        spawnedOpt.GetComponentInChildren<TextMeshProUGUI>().text = optionTexts[index];
        spawnedOpt.GetComponent<ImageDragandDrop>().ResetInitialPositoin();
        Utilities.Instance.ANIM_ShowBounceNormal(spawnedOpt.transform);

        yield return new WaitForSeconds(0.25f);

        StartCoroutine(SpawnOption(++index));
    }


    void OnOptionDrop(GameObject dragedObj, GameObject dropSlotObj)
    {
        string dragObjText = dragedObj.transform.GetComponentInChildren<TextMeshProUGUI>().text;
        string dropSlotText = dropSlotObj.transform.parent.parent.GetChild(0).name;

        if (dragObjText.Equals(dropSlotText))
        {
            AudioManager.PlayAudio(sucessSFX);
            Destroy(dragedObj);

            act3AnsweredCount++;

            var optObj = dropSlotObj.transform.parent.GetChild(2);
            optObj.gameObject.SetActive(true);
            optObj.GetComponentInChildren<TextMeshProUGUI>().text = dragObjText;
        }
        else
        {
            AudioManager.PlayAudio(wrongSFx);
        }

        if (act3AnsweredCount == optionTexts.Length)
        {
            StartCoroutine(IENUM_ShowActivityCompleted());
        }
    }


    IEnumerator IENUM_ShowActivityCompleted()
    {
        yield return new WaitForSeconds(1f);
        activityCompleted.SetActive(true);
    }


    void OnOptionDrag(GameObject dragObject)
    {
        HighlightNearbyBasket(dragObject.transform);
    }


    void OnOptionDragEnd(GameObject dropObject) => ResetBasketHiglights();


    void HighlightNearbyBasket(Transform fruitObj)
    {
        ResetBasketHiglights();
        List<float> distances = new List<float>();
        float _distance = 0f;

        int minIndex = 0;
        _distance = Vector3.Distance(fruitObj.position, baskets[minIndex].position);

        for (int i = 1; i < baskets.Length; i++)
        {
            if (_distance > Vector3.Distance(fruitObj.position, baskets[i].position))
            {
                minIndex = i;
                _distance = Vector3.Distance(fruitObj.position, baskets[i].position);
            }
        }
        baskets[minIndex].GetChild(0).gameObject.SetActive(true);
    }


    void ResetBasketHiglights()
    {
        for (int i = 0; i < baskets.Length; i++)
        {
            baskets[i].transform.GetChild(0).gameObject.SetActive(false);
        }
    }


    #endregion


    private void DisableTransparentScreen()
    {
        G_TransparentScreen.SetActive(false);
    }





}
