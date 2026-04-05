using UnityEngine;
using UnityEngine.UI;

public class HealthBar : MonoBehaviour
{

    public Texture[] textures;

    RawImage rawImage;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        this.rawImage = GetComponent<RawImage>();
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
