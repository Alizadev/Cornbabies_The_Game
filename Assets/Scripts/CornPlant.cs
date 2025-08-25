using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CornPlant : MonoBehaviour
{
    bool makingNoise = false;
    public void MakeNoise()
    {
        if (makingNoise)
        {
            return;
        }
        StartCoroutine(MakeTheNoise());
    }

    IEnumerator MakeTheNoise()
    {
        makingNoise = true;
        //bush sound
        float distToPlayer = (transform.position - PlayerControl.Instance.transform.position).magnitude;
        if (distToPlayer < Cornfield.Instance.bushSoundPool[0].maxDistance)
        {
            foreach (AudioSource bushSound in Cornfield.Instance.bushSoundPool)
            {
                if (bushSound.isPlaying == false)
                {
                    bushSound.transform.position = transform.position + Vector3.up;
                    bushSound.clip = Cornfield.Instance.bushHitClips[Random.Range(0, Cornfield.Instance.bushHitClips.Length)];
                    bushSound.pitch = Random.Range(0.9f, 1.1f);
                    bushSound.Play();
                    break;
                }
            }
        }
        //animate
        float animTime = 0;
        int animSpeed = Random.Range(5, 10);
        float bendRange = Random.Range(-5f, 5f);
        while (animTime < 1)
        {
            animTime += Time.deltaTime * animSpeed;
            transform.rotation = Quaternion.Euler(animTime * bendRange, transform.eulerAngles.y, animTime * bendRange);
            yield return null;
        }
        while (animTime > 0)
        {
            animTime -= Time.deltaTime * animSpeed;
            transform.rotation = Quaternion.Euler(animTime * bendRange, transform.eulerAngles.y, animTime * bendRange);
            yield return null;
        }
        makingNoise = false;
    }
}
