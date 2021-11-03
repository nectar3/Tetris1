using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class Grid
{
    public Dot[,] grid;

    public int Y_ceiling = 20; // ���� �� ����

    public Grid()
    {
        grid = new Dot[10, 25]; // ���� �� ũ��� 10 by 20 ������ �ణ ������ ��ϻ����ϹǷ� �����ְ�
        Debug.Log("grid constructor size: " + Size);

        for (int i = 0; i < Size.y; i++)
        {
            for (int j = 0; j < Size.x; j++)
            {
                grid[j, i] = new Dot();
            }
        }
    }

    private static Grid instance = null;
    public static Grid I
    {
        get
        {
            if (instance == null)
                instance = new Grid();
            return instance;
        }
    }

    public Vector2Int Size => new Vector2Int(grid.GetLength(0), grid.GetLength(1));


    public void SetGridDone(Vector3 pos)
    {
        grid[(int)pos.x, (int)pos.y].IsDot = 2;
    }


    public bool CanPutDot(Vector3 pos)
    {
        if ((int)pos.y < 0 || (int)pos.y >= Size.y)
            return false;
        if ((int)pos.x < 0 || (int)pos.x >= Size.x)
            return false;
        return grid[(int)pos.x, (int)pos.y].IsDot != 2;
    }



    public string Tostring()
    {
        string str = "";
        var size = Size;
        for (int i = size.y - 1; i >= 0; i--)
        {
            string line = "";
            for (int j = 0; j < size.x; j++)
                line += grid[j, i].IsDot + " ";
            str += line + "\n";
        }
        return str;
    }

    public bool CheckDie()
    {
        for (int k = 0; k < Size.x; k++)
        {
            if (grid[k, Y_ceiling - 1].IsDot == 2)
                return true;
        }
        return false;
    }


    public IEnumerator BlinkLines(List<int> linesY)
    {
        int c = 0;
        while (c++ < 8)
        {
            foreach (var y in linesY)
            {
                SetLineColor(y, c % 2 == 1 ? new Color(500 / 255f, 500 / 255f, 500 / 255f) : Color.white); // 0~1 ���ε� 2�� ������ �㿸�Ե�.
            }
            yield return new WaitForSeconds(0.05f);
        }
        foreach (var y in linesY)
        {
            DeleteLineAndRearrange(y);
        }
        Manager.I.MakeNewBlock();
    }


    // �����̽��� �������� ���� ó�� �ε����� y ��ġ



    public List<int> GetCompletedLineYPos()
    {
        List<int> comple = new List<int>();
        for (int i = Size.y - 1; i >= 0; i--) // �� ���������� �����ϱ����� ��������
        {
            var isCompleted = IsThisLineComplete(i);
            if (isCompleted)
                comple.Add(i);
        }
        return comple;
    }

    bool IsThisLineComplete(int y)
    {
        bool isCompleted = true;
        for (int k = 0; k < Size.x; k++)
        {
            if (grid[k, y].IsDot != 2)
            {
                isCompleted = false;
                break;
            }
        }
        return isCompleted;
    }


    void DeleteLineAndRearrange(int y)
    {
        for (int k = 0; k < Size.x; k++)
        {
            grid[k, y].IsDot = 0;
            if (grid[k, y].go != null)
                grid[k, y].go.GetComponent<Dots>().DestroySelf();
            grid[k, y].go = null;
        }

        for (int i = y + 1; i < Size.y; i++) // ������ �� ���� dots ���پ� �����°���
        {
            for (int k = 0; k < Size.x; k++)
            {
                grid[k, i - 1].go = grid[k, i].go;
                grid[k, i].go = null;
                grid[k, i - 1].IsDot = grid[k, i].IsDot;
                grid[k, i].IsDot = 0;

                if (grid[k, i - 1].go)
                    grid[k, i - 1].go.transform.position += Vector3.down;
            }
        }
    }


    void SetLineColor(int y, Color col)
    {
        for (int i = 0; i < Size.x; i++)
        {
            if (grid[i, y].go)
                grid[i, y].go.GetComponent<Dots>().SetColor(col);
        }
    }
    public void SetDotGo(Vector3 pos, GameObject go)
    {
        if (grid[(int)pos.x, (int)pos.y].IsDot == 2)
            grid[(int)pos.x, (int)pos.y].go = go;
    }
}



// grid ���� �׸� ��ĭ( 
public class Dot
{
    public int IsDot = 0;
    public GameObject go;

    public Dot()
    {
    }

}


