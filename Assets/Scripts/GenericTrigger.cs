using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class GenericTrigger : MonoBehaviour
{
    public UnityEvent eventToTrigger;
    public AudioManager.Tracks musicToTrigger = AudioManager.Tracks.NONE;

    private void OnTriggerEnter2D(Collider2D collision)
    {
        eventToTrigger.Invoke();
        if (musicToTrigger != AudioManager.Tracks.NONE)
        {
            AudioManager.instance.PlayMusic(musicToTrigger, true, 0.5f);
        }
    }

    private void OnDrawGizmos()
    {
        BoxCollider2D collider = GetComponent<BoxCollider2D>();
        if (collider)
        {
            Gizmos.color = Color.yellow;
            Gizmos.DrawWireCube(transform.position, collider.size);
        }
    }

}
