using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using UnityStandardAssets.ImageEffects;

public class Menu : MonoBehaviour
{
    int graphics = 0;
    public Text gfxStats;
    int music = 1;
    public Text musicStats;

    public CanvasGroup blackscreen;
    public BloomOptimized bloom;

    int screenWidth;
    int screenHeight;

    // Start is called before the first frame update
    void Start()
    {
        DropResolution();

        //load
        if (PlayerPrefs.HasKey("Graphics"))
        {
            graphics = PlayerPrefs.GetInt("Graphics");
        }
        if (PlayerPrefs.HasKey("Music"))
        {
            if (PlayerPrefs.GetInt("Music") == 0)
            {
                ToggleMusic();
            }
        }
        UpdateGraphics();
    }

    void DropResolution()
    {
        if (PlayerPrefs.HasKey("NativeScreenResolution"))
        {
            screenWidth = int.Parse(PlayerPrefs.GetString("NativeScreenResolution").Split('x')[0]);
            screenHeight = int.Parse(PlayerPrefs.GetString("NativeScreenResolution").Split('x')[1]);
        }
        else
        {
            screenWidth = Screen.width;
            screenHeight = Screen.height;
            PlayerPrefs.SetString("NativeScreenResolution", screenWidth + "x" + screenHeight);
        }
        //drop resolution
        int resolutionPercent = 80;
        Screen.SetResolution(screenWidth * resolutionPercent / 100, screenHeight * resolutionPercent / 100, Screen.fullScreen);
    }

    public void QuitTheGame()
    {
        Application.Quit();
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.X))
        {
            PlayerPrefs.DeleteAll();
            Debug.Log("All data deleted.");
        }
        blackscreen.alpha = Mathf.MoveTowards(blackscreen.alpha, 0f, 0.5f * Time.deltaTime);
    }

    public void ToggleMusic()
    {
        if (music == 0)
        {
            music = 1;
            musicStats.text = "MUSIC\nOn";
            GameObject.Find("_Music").GetComponent<AudioSource>().UnPause();
        }
        else
        {
            music = 0;
            musicStats.text = "MUSIC\nOff";
            GameObject.Find("_Music").GetComponent<AudioSource>().Pause();
        }
        //save
        PlayerPrefs.SetInt("Music", music);
    }

    public void ToggleGraphics()
    {
        graphics++;
        if (graphics > 3)
        {
            graphics = 0;
        }
        UpdateGraphics();
        //save
        PlayerPrefs.SetInt("Graphics", graphics);
    }

    public void UpdateGraphics()
    {
        if (graphics == 0)
        {
            QualitySettings.shadowDistance = 0;
            bloom.enabled = false;
            gfxStats.text = "GRAPHICS\nLow";
        }
        if (graphics == 1)
        {
            QualitySettings.shadowDistance = 0;
            bloom.enabled = true;
            gfxStats.text = "GRAPHICS\nMedium";
        }
        if (graphics == 2)
        {
            QualitySettings.shadowDistance = 30;
            QualitySettings.shadowResolution = ShadowResolution.Medium;
            bloom.enabled = true;
            gfxStats.text = "GRAPHICS\nHigh";
        }
        if (graphics == 3)
        {
            QualitySettings.shadowDistance = 30;
            QualitySettings.shadowResolution = ShadowResolution.VeryHigh;
            bloom.enabled = true;
            gfxStats.text = "GRAPHICS\nVery-High";
        }
    }

    public void LoadToLevel(int _lvl)
    {
        SceneManager.LoadScene(_lvl);
    }

    public void OpenURL(string _url)
    {
        Application.OpenURL(_url);
    }
}
