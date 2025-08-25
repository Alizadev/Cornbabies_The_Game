using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using UnityEngine.Networking;

public class GameManager : MonoBehaviour
{

    public static GameManager Instance { get; private set; }


    public int score;
    public Text scoreTxt;

    public AnimationCurve noiseAnim;
    public GameObject deadPanel;
    public CanvasGroup deadTitleCanvas;
    RectTransform deadTitleRect;

    public CanvasGroup killsCanvas;
    RectTransform killsRect;

    public Image cornBabyImg;
    public Text cornbabyScoreText;

    bool showKills = false;

    public Button quitButton;

    private void Awake()
    {
        Instance = this;
    }

    // Start is called before the first frame update
    void Start()
    {
        if (deadTitleCanvas != null)
        {
            noiseAnim.postWrapMode = WrapMode.Loop;
            deadTitleRect = deadTitleCanvas.GetComponent<RectTransform>();
            deadTitleRect.anchoredPosition = new Vector2(0, 0);
            killsRect = killsCanvas.GetComponent<RectTransform>();
        }

        //quit button
        quitButton.onClick.AddListener(delegate {
            QuitGame();
        });
    }

    // Update is called once per frame
    void Update()
    {
        //show dead screen
        if (PlayerControl.Instance.dead)
        {
            quitButton.image.color = Color.gray * (0.25f + Mathf.PingPong(Time.time * 0.5f, 0.35f));
            if (deadPanel.activeSelf == false)
            {
                deadPanel.SetActive(true);
                StartCoroutine(DeadRoutine());
            }
        }
        if (Leaderboard.Instance == null)
        {
            return;
        }
        //fade dead when show kills
        float maxHeight = 60;
        float deadTitleLeftMove = Leaderboard.Instance.leaderboardRect.gameObject.activeSelf ? -100 : 0;
        deadTitleRect.anchoredPosition = Vector2.MoveTowards(deadTitleRect.anchoredPosition, new Vector2(deadTitleLeftMove, showKills ? maxHeight : 0), 100f * Time.deltaTime);
        deadTitleCanvas.alpha = Mathf.Clamp(1f - (deadTitleRect.anchoredPosition.y / maxHeight), 0.2f + noiseAnim.Evaluate(Time.time * 0.1f), 1f);
        //show kills
        killsCanvas.alpha = deadTitleRect.anchoredPosition.y / maxHeight;
        killsRect.anchoredPosition = new Vector2(deadTitleRect.anchoredPosition.x, -300 + (killsCanvas.alpha * 200));
        //show leaderboard
        Leaderboard.Instance.leaderboardRect.localScale = new Vector3(deadTitleRect.anchoredPosition.x * -1f / 100f, 1f, 1f);
    }

    IEnumerator DeadRoutine()
    {
        //played enough to show ads
        PlayerPrefs.SetString(CornUtility.ADS_PREFS, "true");
        yield return new WaitForSeconds(1f);
        showKills = true;
        //wait for kill canvas
        while (killsCanvas.alpha != 1f)
        {
            yield return null;
        }
        //animate score
        int maxAmount = Mathf.Clamp(7, 0, score);
        for (int i = 0; i <= score; i++)
        {
            if (score == 0)
            {
                cornBabyImg.color *= 0.5f;
                break;
            }
            float percentage = (float)i / score;
            if (i < maxAmount)
            {
                yield return new WaitForSeconds(0.05f);
            }
            else
            {
                yield return new WaitForSeconds(0.05f * (1f - percentage));
            }
            cornbabyScoreText.text = "x" + i;
            //skip making cornbaby img
            if (i < 1 || i > maxAmount)
            {
                continue;
            }
            Image cbImg = Instantiate(cornBabyImg, cornBabyImg.transform.parent);
            cbImg.GetComponent<RectTransform>().anchoredPosition += 20f * i * Vector2.left;
            cbImg.transform.SetAsFirstSibling();
            Color cbImgCol = cbImg.color;
            cbImgCol *= (1f - ((float)i / maxAmount)) * 0.5f;
            cbImgCol.a = 1f;
            cbImg.color = cbImgCol;
        }
        //save your best score
        if (score > PlayerPrefs.GetInt(CornUtility.SCORE_PREFS))
        {
            PlayerPrefs.SetInt(CornUtility.SCORE_PREFS, score);
            Debug.Log("[GameManager] saved your best score: " + score);
        }
        else
        {
            Debug.Log("[GameManager] your score: " + score);
        }

        //get rank
        StartCoroutine(Leaderboard.Instance.GetMyRank());
        yield return new WaitForSeconds(1f);
        while (Leaderboard.Instance.rankStat.text.Length == 0)
        {
            yield return null;
        }
        //show leaderboard
        Leaderboard.Instance.ToggleLeaderboard();
    }

    public void AddScore(int _amount)
    {
        score += _amount;
        scoreTxt.text = score.ToString();
    }


    void QuitGame()
    {
        if (PlayerControl.Instance.dead)
        {
            SceneManager.LoadScene(2);
        }
        else
        {
            SceneManager.LoadScene(1);
        }
    }
}
