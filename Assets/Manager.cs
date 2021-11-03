using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using TMPro;



public class Manager : MonoBehaviour
{

    public List<GameObject> BlockPref;
    public Transform spawnPoint;
    public GameObject dotsParent;

    public TextMeshProUGUI text_score;
    public TextMeshProUGUI textGrid;
    public GameObject panelGameOver;
    public Grid grid;

    public float blockDownSec = 0.4f; // 블럭 내려오는 시간간격

    int score = 0;

    void Start()
    {
        var grid = Grid.I; // init

        MakeNewBlock();
    }

    private static Manager instance = null;
    void Awake()
    {
        if (null == instance)
        {
            instance = this;
            DontDestroyOnLoad(this.gameObject);
        }
        else
            Destroy(this.gameObject);
    }
    public static Manager I
    {
        get
        {
            if (null == instance)
                return null;
            return instance;
        }
    }

    public void CurBlockPutted()
    {
        var completedLinesY = Grid.I.GetCompletedLineYPos();

        if (Grid.I.CheckDie())
        {
            Time.timeScale = 0;
            panelGameOver.SetActive(true);
            return;
        }
        if (completedLinesY.Count > 0)
        {
            score = score + completedLinesY.Count;
            text_score.SetText("Score: " + score);

            StartCoroutine(Grid.I.BlinkLines(completedLinesY));
        }
        else
            MakeNewBlock();
    }


    int c = 0;
    public void MakeNewBlock()
    {
        var block = Instantiate(BlockPref[Random.Range(0, BlockPref.Count)], spawnPoint.position, Quaternion.identity);
        block.name = "block" + c++;
        block.GetComponent<Block>().dotsParent = dotsParent;
        block.GetComponent<Block>().downGapSec = blockDownSec;
    }

    void OnDrawGizmos()
    {
        var style = new GUIStyle();
        style.fontSize = 15;
        style.normal.textColor = Color.white;
        int offset_up = 15;
        if (Grid.I != null)
            Handles.Label(transform.position + Vector3.up * offset_up, Grid.I.Tostring(), style);
    }


    //public Vector2 rotate(Vector2 v, float delta)
    //{
    //    return new Vector2(
    //        v.x * Mathf.Cos(delta) - v.y * Mathf.Sin(delta),
    //        v.x * Mathf.Sin(delta) + v.y * Mathf.Cos(delta)
    //    );
    //}

}
