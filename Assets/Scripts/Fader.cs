using System.Collections;
using System.Linq;
using System.Collections.Generic;
using UnityEngine;

public enum FadeDirection
{
    FADE_IN,
    FADE_OUT
}

public class Fader : MonoBehaviour
{
    Renderer[] renderers;
    Color currentColor;
    bool shouldFade = false;
    FadeDirection fadeDirection;

    float startTime;
    float t;
    private float fadeTime = 0.15f;
    private float minAlpha = 0.1f;

    void Awake()
    {
        renderers = gameObject.GetComponentsInChildren<Renderer>();

        if (renderers == null || renderers.Length == 0)
        {
            return;
        }
    }
    void Start()
    {
        gameObject.layer = LayerMask.NameToLayer("Seethru");
    }

    // Update is called once per frame
    void Update()
    {
        if (shouldFade)
        {
            t = (Time.time - startTime) / fadeTime;

            foreach (Renderer r in renderers)
            {
                Color color = r.material.color;
                float alpha = 1.0f;

                switch (fadeDirection)
                {
                    case FadeDirection.FADE_OUT:
                        alpha = 1.0f - t;
                        break;

                    case FadeDirection.FADE_IN:
                        alpha = t;
                        break;
                }

                color.a = Mathf.Max(minAlpha, alpha);
                r.material.SetColor("_Color", color);
            }

            if (t >= 1.0f)
            {
                FinishFade();
            }
        }
    }

    private void StartFade(FadeDirection direction)
    {
        fadeDirection = direction;
        shouldFade = true;
        startTime = Time.time;

        foreach (Renderer r in renderers)
        {
            StandardShaderUtils.ChangeRenderMode(r.material, StandardShaderUtils.BlendMode.Transparent);
        }
    }

    private void FinishFade()
    {
        shouldFade = false;
        if (fadeDirection == FadeDirection.FADE_IN)
        {

            foreach (Renderer r in renderers)
            {
                StandardShaderUtils.ChangeRenderMode(r.material, StandardShaderUtils.BlendMode.Opaque);
            }
        }
    }

    public void FadeOut()
    {
        StartFade(FadeDirection.FADE_OUT);
    }

    public void FadeIn()
    {
        StartFade(FadeDirection.FADE_IN);
    }
}