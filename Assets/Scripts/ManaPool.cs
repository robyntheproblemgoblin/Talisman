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

        GameManager.Instance.m_menuManager.m_falseEnd.SetActive(true) ;
        GameManager.Instance.m_menuManager.m_eventSystem.SetSelectedGameObject(GameManager.Instance.m_menuManager.m_falseQuit.gameObject);
        GameManager.Instance.m_player.m_inputControl.Player_Map.Disable();
        GameManager.Instance.m_player.m_inputControl.UI.Enable();
        Cursor.lockState = CursorLockMode.Confined;
    }
}
