using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;
using UnityEngine.UI;

public class CardFlip : MonoBehaviour
{
    [SerializeField] internal Sprite cardImage;
    [SerializeField] internal Button Card_Button;

    [SerializeField] private GambleController gambleController;

    private RectTransform Card_transform;

    internal bool once = false;

    private void Start()
    {
        Card_transform = Card_Button.GetComponent<RectTransform>();
        if (Card_Button) Card_Button.onClick.RemoveAllListeners();
        if (Card_Button) Card_Button.onClick.AddListener(FlipMyObject);
    }



    internal void FlipMyObject()
    {

        if (!once && gambleController.gambleStart)
        {

            Card_transform.localEulerAngles = new Vector3(0, 180, 0);
            Card_transform.DORotate(new Vector3(0, 0, 0), 1, RotateMode.FastBeyond360);
            once = true;
            DOVirtual.DelayedCall(0.3f, changeSprite);
        }
    }

    void changeSprite()
    {

        if (Card_Button)
        {
            Card_Button.image.sprite = cardImage;
            Card_Button.interactable = false;
            gambleController.FlipAllCard();
        }
    }

    internal void Reset()
    {
        Card_Button.interactable = true;
        once = false;
    }
}
