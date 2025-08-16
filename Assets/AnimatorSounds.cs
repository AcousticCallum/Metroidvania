using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AnimatorSounds : MonoBehaviour
{
    public void PlaySound(string sound)
    {
        SoundManager.instance.PlaySound(sound);
    }
}
