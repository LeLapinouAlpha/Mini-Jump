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

        this.maxHP = 100f;
        this.currentHP= maxHP;

        SetHP(0);
    }

    // Update is called once per frame
    void Update()
    {
        // TODO : Mettre à jour le rendu de la barre de vie
        //avec la valeur d'hp courante
    }
    public void SetHP(float value)
    {
        if (value <= maxHP)
        {
            this.currentHP = value;
        }
        TextureSelecter();
        // TODO : Vérifier que la valeur est valide (négative
        //? trop grande ?)
        // avant de mettre à jour la variable d'instance hp.
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
