using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;
using UnityEngine.UI;
using System.Linq;
using TMPro;
using System;

public class SlotBehaviour : MonoBehaviour
{
    [SerializeField]
    private RectTransform mainContainer_RT;

    [Header("Sprites")]
    [SerializeField]
    private Sprite[] myImages;

    [Header("Slot Images")]
    [SerializeField]
    private List<SlotImage> images;
    [SerializeField]
    private List<SlotImage> Tempimages;

    [Header("Slots Objects")]
    [SerializeField]
    private GameObject[] Slot_Objects;
    [Header("Slots Elements")]
    [SerializeField]
    private LayoutElement[] Slot_Elements;

    [Header("Slots Transforms")]
    [SerializeField]
    private Transform[] Slot_Transform;

    [Header("Line Button Objects")]
    [SerializeField]
    private List<GameObject> StaticLine_Objects;

    [Header("Buttons")]
    [SerializeField]
    private Button SlotStart_Button;

    [Header("Animated Sprites")]
    [SerializeField]
    private Sprite[] Symbol1;
    [SerializeField]
    private Sprite[] Symbol2;
    [SerializeField]
    private Sprite[] Symbol3;
    [SerializeField]
    private Sprite[] Symbol4;
    [SerializeField]
    private Sprite[] Symbol5;
    [SerializeField]
    private Sprite[] Symbol6;
    [SerializeField]
    private Sprite[] Symbol7;
    [SerializeField]
    private Sprite[] Symbol8;

    [Header("Miscellaneous UI")]
    [SerializeField]
    private int[] Lines_num;
    [SerializeField]
    private Button LinePlus_Button;
    [SerializeField]
    private Button LineMinus_Button;
    [SerializeField]
    private TMP_Text Balance_text;
    [SerializeField]
    private TMP_Text TotalBet_text;
    [SerializeField]
    private Image Lines_Image;
    [SerializeField]
    private TMP_Text TotalWin_text;
    [SerializeField]
    private Button AutoSpin_Button;
    [SerializeField]
    private Sprite AutoSpinHover_Sprite;
    [SerializeField]
    private Sprite AutoSpin_Sprite;
    [SerializeField]
    private Image AutoSpin_Image;
    [SerializeField]
    private Button MaxBet_Button;
    [SerializeField]
    private Sprite[] Lines_Sprites;
    private int LineCounter = 0;


    int tweenHeight = 0;

    [SerializeField]
    private GameObject Image_Prefab;

    [SerializeField]
    private GameObject Wild_Prefab;

    [SerializeField]
    private PayoutCalculation PayCalculator;

    private List<Tweener> alltweens = new List<Tweener>();

    [SerializeField]
    private List<ImageAnimation> TempList;

    [SerializeField]
    private int IconSizeFactor = 100;
    [SerializeField] private int SpacingFactor;

    private int numberOfSlots = 5;

    [SerializeField]
    int verticalVisibility = 3;

    [SerializeField]
    private SocketIOManager SocketManager;

    Coroutine AutoSpinRoutine = null;
    bool IsAutoSpin = false;

    [SerializeField] private AudioController audioController;

    private void Start()
    {

        if (SlotStart_Button) SlotStart_Button.onClick.RemoveAllListeners();
        if (SlotStart_Button) SlotStart_Button.onClick.AddListener(delegate { StartSlots(); });

        if (MaxBet_Button) MaxBet_Button.onClick.RemoveAllListeners();
        if (MaxBet_Button) MaxBet_Button.onClick.AddListener(MaxBet);

        if (LinePlus_Button) LinePlus_Button.onClick.RemoveAllListeners();
        if (LinePlus_Button) LinePlus_Button.onClick.AddListener(delegate { ToggleLine(true); });

        if (LineMinus_Button) LineMinus_Button.onClick.RemoveAllListeners();
        if (LineMinus_Button) LineMinus_Button.onClick.AddListener(delegate { ToggleLine(false); });

        if (Lines_Image) Lines_Image.sprite = Lines_Sprites[0];
        LineCounter = 0;

        if (AutoSpin_Button) AutoSpin_Button.onClick.RemoveAllListeners();
        if (AutoSpin_Button) AutoSpin_Button.onClick.AddListener(AutoSpin);
    }

    private void AutoSpin()
    {
        IsAutoSpin = !IsAutoSpin;
        if (IsAutoSpin)
        {
            if (AutoSpin_Image) AutoSpin_Image.sprite = AutoSpinHover_Sprite;
            if (AutoSpinRoutine != null)
            {
                StopCoroutine(AutoSpinRoutine);
                AutoSpinRoutine = null;
            }
            AutoSpinRoutine = StartCoroutine(AutoSpinCoroutine());
        }
        else
        {
            if (AutoSpin_Image) AutoSpin_Image.sprite = AutoSpin_Sprite;
            if (AutoSpinRoutine != null)
            {
                StopCoroutine(AutoSpinRoutine);
                AutoSpinRoutine = null;
            }
        }
    }

    private IEnumerator AutoSpinCoroutine()
    {
        while (true)
        {
            StartSlots(true);
            yield return new WaitForSeconds(10);
        }
    }

    private void MaxBet()
    {
        if (audioController) audioController.PlayButtonAudio();

        if (TotalBet_text) TotalBet_text.text = "99999";
    }

    private void ToggleLine(bool inc)
    {
        if (audioController) audioController.PlayButtonAudio();

        if (inc)
        {
            LineCounter++;
            if (LineCounter == Lines_num.Length)
            {
                LineCounter = 0;
            }
        }
        else
        {
            LineCounter--;
            if (LineCounter == -1)
            {
                LineCounter = Lines_num.Length - 1;
            }
        }
        if (Lines_Image) Lines_Image.sprite = Lines_Sprites[LineCounter];
        PayCalculator.ResetLines();
        for (int i = 0; i < StaticLine_Objects.Count - 9; i++)
        {
            if (i < Lines_num[LineCounter])
            {
                if (StaticLine_Objects[i]) StaticLine_Objects[i].GetComponent<Button>().interactable = true;
                if (StaticLine_Objects[i]) StaticLine_Objects[i].GetComponent<ManageLineButtons>().isEnabled = true;
                if (StaticLine_Objects[i]) StaticLine_Objects[i + 9].GetComponent<Button>().interactable = true;
                if (StaticLine_Objects[i]) StaticLine_Objects[i + 9].GetComponent<ManageLineButtons>().isEnabled = true;
                PayCalculator.GeneratePayoutLinesBackend(i, true);
            }
            else
            {
                if (StaticLine_Objects[i]) StaticLine_Objects[i].GetComponent<Button>().interactable = false;
                if (StaticLine_Objects[i]) StaticLine_Objects[i].GetComponent<ManageLineButtons>().isEnabled = false;
                if (StaticLine_Objects[i]) StaticLine_Objects[i + 9].GetComponent<Button>().interactable = false;
                if (StaticLine_Objects[i]) StaticLine_Objects[i + 9].GetComponent<ManageLineButtons>().isEnabled = false;
            }
        }
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Space) && SlotStart_Button.interactable)
        {
            StartSlots();
        }
    }

    internal void PopulateInitalSlots(int number, List<int> myvalues)
    {
        PopulateSlot(myvalues, number);
    }

    internal void LayoutReset(int number)
    {
        if (Slot_Elements[number]) Slot_Elements[number].ignoreLayout = true;
        if (SlotStart_Button) SlotStart_Button.interactable = true;
    }

    private void PopulateSlot(List<int> values , int number)
    {
        if (Slot_Objects[number]) Slot_Objects[number].SetActive(true);
        for(int i = 0; i<values.Count; i++)
        {
            GameObject myImg = null;
            if (values[i] == 6)
            {
                myImg = Instantiate(Wild_Prefab, Slot_Transform[number]);
            }
            else
            {
                myImg = Instantiate(Image_Prefab, Slot_Transform[number]);
            }
            images[number].slotImages.Add(myImg.transform.GetChild(0).GetComponent<Image>());
            if (values[i] != 6)
            {
                images[number].slotImages[i].sprite = myImages[values[i]];
            }
            PopulateAnimationSprites(images[number].slotImages[i].GetComponent<ImageAnimation>(), values[i]);
        }
        for (int k = 0; k < 2; k++)
        {
            GameObject mylastImg = null;
            if (values[k] == 6)
            {
                mylastImg = Instantiate(Wild_Prefab, Slot_Transform[number]);
            }
            else
            {
                mylastImg = Instantiate(Image_Prefab, Slot_Transform[number]);
            }
            images[number].slotImages.Add(mylastImg.transform.GetChild(0).GetComponent<Image>());
            if (values[k] != 6)
            {
                images[number].slotImages[images[number].slotImages.Count - 1].sprite = myImages[values[k]];
            }
            PopulateAnimationSprites(images[number].slotImages[images[number].slotImages.Count - 1].GetComponent<ImageAnimation>(), values[k]);
        }
        if (mainContainer_RT) LayoutRebuilder.ForceRebuildLayoutImmediate(mainContainer_RT);
        tweenHeight = (values.Count * IconSizeFactor) - 280;
    }

    private void PopulateAnimationSprites(ImageAnimation animScript, int val)
    {
        switch(val)
        {
            case 0:
                for (int i = 0; i < Symbol1.Length; i++)
                {
                    animScript.textureArray.Add(Symbol1[i]);
                }
                break;
            case 1:
                for (int i = 0; i < Symbol2.Length; i++)
                {
                    animScript.textureArray.Add(Symbol2[i]);
                }
                break;
            case 2:
                for (int i = 0; i < Symbol3.Length; i++)
                {
                    animScript.textureArray.Add(Symbol3[i]);
                }
                break;
            case 3:
                for (int i = 0; i < Symbol4.Length; i++)
                {
                    animScript.textureArray.Add(Symbol4[i]);
                }
                break;
            case 4:
                for (int i = 0; i < Symbol5.Length; i++)
                {
                    animScript.textureArray.Add(Symbol5[i]);
                }
                break;
            case 5:
                for (int i = 0; i < Symbol6.Length; i++)
                {
                    animScript.textureArray.Add(Symbol6[i]);
                }
                break;
            case 6:
                //do nothing
                break;
            case 7:
                for (int i = 0; i < Symbol8.Length; i++)
                {
                    animScript.textureArray.Add(Symbol8[i]);
                }
                break;
        }
    }

    private void StartSlots(bool autoSpin = false)
    {
        if (!autoSpin)
        {
            if (AutoSpin_Image) AutoSpin_Image.sprite = AutoSpin_Sprite;
            if (AutoSpinRoutine != null)
            {
                StopCoroutine(AutoSpinRoutine);
                AutoSpinRoutine = null;
            }
        }

        if (audioController) audioController.PlayWLAudio("spin");

        if (SlotStart_Button) SlotStart_Button.interactable = false;
        if (TempList.Count > 0) 
        {
            StopGameAnimation();
        }
        for (int i = 0; i < Tempimages.Count; i++)
        {
            Tempimages[i].slotImages.Clear();
            Tempimages[i].slotImages.TrimExcess();
        }
        PayCalculator.ResetLines();
        StartCoroutine(TweenRoutine());
    }

    private IEnumerator TweenRoutine()
    {
        for (int i = 0; i < numberOfSlots; i++)
        {
            InitializeTweening(Slot_Transform[i]);
            yield return new WaitForSeconds(0.1f);
        }
        SocketManager.AccumulateResult();
        yield return new WaitForSeconds(0.5f);
        List<int> resultnum = SocketManager.tempresult.StopList?.Split(',')?.Select(Int32.Parse)?.ToList();
        for (int i = 0; i < numberOfSlots; i++)
        {
            yield return StopTweening(resultnum[i] + 3, Slot_Transform[i], i);
        }
        yield return new WaitForSeconds(0.3f);
        GenerateMatrix(SocketManager.tempresult.StopList);
        CheckPayoutLineBackend(SocketManager.tempresult.resultLine, SocketManager.tempresult.x_animResult, SocketManager.tempresult.y_animResult);
        KillAllTweens();

        if (SlotStart_Button) SlotStart_Button.interactable = true;
    }

    private void StartGameAnimation(GameObject animObjects, int  LineID) 
    {
        ImageAnimation temp = animObjects.GetComponent<ImageAnimation>();
        temp.StartAnimation();
        TempList.Add(temp);
    }

    private void StopGameAnimation()
    {
        for (int i = 0; i < TempList.Count; i++)
        {
            TempList[i].StopAnimation();
        }
        TempList.Clear();
        TempList.TrimExcess();
    }

    private void CheckPayoutLineBackend(List<int> LineId, List<string> x_AnimString, List<string> y_AnimString)
    {
        List<int> x_anim = null;
        List<int> y_anim = null;

        if (LineId.Count > 0)
        {
        if (audioController) audioController.PlayWLAudio("win");

            for (int i = 0; i < x_AnimString.Count; i++)
            {
                x_anim = x_AnimString[i]?.Split(',')?.Select(Int32.Parse)?.ToList();
                y_anim = y_AnimString[i]?.Split(',')?.Select(Int32.Parse)?.ToList();

                for (int k = 0; k < x_anim.Count; k++)
                {
                    StartGameAnimation(Tempimages[x_anim[k]].slotImages[y_anim[k]].gameObject, LineId[i]);
                }
            }

            for (int i = 0; i < LineId.Count; i++)
            {
                PayCalculator.GeneratePayoutLinesBackend(LineId[i] - 1);
            }

        }
        else {

            if (audioController) audioController.PlayWLAudio("lose");

        }

    }

    private void GenerateMatrix(string stopList)
    {
        List<int> numbers = stopList?.Split(',')?.Select(Int32.Parse)?.ToList();

        for (int i = 0; i < numbers.Count; i++)
        {
            for (int s = 0; s < verticalVisibility; s++)
            {
                Tempimages[i].slotImages.Add(images[i].slotImages[(images[i].slotImages.Count - (numbers[i] + 3)) + s]);
            }
        }
    }

    #region TweeningCode
    private void InitializeTweening(Transform slotTransform)
    {
        slotTransform.localPosition = new Vector2(slotTransform.localPosition.x, 0);
        Tweener tweener = slotTransform.DOLocalMoveY(-tweenHeight, 0.2f).SetLoops(-1, LoopType.Restart).SetDelay(0);
        tweener.Play();
        alltweens.Add(tweener);
    }

    private IEnumerator StopTweening(int reqpos, Transform slotTransform, int index)
    {
        alltweens[index].Pause();
        int tweenpos = (reqpos * (IconSizeFactor+ SpacingFactor)) - (IconSizeFactor+(2* SpacingFactor));
        alltweens[index] = slotTransform.DOLocalMoveY(-tweenpos + 100+(SpacingFactor>0? SpacingFactor/4 : 0), 0.5f).SetEase(Ease.OutElastic);
        yield return new WaitForSeconds(0.2f);
    }


    private void KillAllTweens()
    {
        for (int i = 0; i < numberOfSlots; i++)
        {
            alltweens[i].Kill();
        }
        alltweens.Clear();

    }
    #endregion

}

[Serializable]
public class SlotImage
{
    public List<Image> slotImages = new List<Image>(10);
}

