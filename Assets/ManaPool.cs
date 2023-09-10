using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ManaPool : MonoBehaviour
{
    public float m_manaAmount;
    public Transform m_respawnPosition;
    public Door m_door;
    public string m_interactMessage = "Interact";

    public void Interact(PlayerController pc)
    {
        m_door.CloseDoor();
        pc.AddMana(m_manaAmount);
        pc.m_game.SetCheckPoint(m_respawnPosition);
    }
}
