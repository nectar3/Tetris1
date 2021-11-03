using System.Collections;
using System.Collections.Generic;
using UnityEngine;


[SelectionBase]
public class Block : MonoBehaviour
{

    public GameObject DotsPref;

    [HideInInspector]
    public float downGapSec = 0.3f;
    public bool Is360DegreeFlip = true; // 4방향 회전인지 일자블럭처럼 90도 회전 후 원위치인지
    public bool PreventTurn = false; // 네모블럭은 회전 금지

    [HideInInspector]
    public GameObject dotsParent;

    GameObject[] dots = new GameObject[4];

    [SerializeField]
    public Vector2[] DotsLocalPosition = new Vector2[4];

    bool isDone = false;
    bool isTurned = false; // 90도 원상복귀할 경우

    void Start()
    {
        Init();
        StartCoroutine(MoveCorou());
    }

    void Init()
    {
        // localposition이 (0, 0)인 dot이 회전의 중심축이 됨
        for (int i = 0; i < DotsLocalPosition.Length; i++)
        {
            dots[i] = Instantiate(DotsPref, Vector2.zero, Quaternion.identity, this.transform);
            dots[i].transform.localPosition = new Vector2(DotsLocalPosition[i].x, DotsLocalPosition[i].y);
        }

    }


    public void TurnRight()
    {
        var Xoffset = GetTurnRightAndMoveHorizontalXOffset();
        if (Xoffset != null)
        {
            var turnedLocal = GetTurnRightLocalPosition();

            EraseGrid();
            for (int i = 0; i < 4; i++)
            {
                dots[i].transform.localPosition = turnedLocal[i];
            }
            transform.position = transform.position + new Vector3((float)Xoffset, 0, 0);

            SynchGrid();

            isTurned = !isTurned; // 토글
        }
    }

    List<Vector3> GetTurnRightLocalPosition()
    {
        List<Vector3> turned = new List<Vector3>();
        for (int i = 0; i < 4; i++)
        {
            var loc = Quaternion.Euler(0, 0, (!Is360DegreeFlip && isTurned) ? 90 : -90) * dots[i].transform.localPosition;
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

    // 횡이동은 block 자체에서 적용
    // 회전 후 되는 포지션 나올때까지 횡이동 5번(Xoffset) 시행후 되는 offset리턴. 회전 자체가 안되면 null
    int? GetTurnRightAndMoveHorizontalXOffset()
    {
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
                return Xoffset[i];
        }
        return null;
    }

    private void Update()
    {
        if (isDone)
            return;

        if (Input.GetKeyDown(KeyCode.UpArrow))
        {
            if (!PreventTurn)
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
        else if (Input.GetKeyDown(KeyCode.Space)) //TODO:
        {
            var y_offset = GetFirstHitYPos();

            EraseGrid();
            transform.position -= new Vector3(0, y_offset, 0);
            SynchGrid();

            isDone = true;
            SetGridDone();
            Manager.I.CurBlockPutted();
        }
        else if (Input.GetKeyDown(KeyCode.DownArrow))
        {
            BlockDown();
        }
    }

    // 스페이스바 눌렀을때: 블럭 현재위치부터 한칸씩 아래로 내려보다가, 막히는 지점의 바로 위가 놓아질 부분이다(y_offset - 1)
    public int GetFirstHitYPos()
    {
        int y_offset = 1;
        for (; ; )
        {
            for (int i = 0; i < 4; i++)
            {
                if (!Grid.I.CanPutDot(transform.position + dots[i].transform.localPosition - new Vector3(0, y_offset, 0)))
                    return y_offset - 1;
            }
            y_offset++;
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



    IEnumerator MoveCorou()
    {
        while (true)
        {
            yield return new WaitForSeconds(downGapSec);

            if (BlockDown() == false)
                break;
        }
    }

    bool BlockDown()
    {
        if (IsItOkToGoDownSide() == false)
        {
            isDone = true;
            SetGridDone();
            Manager.I.CurBlockPutted();
            return false;
        }
        EraseGrid();
        transform.position = transform.position + new Vector3(0, -1, 0);
        SynchGrid();
        return true;
    }


    void SetGridDone()
    {
        for (int i = 0; i < 4; i++)
        {
            Grid.I.SetGridDone(dots[i].transform.position);
            Grid.I.SetDotGameObject(dots[i].transform.position, dots[i]);

            dots[i].transform.SetParent(dotsParent.transform); // 놓을때 dots go만 남기
        }
        Destroy(gameObject);
    }


}
