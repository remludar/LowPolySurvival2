using UnityEngine;

public class Transparency : MonoBehaviour
{
    bool isTransparent = false;
    float fadeSpeed = 5.0f;
    float fadeInThreshold = 0.9f;
    float fadeMin = 0.0f;
    float fadeMax = 1.0f;

    void Update()
    {
        UpdateTransparencies();
    }

    void UpdateTransparencies()
    {
        var meshRenderers = GetComponentsInChildren<MeshRenderer>();
        if (isTransparent)
        {
            foreach (MeshRenderer m in meshRenderers)
            {
                var material = m.material;
                StandardShaderUtils.ChangeRenderMode(material, StandardShaderUtils.BlendMode.Transparent);
                material.SetColor("_Color", Color.Lerp(material.color, new Color(material.color.r, material.color.g, material.color.b, fadeMin), fadeSpeed * Time.deltaTime));
            }
        }
        else
        {
            foreach (MeshRenderer m in meshRenderers)
            {
                var material = m.material;
                material.SetColor("_Color", Color.Lerp(material.color, new Color(material.color.r, material.color.g, material.color.b, fadeMax), fadeSpeed * Time.deltaTime));
                if (material.color.a > fadeInThreshold)
                    StandardShaderUtils.ChangeRenderMode(material, StandardShaderUtils.BlendMode.Opaque);

            }
        }


    }

    public void MakeTransparent(bool b)
    {
        isTransparent = b;
    }
}