using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Unity.VisualScripting;
using UnityEngine;

public class Puzzle : MonoBehaviour
{
    public Material lightMaterial;
    public MeshRenderer lightNotice;
    public Chosen current;
    public Chosen end;
    bool rotate = false;
    float nextY;
    Quaternion targetRotation;
    int rotations = 0;
    public bool unlocked = false;
    public Door door;

    private void Start()
    {
        nextY = transform.rotation.eulerAngles.y + 120;
        door.puzzleList.Add(this);
    }
    public void RotatePuzzle()
    {
        if (!unlocked && !rotate)
        {
            targetRotation = Quaternion.Euler(0, nextY, 0);
            rotations++;
            rotate = true;
        }
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        if (rotate)
        {            
            transform.RotateAround(transform.position, Vector3.up, 120 * Time.deltaTime);
            if (Quaternion.Angle(transform.rotation, targetRotation) <= 1f)
            {
                transform.rotation = Quaternion.Euler(0, nextY, 0);
                nextY += 120;
                rotate = false;
                if (rotations == 3)
                {
                    rotations = 0;
                    nextY -= 360;                    
                    transform.rotation = Quaternion.Euler(0, nextY, 0);
                }
                if (current == Chosen.GREEN)
                {
                    current = Chosen.RED;
                    Debug.Log("ShouldBeRed");
                }
                else
                {
                    current++;
                }
                if (current == end)
                {
                    Debug.Log("Unlocked");
                    unlocked = true;
                    List<Material> nm = lightNotice.sharedMaterials.ToList();
                    nm.Add(lightMaterial);
                    lightNotice.sharedMaterials = nm.ToArray();
                    door.Unlock();
                }
            }
        }
    }

}

public enum Chosen
{
    RED,
    BLUE,
    GREEN,
}