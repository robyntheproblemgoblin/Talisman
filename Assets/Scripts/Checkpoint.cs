using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Checkpoint : MonoBehaviour
{
    public Bridge[] m_sideBridges;
    public Bridge m_entryBridge;

    private void Start()
    {
        foreach (Bridge bridge in m_sideBridges)
        {
            bridge.SetBridgeState(false);
        }
        m_entryBridge.SetBridgeState(true);
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.GetComponentInParent<PlayerController>() != null)
        {
            foreach (Bridge bridge in m_sideBridges)
            {
                bridge.SetBridgeState(true);
            }
            m_entryBridge.SetBridgeState(false);
            Destroy(this.gameObject);
        }
    }
}