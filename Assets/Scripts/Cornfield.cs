using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Cornfield : MonoBehaviour
{
    public static Cornfield Instance { get; private set; }

    public int cornAmount = 5;
    [Range(1, 20)]
    public int cornScale = 2;

    public GameObject cornPrefab;
    public Transform _player;

    GameObject[] cornPool;
    Vector3[] generatedPoint;
    bool firstInit = false;

    [HideInInspector]
    public List<CornPlant> allCorns;

    Vector3 _lastPos;

    public int score;
    public Text scoreTxt;

    public AudioSource[] bushSoundPool;
    public AudioClip[] bushHitClips;

    private void Awake()
    {
        Instance = this;
    }

    // Start is called before the first frame update
    void Start()
    {
        allCorns = new List<CornPlant>();
        cornPool = new GameObject[cornAmount * cornAmount];
        //create corn
        for (int i = 0; i < cornAmount * cornAmount; i++)
        {
            //clone
            cornPool[i] = Instantiate(cornPrefab, Vector3.zero, Quaternion.identity);
            foreach (CornPlant cp in cornPool[i].GetComponentsInChildren<CornPlant>())
            {
                allCorns.Add(cp);
            }
        }
        Debug.Log("all corns: " + allCorns.Count);
        Destroy(cornPrefab);
    }
    // Update is called once per frame
    void Update()
    {
        scoreTxt.text = score.ToString();
        //generate new seed
        if (Input.GetKeyDown(KeyCode.E))
        {
            GenerateSeed();
        }
        if ((_player.position - _lastPos).magnitude > 0.25f)
        {
            _lastPos = _player.position;
            GenerateSeed();
            UpdateCornPos();
            Debug.DrawRay(_player.transform.position, Vector3.forward, Color.red, 2f);
        }
    }

    void GenerateSeed()
    {
        //generate
        int pointAmount = cornAmount * 10;
        Vector3[] generatedTempPoint = new Vector3[pointAmount * pointAmount];
        int xPos = 0;
        int zPos = 0;
        for (int i = 0; i < generatedTempPoint.Length; i++)
        {
            Vector3 generatedPos = new Vector3(
                Mathf.Round(_player.transform.position.x / cornScale) * cornScale,
                0,
                Mathf.Round(_player.transform.position.z / cornScale) * cornScale);
            //adjust
            generatedPos.x += xPos;
            generatedPos.z += zPos;
            //offset center
            generatedPos.x -= cornScale * (pointAmount / 2);
            generatedPos.z -= cornScale * (pointAmount / 2);
            //fixed height
            generatedPos.y = Random.Range(0, 180);
            //grid
            xPos += cornScale;
            if (xPos == pointAmount * cornScale)
            {
                zPos += cornScale;
                xPos = 0;
            }
            generatedTempPoint[i] = generatedPos;
        }
        //get random
        if (firstInit == false)
        {
            generatedPoint = new Vector3[500];
        }
        for (int i = 0; i < generatedPoint.Length; i++)
        {
            int rnd = Random.Range(0, generatedTempPoint.Length);
            if (firstInit == false)
            {
                generatedPoint[i] = generatedTempPoint[rnd];
                Debug.DrawRay(new Vector3(generatedPoint[i].x, 0, generatedPoint[i].z), Vector3.up * 5, Color.green, 3);
            }
            else
            {
                //only re-new far points
                float distToNewPoint = (_player.position - generatedTempPoint[rnd]).magnitude;
                float distToGrid = (_player.position - generatedPoint[i]).magnitude;
                if (distToNewPoint > 200 && distToGrid > 200)
                {
                    generatedPoint[i] = generatedTempPoint[rnd];
                    Debug.DrawRay(new Vector3(generatedPoint[i].x, 0, generatedPoint[i].z), Vector3.up * 5, Color.red, 0.25f);
                }
            }
        }
        firstInit = true;
    }

    void UpdateCornPos()
    {
        int xPos = 0;
        int zPos = 0;
        for (int i = 0; i < cornPool.Length; i++)
        {
            Vector3 generatedPos = new Vector3(
                Mathf.Round(_player.transform.position.x / cornScale) * cornScale,
                0,
                Mathf.Round(_player.transform.position.z / cornScale) * cornScale);
            //adjust
            generatedPos.x += xPos;
            generatedPos.z += zPos;
            //offset center
            generatedPos.x -= cornScale * (cornAmount / 2);
            generatedPos.z -= cornScale * (cornAmount / 2);
            //fixed height
            generatedPos.y = 0;
            //pos
            cornPool[i].transform.position = generatedPos;
            //grid
            xPos += cornScale;
            if (xPos == cornAmount * cornScale)
            {
                zPos += cornScale;
                xPos = 0;
            }

            //personate
            cornPool[i].transform.rotation = Quaternion.Euler(0, 0, 0);
            foreach (Vector3 v3 in generatedPoint)
            {
                Vector3 thePoint = v3;
                thePoint.y = 0;
                if (cornPool[i].transform.position == thePoint)
                {
                    cornPool[i].transform.rotation = Quaternion.Euler(0, v3.y, 0);
                }
            }
        }
    }

    public void CheckCornDist(Vector3 _target)
    {
        foreach (CornPlant cp in allCorns)
        {
            if ((cp.transform.position - _target).sqrMagnitude < 2)
            {
                cp.MakeNoise();
            }
        }
    }
}
