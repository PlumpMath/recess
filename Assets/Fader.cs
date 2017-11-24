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
    Color startColor;
    Color currentColor;
    Color endColor;
    bool shouldFade = false;
    FadeDirection fadeDirection;

    float startTime;
    float t;
    private float fadeTime = 0.2f;

    void Awake()
    {
        renderers = gameObject.GetComponentsInChildren<Renderer>();

        if (renderers == null || renderers.Length == 0)
        {
            return;
        }
        else
        {
            startColor = renderers[0].material.color;
            endColor = new Color(startColor.r, startColor.g, startColor.b, 0.0f);
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

            foreach(Renderer r in renderers){
                Color color = r.material.color;

                switch (fadeDirection)
                {
                    case FadeDirection.FADE_OUT:
                        color.a = 1.0f - t;
                        break;

                    case FadeDirection.FADE_IN:
                        color.a = t;
                        break;
                }

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
    }

    private void FinishFade(){
        shouldFade = false;
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