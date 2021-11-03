using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Dots : MonoBehaviour
{
    SpriteRenderer render;


    private void Start()
    {
        render = GetComponent<SpriteRenderer>();

    }

    public void SetColor(Color col)
    {
        render.material.color = col;
    }

    public void DestroySelf()
    {
        Destroy(gameObject);
    }


}
