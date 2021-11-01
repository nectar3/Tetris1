using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using TMPro;



public class Manager : MonoBehaviour
{

    public GameObject BlockPref;
    public Transform spawnPoint;

    public TextMeshProUGUI textGrid;
    public Grid grid;

    void Start()
    {
        var grid = Grid.I; // init

        var block = Instantiate(BlockPref, spawnPoint.position, Quaternion.identity);


        StartCoroutine(ShowGridText());
    }
    IEnumerator ShowGridText()
    {
        while (true)
        {
            yield return new WaitForSeconds(0.1f);
            var str = Grid.I.Tostring();
            textGrid.SetText(str);
        }
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
            {
                return null;
            }
            return instance;
        }
    }

    int c = 0;
    public void MakeNewBlock()
    {
        var block = Instantiate(BlockPref, spawnPoint.position, Quaternion.identity);
        block.name = "block" + c++;

    }

    void OnDrawGizmos()
    {
        var style = new GUIStyle();
        style.fontSize = 15;
        style.normal.textColor = Color.white;
        if (Grid.I != null)
            Handles.Label(transform.position + Vector3.up * 6, Grid.I.Tostring(), style);
    }


    //public Vector2 rotate(Vector2 v, float delta)
    //{
    //    return new Vector2(
    //        v.x * Mathf.Cos(delta) - v.y * Mathf.Sin(delta),
    //        v.x * Mathf.Sin(delta) + v.y * Mathf.Cos(delta)
    //    );
    //}

}
