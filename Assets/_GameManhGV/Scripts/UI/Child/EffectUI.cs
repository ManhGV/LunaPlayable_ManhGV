using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Random = UnityEngine.Random;

public class EffectUI : Singleton<EffectUI>
{
    [Header("Effects")] [SerializeField]
    Image[] effectImages; // Array of effect images that can be assigned in inspector

    [SerializeField] Image Background;
    [SerializeField] RectTransform containerPanel; // Panel that will contain the effects

    [Header("Settings")] [SerializeField] float _duration = 1f;
    [SerializeField] bool _isPlay = false;

    private List<Image> availableEffects = new List<Image>();

    void Start()
    {
        // Initialize all effects to invisible
        foreach (var effect in effectImages)
        {
            SetAlpha(effect, 0);
        }

        SetAlpha(Background, 0);

        // Make sure we have a container panel
        if (containerPanel == null)
        {
            Debug.LogError("Container Panel is not assigned in EffectUI!");
        }
    }

    private void Update()
    {
        if(Input.GetKeyDown(KeyCode.P))
            Play();
    }

    public void Play()
    {
        // Don't play if already playing or no panel
        if (_isPlay || containerPanel == null) return;

        _isPlay = true;

        // Find all images that are currently not visible
        availableEffects.Clear();
        foreach (var effect in effectImages)
        {
            if (effect.color.a <= 0.01f)
            {
                availableEffects.Add(effect);
            }
        }

        // If no available effects, don't proceed
        if (availableEffects.Count == 0) return;

        // Randomly select one of the available effects
        Image chosenEffect = availableEffects[Random.Range(0, availableEffects.Count)];
        RectTransform chosenRectTransform = chosenEffect.GetComponent<RectTransform>();

        // Set background alpha and start fade
        SetAlpha(Background, 1);
        StartCoroutine(FadeOutEffect(Background));

        // Position the effect within the panel
        PositionWithinPanel(chosenRectTransform);

        // Set effect alpha and start fade
        SetAlpha(chosenEffect, 1);
        StartCoroutine(FadeOutEffect(chosenEffect));
    }

    private void PositionWithinPanel(RectTransform effectTransform)
    {
        // Get panel dimensions
        Vector2 panelSize = containerPanel.rect.size;

        // Get effect dimensions
        Vector2 effectSize = effectTransform.rect.size;

        // Calculate max position while keeping the effect within panel bounds
        float maxPosX = (panelSize.x - effectSize.x) / 2;
        float maxPosY = (panelSize.y - effectSize.y) / 2;

        // Generate random position within panel boundaries
        float randomX = Random.Range(-maxPosX, maxPosX);
        float randomY = Random.Range(-maxPosY, maxPosY);

        // Set position
        effectTransform.anchoredPosition = new Vector2(randomX, randomY);

        // Set random rotation
        effectTransform.localRotation = Quaternion.Euler(0, 0, Random.Range(-45f, 45f));
    }

    private IEnumerator FadeOutEffect(Image img)
    {
        float elapsed = 0f;
        Color color = img.color;
        color.a = 1f;
        img.color = color;

        // Use a single calculation per frame approach instead of lerping multiple values
        while (elapsed < _duration)
        {
            elapsed += Time.deltaTime;
            color.a = Mathf.Clamp01(1 - (elapsed / _duration));
            img.color = color;
            yield return null;
        }

        // Ensure alpha is exactly 0 at the end
        color.a = 0f;
        img.color = color;

        // Only mark as not playing when it's the background fading out
        if (img == Background)
            _isPlay = false;
    }

    private void SetAlpha(Image img, float alpha)
    {
        Color c = img.color;
        c.a = alpha;
        img.color = c;
    }
}