using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TutorialZone : MonoBehaviour
{
    public int voiceLineIndex;

    private void OnTriggerStay2D(Collider2D collision)
    {
        if (collision.TryGetComponent(out PlayerController player) && !TutorialManager.instance.isInCutscene)
        {
            TutorialManager.instance.PlayVoiceLine(voiceLineIndex);
            Destroy(gameObject);
        }
    }
}
