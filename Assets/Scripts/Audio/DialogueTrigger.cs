using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

public class DialogueTrigger : MonoBehaviour
{    
    public AudioSubtitle m_oneOff;
    AudioManager m_audioManager;    

    private void Start()
    {
        m_audioManager = GameManager.Instance.m_audioManager;        
    }


    public void OnTriggerEnter(Collider other)
    {
        if(other.gameObject.GetComponent<PlayerController>() != null)
        {
                m_audioManager.OneOffDialouge(m_oneOff);     
                Destroy(gameObject); 
        }
    }
}

