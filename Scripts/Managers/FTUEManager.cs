using System;
using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using TMPro;
using UnityEngine;

public class FTUEManager : Singleton<FTUEManager>
{
    [Header("General Components")]
    [SerializeField] private RectTransform hand;
    [SerializeField] private RectTransform tapPos;
    [SerializeField] private RectTransform speedUpTapPos;
    [SerializeField] private GameObject FTUEHeader;
    [SerializeField] private bool DeactivateFTUE;

    private Canvas canvas;
    private Vector3 originalRot;
    private TextMeshProUGUI ftueHeaderText;

    private void Awake()
    {
        canvas = GetComponentInParent<Canvas>();
        originalRot = hand.rotation.eulerAngles;
        ftueHeaderText = FTUEHeader.GetComponentInChildren<TextMeshProUGUI>();

        if (DeactivateFTUE)
        {
            PlayerPrefs.SetInt("isFTUE", 1);
        }
    }

    private void OnEnable()
    {
        Step1 += ProcessStep1;
        Step2 += ProcessStep2;
        Step3 += ProcessStep3;
        Step4 += ProcessStep4;
        Step5 += ProcessStep5;
    }

    private void OnDisable()
    {
        Step1 -= ProcessStep1;
        Step2 -= ProcessStep2;
        Step3 -= ProcessStep3;
        Step4 -= ProcessStep4;
        Step5 -= ProcessStep5;
    }

    #region Step 1

    public Action Step1;

    //Orient player to tap add button
    private void ProcessStep1()
    {
        if (PlayerPrefs.GetInt("isFTUE") <= 0)
        {
            ftueHeaderText.text = "ADD CIRCLE";
            FTUEHeader.SetActive(true);

            hand.anchoredPosition = tapPos.anchoredPosition;
            hand.gameObject.SetActive(true);
            hand.DORotate(new Vector3(30f, 0f, 15f), 0.7f)
                .SetId("FingerTap").SetLoops(-1, LoopType.Yoyo);
        }
    }

    #endregion


    #region Step 2

    public Action Step2;

    private List<Wheel> wheels = new List<Wheel>();
    private int currentTap;
    private bool isFingerMoving = true;
    private bool isStep2Active = true;

    //Orient player to merge wheels
    private void ProcessStep2()
    {
        if (PlayerPrefs.GetInt("isFTUE") <= 0 && isStep2Active)
        {
            if (currentTap < 1)
            {
                wheels.Add(WheelManager.Instance.Wheels[0]);

                currentTap++;
            }
            else
            {
                isStep2Active = false;

                wheels.Add(WheelManager.Instance.Wheels[1]);

                DOTween.Kill("FingerTap");

                StartCoroutine(FingerMove());
            }
        }
    }

    IEnumerator FingerMove()
    {
        hand.rotation = Quaternion.Euler(originalRot);

        ftueHeaderText.text = "DRAG TO MERGE";

        while (isFingerMoving)
        {
            Vector2 startPos = UsefulFunctions.WorldToCanvasPosition(wheels[0].transform, canvas);
            Vector2 endPos = UsefulFunctions.WorldToCanvasPosition(wheels[1].transform, canvas);

            hand.anchoredPosition = startPos;

            hand.DOAnchorPos(endPos, 1f).SetId("FingerMove");

            yield return new WaitForSeconds(1f);
        }
    }

    #endregion


    #region Step 3

    public Action Step3;

    //Orient player to add another wheel
    private void ProcessStep3()
    {
        if (PlayerPrefs.GetInt("isFTUE") <= 0)
        {
            isFingerMoving = false;

            DOTween.Kill("FingerMove");

            SpawnerManager.Instance.IncreaseWallet(200);

            Step1?.Invoke();
            isStep4Active = true;
        }
    }

    #endregion


    #region Step 4

    public Action Step4;

    private bool isStep4Active;
    private bool isFingerIntersectMoving = true;

    //Orient player to intersect wheels
    private void ProcessStep4()
    {
        if (PlayerPrefs.GetInt("isFTUE") <= 0 && isStep4Active)
        {
            DOTween.Kill("FingerTap");

            wheels.Clear();
            wheels.Add(WheelManager.Instance.Wheels[0]);
            wheels.Add(WheelManager.Instance.Wheels[1]);

            StartCoroutine(FingerIntersectMove());
        }
    }

    IEnumerator FingerIntersectMove()
    {
        hand.rotation = Quaternion.Euler(originalRot);

        ftueHeaderText.text = "INTERSECT TO EARN MORE";

        while (isFingerIntersectMoving)
        {
            Vector2 startPos = UsefulFunctions.WorldToCanvasPosition(wheels[1].transform, canvas);
            Vector2 endPos = UsefulFunctions.WorldToCanvasPosition(wheels[0].transform, canvas);

            //Define offset pos to make intersect wheels
            Vector2 endPosOffset = new Vector2(endPos.x - 250f, endPos.y);

            hand.anchoredPosition = startPos;

            hand.DOAnchorPos(endPosOffset, 1f).SetId("FingerMove");

            yield return new WaitForSeconds(1f);

            if (Vector3.Distance(wheels[0].transform.position, wheels[1].transform.position) < 1.3f)
            {
                isStep4Active = false;
                isFingerIntersectMoving = false;
                DOTween.Kill("FingerMove");

                Step5?.Invoke();
            }
        }
    }

    #endregion


    #region Step 5

    public Action Step5;

    //Orient player to tap to speed up
    private void ProcessStep5()
    {
        if (PlayerPrefs.GetInt("isFTUE") <= 0)
        {
            hand.anchoredPosition = speedUpTapPos.anchoredPosition;

            ftueHeaderText.text = "TAP TO SPEED UP";

            hand.DORotate(new Vector3(30f, 0f, 15f), 0.2f)
                .SetId("FingerTap").SetLoops(15, LoopType.Yoyo).OnComplete(() =>
                {
                    hand.gameObject.SetActive(false);
                    FTUEHeader.SetActive(false);
                    PlayerPrefs.SetInt("isFTUE", 1);
                });
        }
    }

    #endregion
}