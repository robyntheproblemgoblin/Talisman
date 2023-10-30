using Cysharp.Threading.Tasks.Triggers;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class UIFunctions : MonoBehaviour, IPointerEnterHandler, ISelectHandler
{
    AudioManager m_audioManager;

    void Start()
    {
        m_audioManager = GameManager.Instance.m_audioManager;
    }

    public void OnSelect(BaseEventData eventData)
    {
        m_audioManager.OnMenuSelect();
    }    

    public void OnPointerEnter(PointerEventData eventData)
    {
        m_audioManager.OnMenuNavigation();
    }      
}
