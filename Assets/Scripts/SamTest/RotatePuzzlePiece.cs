using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RotatePuzzlePiece : MonoBehaviour
{
    private float m_interactDistance;
    public Vector3 m_position1;
    public Vector3 m_position2;
    public Vector3 m_position3;
    public bool position1;
    public bool position2;
    public bool position3;
    public float lerpTime;
    public ParticleSystem particle;
    // Start is called before the first frame update
    void Start()
    {
        if(position1 == true && position2 == false && position3 == false)
        {
            transform.rotation = Quaternion.Euler(m_position1);
        }
        else if(position1 == false && position2 == true && position3 == false)
        {
            transform.rotation = Quaternion.Euler(m_position2);
        }
        else if(position1 == false && position2 == false && position3 == true)
        {
            transform.rotation = Quaternion.Euler(m_position3);
        }
        else
        {
            transform.rotation = Quaternion.Euler(m_position1);
        }
    }

    // Update is called once per frame
    void Update()
    {
        
    }
    private void RotateObject()
    {
        /*if (transform.rotation == Quaternion.Euler(m_position1))
        {
            
        }
        else if (transform.rotation == Quaternion.Euler(m_position2))
        {

        }
        else if (transform.rotation == Quaternion.Euler(m_position3))
        {

        }*/
    }
    private void PlayerInfo()
    {
        Ray puzzleRay = Camera.main.ViewportPointToRay(new Vector3(0.5f, 0.5f, 0));
        RaycastHit hit;
        if (Physics.Raycast(puzzleRay, out hit, m_interactDistance))
        {
            RotatePuzzlePiece puzzleRotate = hit.transform.gameObject.GetComponent<RotatePuzzlePiece>();
            if (puzzleRotate != null)
            {
                Debug.Log("Puzzle hit");
                puzzleRotate.RotateObject();
            }
        }
    }
}
