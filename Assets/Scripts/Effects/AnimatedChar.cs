using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class AnimatedChar : MonoBehaviour
{
    public Sprite[] charSprites;
    private SpriteRenderer spriteRenderer;
    private Image image;

    public int digit = 0;
    private int frame = 0;

    public int noChars;
    public int noFrames;

    public float fps = 10;
    private float timer;

    public int offset = 0;

    // Start is called before the first frame update
    void Start()
    {
        spriteRenderer = GetComponent<SpriteRenderer>();
        if (!spriteRenderer)
        {
            image = GetComponent<Image>();
        }
        Debug.Assert(spriteRenderer != null || image != null);
        timer = 1 / fps;
        UpdateSprite(0);
    }
    public void UpdateSprite(int newFrame)
    {
        int loopedFrame = (newFrame + offset) % noFrames;
        int spriteIdx = digit + loopedFrame * noChars;
        if (spriteRenderer)
        {
            spriteRenderer.sprite = charSprites[spriteIdx];
        }
        else
        {
            image.sprite = charSprites[spriteIdx];
        }
    }

    // Update is called once per frame
    void Update()
    {
        timer -= Time.deltaTime;
        if(timer <= 0)
        {
            timer = 1 / fps;
            frame++;
            frame %= noFrames;
            UpdateSprite(frame);
        }
    }
}
