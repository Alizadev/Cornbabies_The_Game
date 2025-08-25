using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.UI;

public class Leaderboard : MonoBehaviour
{
    public static Leaderboard Instance { get; private set; }

    private bool loadingLeaderboard = false;
    public RectTransform leaderboardRect;
    public RectTransform loadingCircle;
    public Text leaderboardStat;
    public GameObject leaderboardNamePrefab;

    public Text rankStat;

    List<GameObject> huntersCache;

    private void Awake()
    {
        Instance = this;
    }

    // Start is called before the first frame update
    void Start()
    {
        huntersCache = new();
    }

    public void ToggleLeaderboard()
    {
        leaderboardRect.gameObject.SetActive(!leaderboardRect.gameObject.activeSelf);
        if (leaderboardRect.gameObject.activeSelf)
        {
            LoadLeaderboard();
        }
    }

    void LoadLeaderboard()
    {
        if (loadingLeaderboard == false)
        {
            loadingLeaderboard = true;
            StartCoroutine(LoadTheLeaderboard());
        }
    }

    IEnumerator LoadTheLeaderboard()
    {
        //clear cache
        foreach (GameObject hunter in huntersCache)
        {
            if (hunter != null)
            {
                Destroy(hunter);
            }
        }
        huntersCache.Clear();
        //request
        WWWForm rankForm = new();
        rankForm.AddField("tophunters", "ok");
        UnityWebRequest rankReq = UnityWebRequest.Post(CornUtility.DATABASE_ADDRESS, rankForm);
        rankReq.SendWebRequest();
        //download result
        loadingCircle.gameObject.SetActive(true);
        while (rankReq.isDone == false)
        {
            loadingCircle.Rotate(-360f * Time.deltaTime * Vector3.forward, Space.Self);
            yield return null;
        }
        loadingCircle.gameObject.SetActive(false);
        //results
        if (rankReq.error == null)
        {
            if (rankReq.downloadHandler.text.Contains("<br>"))
            {
                string[] huntersData = rankReq.downloadHandler.text.Replace("<br>", "\n").Split('\n');
                foreach (string hunter in huntersData)
                {
                    if (hunter.Contains("|"))
                    {
                        GameObject userInfo = Instantiate(leaderboardNamePrefab, leaderboardNamePrefab.transform.parent);
                        userInfo.SetActive(true);
                        userInfo.transform.GetChild(0).GetComponent<Text>().text = hunter;
                        //cache
                        huntersCache.Add(userInfo);
                    }
                }
            }
        }
        //error
        else
        {
            leaderboardStat.gameObject.SetActive(true);
            leaderboardStat.text = rankReq.error;
        }
        rankReq.Dispose();
        loadingLeaderboard = false;
    }

    public IEnumerator GetMyRank()
    {
        WWWForm rankForm = new();
        rankForm.AddField("updatescore", PlayerPrefs.GetInt(CornUtility.SCORE_PREFS));
        rankForm.AddField("deviceid", SystemInfo.deviceUniqueIdentifier);
        UnityWebRequest rankReq = UnityWebRequest.Post(CornUtility.DATABASE_ADDRESS, rankForm);
        yield return rankReq.SendWebRequest();
        if (rankReq.error == null)
        {
            string yourResult = rankReq.downloadHandler.text;
            Debug.Log("[GameManager] rank result: " + yourResult);
            if (yourResult.Contains("#"))
            {
                //in game
                if (GameManager.Instance)
                {
                    for (int i = 0; i <= yourResult.Length; i++)
                    {
                        rankStat.text = yourResult.Substring(0, i);
                        yield return new WaitForSeconds(0.1f);
                    }
                }
                //in menu
                else
                {
                    rankStat.text = "#" + yourResult.Split('#')[1];
                }
            }
            else
            {
                //we got internet at least
                rankStat.text = " ";
            }
        }
        else
        {
            Debug.Log("[GameManager] failed to get rank: " + rankReq.error);
        }
        rankReq.Dispose();
    }
}
