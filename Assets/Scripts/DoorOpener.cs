﻿using System;
using UnityEngine;

public class DoorOpener : MonoBehaviour
{    
    public DoorCloser m_doorCloser;
    GameManager m_game;
    bool m_hasPassed;

    private void Start()
    {
        m_game = GameManager.Instance;
        m_game.OnGameStateChanged += ResetOnDeath;
        m_doorCloser.m_safetyNet = this;
        gameObject.SetActive(false);
    }

    private void ResetOnDeath(GameState state)
    {
        if(state == GameState.DEATH && !m_hasPassed && gameObject.active)
        {
            m_doorCloser.ResetDoor();
        }

    }

    private void OnDestroy()
    {
        m_game.OnGameStateChanged -= ResetOnDeath;
    }

    private void OnTriggerEnter(Collider other)
    {
     Destroy(m_doorCloser.gameObject);
     Destroy(gameObject);
    }
}