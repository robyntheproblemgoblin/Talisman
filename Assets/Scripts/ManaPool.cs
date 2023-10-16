using UnityEngine;

public class ManaPool : MonoBehaviour
{
    public float m_manaAmount;
    public Transform m_respawnPosition;
    public Door m_inDoor;
    public Door m_outDoor;
    public Light m_light;
    public string m_interactMessage = "<sprite=Reticle> Interact";
    public bool m_isFalseEnd;


    public void Interact(PlayerController pc)
    {
        if (m_inDoor != null)
        {
            m_inDoor.CloseDoor();
        }
        if(m_outDoor != null) 
        {
            m_outDoor.OpenDoor();
        }
        pc.AddMana(m_manaAmount);
        pc.m_game.SetCheckPoint(m_respawnPosition);
        if (m_light != null)
        {
            m_light.enabled = false;
        }
        if (m_isFalseEnd)
        {
            GameManager.Instance.m_menuManager.m_falseEnd.SetActive(true);
            GameManager.Instance.m_menuManager.m_eventSystem.SetSelectedGameObject(GameManager.Instance.m_menuManager.m_falseQuit.gameObject);
            GameManager.Instance.m_player.m_inputControl.Player_Map.Disable();
            GameManager.Instance.m_player.m_inputControl.UI.Enable();
            Cursor.lockState = CursorLockMode.Confined;
        }
    }
}
