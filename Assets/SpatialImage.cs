using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpatialImage : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {

    }

    public void ApplyImage(Texture2D texture)
    {
        Renderer renderer = GetComponent<Renderer>();

        if (texture != null)
        {
            renderer.material.mainTexture = texture;
        }
        else
        {
            Debug.LogWarning("请在 Inspector 中分配一个 Texture2D 图片！");
        }
    }
}