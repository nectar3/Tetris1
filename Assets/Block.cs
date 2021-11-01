using System.Collections;
using System.Collections.Generic;
using UnityEngine;


[SelectionBase]
public class Block : MonoBehaviour
{

    public GameObject DotsPref;
    public Transform point;

    public float downGapSec = 0.3f;

    GameObject[] dots = new GameObject[4];


    bool isDone = false;

    void Start()
    {
        Init();
        StartCoroutine(MoveCorou());
    }


    void Init()
    {

        dots[0] = Instantiate(DotsPref, Vector2.zero, Quaternion.identity, this.transform);
        dots[0].transform.localPosition = new Vector2(-1, 1);

        dots[1] = Instantiate(DotsPref, Vector2.zero, Quaternion.identity, this.transform);
        dots[1].transform.localPosition = new Vector2(-1, 0);

        dots[2] = Instantiate(DotsPref, Vector2.zero, Quaternion.identity, this.transform);
        dots[2].transform.localPosition = new Vector2(0, 0);

        dots[3] = Instantiate(DotsPref, Vector2.zero, Quaternion.identity, this.transform);
        dots[3].transform.localPosition = new Vector2(1, 0);

    }




    private void Update()
    {
        if (isDone)
            return;

        if (Input.GetKeyDown(KeyCode.UpArrow))
        {
            TurnRight();
        }
        else if (Input.GetKeyDown(KeyCode.LeftArrow))
        {

            if (isItOkToGoLeft())
            {
                EraseGrid();
                transform.position += Vector3.left;
                SynchGrid();
            }
        }
        else if (Input.GetKeyDown(KeyCode.RightArrow))
        {
            if (isItOkToGoRight())
            {
                EraseGrid();
                transform.position += Vector3.right;
                SynchGrid();

            }
        }
    }




    bool CheckSide(Vector3 dir)
    {
        for (int i = 0; i < 4; i++)
        {
            if (dots[i].transform.position.x + dir.x < 0 || dots[i].transform.position.x + dir.x > 9)
                return false;
        }
        return true;
    }

    int GetBottomDotsPosY()
    {
        int minY = int.MaxValue;
        for (int i = 0; i < 4; i++)
            minY = Mathf.Min(minY, (int)dots[i].transform.position.y);
        return minY;
    }


    int GetLeftestDotsPosX()
    {
        int min = int.MaxValue;
        for (int i = 0; i < 4; i++)
            min = Mathf.Min(min, (int)dots[i].transform.position.x);
        return min;
    }

    int GetRightestDotsPosX()
    {
        int max = int.MinValue;
        for (int i = 0; i < 4; i++)
            max = Mathf.Max(max, (int)dots[i].transform.position.x);
        return max;
    }

    bool isItOkToGoLeft()
    {
        var leftX = GetLeftestDotsPosX();
        if (leftX <= 0)
            return false;
        for (int i = 0; i < 4; i++)
        {
            if (Grid.I.IsThereLeftSideDot(dots[i].transform.position))
                return false;
        }
        return true;
    }
    bool isItOkToGoRight()
    {
        var rightx = GetRightestDotsPosX();
        if (rightx >= Grid.I.Size().x - 1)
            return false;

        for (int i = 0; i < 4; i++)
        {
            if (Grid.I.IsThereRightSideDot(dots[i].transform.position))
                return false;
        }
        return true;
    }


    /// <summary>
    /// 아래로 진행해도 되면 true
    /// </summary>
    bool IsItOkToGoDownSide()
    {
        var bottomY = GetBottomDotsPosY();
        if (bottomY > Grid.I.Size().y)
            return true;

        for (int i = 0; i < 4; i++)
        {
            if (Grid.I.IsThereBottomDot(dots[i].transform.position))
                return false;
        }
        return true;
    }


    public void TurnRight()
    {
        EraseGrid();

        for (int i = 0; i < 4; i++)
        {
            var loc = Quaternion.Euler(0, 0, -90) * dots[i].transform.localPosition;
            loc.x = Mathf.RoundToInt(loc.x);
            loc.y = Mathf.RoundToInt(loc.y);
            dots[i].transform.localPosition = loc;
        }
        SynchGrid();

    }


    void SynchGrid()
    {
        //Debug.Log("SynchGrid");

        for (int i = 0; i < 4; i++)
        {
            var pos = dots[i].transform.position;

            if ((pos.x >= 0 && pos.x < Grid.I.Size().x)
                && (pos.y >= 0 && pos.y < Grid.I.Size().y))
            {
                Grid.I.grid[(int)pos.x, (int)pos.y].IsDot = 1;
            }
        }
    }
    void EraseGrid()
    {
        //Debug.Log("EraseGrid");
        for (int i = 0; i < 4; i++)
        {
            var pos = dots[i].transform.position;
            if ((pos.x >= 0 && pos.x < Grid.I.Size().x)
                && (pos.y >= 0 && pos.y < Grid.I.Size().y))
                Grid.I.grid[(int)pos.x, (int)pos.y].IsDot = 0;
        }
    }

    //TODO 먼저 내려가지 말고, 일단 아래쪽 체크하고 내리기

    IEnumerator MoveCorou()
    {
        while (true)
        {
            yield return new WaitForSeconds(downGapSec);
            //Debug.Log("MoveCorou");

            if (IsItOkToGoDownSide() == false)
            {
                isDone = true;
                SetGridDone();
                Manager.I.MakeNewBlock();
                break;
            }

            EraseGrid();
            transform.position = transform.position + new Vector3(0, -1, 0);
            SynchGrid();
        }
    }

    void SetGridDone()
    {
        for (int i = 0; i < 4; i++)
        {
            Grid.I.SetGridDone(dots[i].transform.position);
        }
    }




}
