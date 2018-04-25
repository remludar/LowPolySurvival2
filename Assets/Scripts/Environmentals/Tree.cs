using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Tree : MonoBehaviour
{

    public void MakeTransparent(bool b)
    {
        var meshRenderers = GetComponentsInChildren<MeshRenderer>();
        if (b)
        {
            foreach(MeshRenderer m in meshRenderers)
            {
                Debug.Log(m.name);
                var material = m.material;
                material.color = new Color(material.color.r, material.color.g, material.color.b, 0.0f);
            }
                
        }
        else
        {
            foreach (MeshRenderer m in meshRenderers)
            {
                var material = m.material;
                material.color = new Color(material.color.r, material.color.g, material.color.b, 1.0f);
            }
        }
    }
}