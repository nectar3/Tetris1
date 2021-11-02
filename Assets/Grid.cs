using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class Grid
{
    public Dot[,] grid;


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

}


// grid ���� �׸� ��ĭ( 
public class Dot
{
    public int IsDot = 0;

    public Dot()
    {
    }

}


