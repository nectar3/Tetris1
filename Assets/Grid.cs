using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class Grid
{
    public Section[,] grid;


    public Grid()
    {
        grid = new Section[10, 20];
        Debug.Log("grid constructor size: " + Size());

        for (int i = 0; i < Size().y; i++)
        {
            for (int j = 0; j < Size().x; j++)
            {
                grid[j, i] = new Section();
            }
        }

    }




    private static Grid instance = null;
    public static Grid I
    {
        get
        {
            if (instance == null)
            {
                instance = new Grid();
            }
            return instance;
        }
    }

    public Vector2Int Size()
    {
        return new Vector2Int(grid.GetLength(0), grid.GetLength(1));
    }



    public void SetGridDone(Vector3 pos)
    {
        grid[(int)pos.x, (int)pos.y].IsDot = 2;
    }

    public bool IsThereBottomDot(Vector3 pos)
    {
        if ((int)pos.y <= 0)
            return true;
        if ((int)pos.y - 1 >= Size().y - 1)
            return false;
        return grid[(int)pos.x, (int)pos.y - 1].IsDot == 2;
    }

    public bool IsThereLeftSideDot(Vector3 pos)
    {
        if ((int)pos.y - 1 >= Size().y - 1)
            return false;
        if ((int)pos.x <= 0)
            return true;
        return grid[(int)pos.x - 1, (int)pos.y].IsDot == 2;
    }
    public bool IsThereRightSideDot(Vector3 pos)
    {
        //Debug.Log("IsThereRightSideDot: " + pos);

        if ((int)pos.y - 1 >= Size().y - 1)
            return false;
        if ((int)pos.x >= Size().x - 1)
            return true;
        return grid[(int)pos.x + 1, (int)pos.y].IsDot == 2;
    }

    public string Tostring()
    {
        string str = "";
        var size = Size();
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


// grid 상의 네모 한칸( 
public class Section
{
    public int IsDot = 0;

    public Section()
    {
    }

}


