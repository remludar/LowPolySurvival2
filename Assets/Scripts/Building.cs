using UnityEngine;

public class Building : MonoBehaviour
{
    public bool isPlayerInside = false;
    void OnTriggerEnter(Collider other)
    {
        if (other.name == "Player")
        {
            isPlayerInside = true;
            gameObject.GetComponent<Transparency>().MakeTransparent(true);
        } 
    }

    void OnTriggerExit(Collider other)
    {
        if (other.name == "Player")
        {
            isPlayerInside = false;
            gameObject.GetComponent<Transparency>().MakeTransparent(false);
        }
    }
}