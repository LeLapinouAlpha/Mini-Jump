using NUnit.Framework;
using System;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;
using static UnityEngine.Rendering.DebugUI;

public class HealthBar : MonoBehaviour
{

    public Texture[] textures;

    int textureCount;
    RawImage rawImage;
    float currentHP;
    float maxHP;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        this.rawImage = GetComponent<RawImage>();
        textureCount = this.textures.Length - 1;

        this.maxHP = 100f; // HARDCODE
        this.currentHP= maxHP;
    }

    public void SetHP(float value)
    {
        if (value <= 0) 
        {
            this.currentHP = 0;
        }
        if (value <= maxHP)
        {
            this.currentHP = value;
        }
        else
        {
            this.currentHP = maxHP;
        }
        TextureSelecter();
    }

    void TextureSelecter()
    {
        int index = (int)(currentHP / maxHP * textureCount);
        this.rawImage.texture = textures[index];
    }

    public float GetHP()
    {
        return this.currentHP;
    }
}