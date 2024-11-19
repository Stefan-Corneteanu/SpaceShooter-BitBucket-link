using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RotateToFace : MonoBehaviour
{
    public bool isFacingPlayer = false;
    public bool isFacingTarget = false;
    public GameObject target = null;
    private bool isActive = false;

    public void Activate()
    {
        isActive = true;
    }

    public void Deactivate()
    {
        isActive = false;
    }

    void FixedUpdate()
    {
        if (isActive)
        {
            if (isFacingPlayer || isFacingTarget)
            {
                Vector2 tarPos = Vector2.zero;
                if (isFacingPlayer)
                {
                    if (GameManager.instance && GameManager.instance.playerCrafts[0])
                    {
                        tarPos = GameManager.instance.playerCrafts[0].transform.position;
                        if (GameManager.instance.playerCrafts[1]
                        && (Vector3.Distance(GameManager.instance.playerCrafts[1].transform.position, transform.position)
                        < Vector3.Distance(GameManager.instance.playerCrafts[0].transform.position, transform.position)))
                        {
                            tarPos = GameManager.instance.playerCrafts[1].transform.position;
                        }
                    }
                    else
                    {
                        return;
                    }
                }
                else if (isFacingTarget)
                {
                    if (target)
                    {
                        tarPos = target.transform.position;
                    }
                    else
                    {
                        return;
                    }
                }

                Vector2 dir = (Vector2)transform.position - tarPos;
                dir.Normalize();

                transform.rotation = Quaternion.LookRotation(Vector3.forward, dir);
            }
        }   
    }
}
