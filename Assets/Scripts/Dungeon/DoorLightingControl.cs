using System;
using System.Collections;
using UnityEngine;

[DisallowMultipleComponent]
public class DoorLightingControl : MonoBehaviour
{
    bool isLit = false;

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.tag == Tags.player || other.tag == Tags.playerWeapon) FadeInDoor();
    }

    public void FadeInDoor()
    {
        // fade in mateiral
        Material material = new Material(GameResources.Instance.variableLitShader);

        if (isLit) return;

        SpriteRenderer[] spriteRenderers = GetComponentsInParent<SpriteRenderer>();

        Array.ForEach(spriteRenderers, (spriteRenderer) =>
        {
            StartCoroutine(FadeInSprite(spriteRenderer, material));
        });

        isLit = true;
    }

    IEnumerator FadeInSprite(SpriteRenderer spriteRenderer, Material material)
    {
        spriteRenderer.material = material;

        for (float i = 0.05f; i <= 1f; i += Time.deltaTime / Settings.fadeInTime)
        {
            material.SetFloat("Alpha_Slider", i);
            yield return null;
        }

        spriteRenderer.material = GameResources.Instance.litMaterial;
    }
}
