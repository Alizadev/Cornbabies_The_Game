using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class SplashScreen : MonoBehaviour
{
    public GameObject music;
    public CanvasGroup splash;
    float splashRoutine = 0;
    // Start is called before the first frame update
    void Start()
    {
        if (PlayerPrefs.HasKey("Music"))
        {
            if (PlayerPrefs.GetInt("Music") == 0)
            {
                GameObject.Find("_Music").GetComponent<AudioSource>().Pause();
            }
        }
        DontDestroyOnLoad(music);
        StartCoroutine(SplashIt());
    }

    // Update is called once per frame
    void Update()
    {
        splash.alpha = Mathf.MoveTowards(splash.alpha, splashRoutine, 0.5f * Time.deltaTime);   
    }

    IEnumerator SplashIt()
    {
        splashRoutine = 1;
        yield return new WaitForSeconds(3);
        splashRoutine = 0;
        yield return new WaitForSeconds(2);
        SceneManager.LoadScene(1);
    }
}
