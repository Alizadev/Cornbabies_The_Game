using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class SplashScreen : MonoBehaviour
{
    public GameObject music;
    public CanvasGroup termsCanvas;

    bool termsAccepted = false;
    // Start is called before the first frame update
    void Start()
    {
        //frame rate
        Application.targetFrameRate = 60;

        //music
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

    IEnumerator SplashIt()
    {
        if (termsAccepted)
        {

        }
        else
        {
            Text pressToContinue = termsCanvas.transform.GetChild(0).GetComponent<Text>();
            Color pressCol = pressToContinue.color;
            //show 
            while (termsAccepted == false)
            {
                //
                termsCanvas.alpha = Mathf.MoveTowards(termsCanvas.alpha, 1f, 5f * Time.deltaTime);
                //text pulse
                pressToContinue.color = new Color(pressCol.r, pressCol.g, pressCol.b, Mathf.PingPong(Time.time * 2f, 1.5f));                
                yield return null;
            }
            //light up
            pressToContinue.color = pressCol;
            //hide
            while (termsCanvas.alpha > 0)
            {
                termsCanvas.alpha = Mathf.MoveTowards(termsCanvas.alpha, 0f, 2f * Time.deltaTime);
                yield return null;
            }
        }
        yield return new WaitForSeconds(1f);
        SceneManager.LoadScene(1);
    }

    public void AcceptTerms()
    {
        if (termsCanvas.alpha < 1f)
        {
            return;
        }
        termsAccepted = true;
    }

    public void OpenPrivacyPolicy()
    {
        Application.OpenURL(CornUtility.POLICY_ADDRESS);
    }

    private void OnGUI()
    {
        GUI.color = Color.gray;
        GUILayout.BeginArea(new Rect(20, Screen.height - 20, Screen.width, 128));
        GUILayout.Label("v" + Application.version);
        GUILayout.EndArea();
    }
}
