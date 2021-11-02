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

    [SerializeField]
    public Vector2[] DotsLocalPosition = new Vector2[4];

    bool isDone = false;

    void Start()
    {
        Init();
        StartCoroutine(MoveCorou());
    }


    void Init()
    {
        for (int i = 0; i < DotsLocalPosition.Length; i++)
        {
            dots[i] = Instantiate(DotsPref, Vector2.zero, Quaternion.identity, this.transform);
            dots[i].transform.localPosition = new Vector2(DotsLocalPosition[i].x, DotsLocalPosition[i].y);
        }


        //dots[0] = Instantiate(DotsPref, Vector2.zero, Quaternion.identity, this.transform);
        //dots[0].transform.localPosition = new Vector2(-1, 1);

        //dots[1] = Instantiate(DotsPref, Vector2.zero, Quaternion.identity, this.transform);
        //dots[1].transform.localPosition = new Vector2(-1, 0);

        //dots[2] = Instantiate(DotsPref, Vector2.zero, Quaternion.identity, this.transform);
        //dots[2].transform.localPosition = new Vector2(0, 0);

        //dots[3] = Instantiate(DotsPref, Vector2.zero, Quaternion.identity, this.transform);
        //dots[3].transform.localPosition = new Vector2(1, 0);

    }
    public void TurnRight()
    {
        var canLocal = GetTurnRightAndMoveHorizontalLocalPosition();
        if (canLocal.Count == 4)
        {
            EraseGrid();
            for (int i = 0; i < 4; i++)
            {
                dots[i].transform.localPosition = canLocal[i];

            }
            Debug.Log(dots[0].transform.localPosition);

            SynchGrid();
        }
    }

    List<Vector3> GetTurnRightLocalPosition()
    {
        List<Vector3> turned = new List<Vector3>();
        for (int i = 0; i < 4; i++)
        {
            var loc = Quaternion.Euler(0, 0, -90) * dots[i].transform.localPosition;
            loc.x = Mathf.RoundToInt(loc.x);
            loc.y = Mathf.RoundToInt(loc.y);
            turned.Add(loc);
        }
        return turned;
    }
    bool isItOkTurnRight()
    {
        var locs = GetTurnRightLocalPosition();
        for (int i = 0; i < 4; i++)
        {
            if (!Grid.I.CanPutDot(dots[i].transform.position + locs[i]))
                return false;
        }
        return true;
    }

    // 회전 후 되는 포지션 나올때까지 횡이동 4번 시행 후 가져오기
    List<Vector3> GetTurnRightAndMoveHorizontalLocalPosition()
    {
        List<Vector3> canPutlocs = new List<Vector3>();

        int[] Xoffset = { 0, 1, -1, 2, -2 };
        var turnedLocal = GetTurnRightLocalPosition();
        for (int i = 0; i < Xoffset.Length; i++)
        {
            bool thisPosIsGood = true;
            for (int k = 0; k < 4; k++)
            {
                if (!Grid.I.CanPutDot(transform.position + turnedLocal[k] + new Vector3(Xoffset[i], 0, 0)))
                {
                    thisPosIsGood = false;
                    break;
                }
            }
            if (thisPosIsGood)
            {
                for (int k = 0; k < 4; k++)
                    canPutlocs.Add(turnedLocal[k] + new Vector3(Xoffset[i], 0, 0));
                break;
            }
        }
        return canPutlocs;
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



    bool isItOkToGoLeft()
    {
        var leftX = GetLeftestDotsPosX();
        for (int i = 0; i < 4; i++)
        {
            if (!Grid.I.CanPutDot(dots[i].transform.position + Vector3.left))
                return false;
        }
        return true;
    }
    bool isItOkToGoRight()
    {
        var rightx = GetRightestDotsPosX();
        for (int i = 0; i < 4; i++)
        {
            if (!Grid.I.CanPutDot(dots[i].transform.position + Vector3.right))
                return false;
        }
        return true;
    }

    // 아래로 진행해도 되면 true
    bool IsItOkToGoDownSide()
    {
        var bottomY = GetBottomDotsPosY();
        for (int i = 0; i < 4; i++)
        {
            if (!Grid.I.CanPutDot(dots[i].transform.position + Vector3.down))
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

    void SynchGrid()
    {
        for (int i = 0; i < 4; i++)
        {
            var pos = dots[i].transform.position;
            Grid.I.grid[(int)pos.x, (int)pos.y].IsDot = 1;
        }
    }
    void EraseGrid()
    {
        for (int i = 0; i < 4; i++)
        {
            var pos = dots[i].transform.position;
            Grid.I.grid[(int)pos.x, (int)pos.y].IsDot = 0;
        }
    }

    // TODO 회전시 밀리는 기능 


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
