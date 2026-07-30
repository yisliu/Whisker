using UnityEngine;
//needed to access button
using UnityEngine.UI;
using System.Collections;
using UnityEngine.EventSystems;
//needed to do scene thing
using DG.Tweening;

public class buttonHover : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
{
    
    private Vector3 initScale;
    [SerializeField] AudioSource newAudio;
    private Tween tween;
    private float scaleBy = 1.2f;
    private float duration = 0.2f;
    private RectTransform rect;
    private Vector3 scaleTo;
    
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        //rect = GetComponent<RectTransform>();
        initScale = transform.localScale;
        //newAudio.SetActive(false);
        //scaleTo = initPos * scaleBy;
    }

    public void OnPointerEnter(PointerEventData pointerData)
    {
        tween?.Kill();
        tween = transform.DOScale(scaleBy*initScale, duration).SetEase(Ease.OutBack);
        newAudio?.Play();
    }

    public void OnPointerExit(PointerEventData pointerData)
    {
        tween?.Kill();
        tween = transform.DOScale(initScale, duration).SetEase(Ease.OutBack);
        //newAudio.SetActive(false);
       // newAudio?.Stop();
       if (newAudio != null)
       {
           newAudio.Stop();
       }
    }

    void OnDestroy()
    {
        tween?.Kill();
    }

}