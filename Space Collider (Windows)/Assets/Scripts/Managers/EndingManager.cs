﻿using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class EndingManager : MonoBehaviour
{
    [Header("Credits Settings")]
    [SerializeField] private float creditsPositionY = 600;
    [SerializeField] private float creditsScrollSpeed = 0.5f;

    [Header("Setup")]
    [SerializeField] private Canvas endingUI = null;
    [SerializeField] private Canvas creditsUI = null;
    [SerializeField] private RectTransform credits = null;
    [SerializeField] private Text loadingText = null;
    [SerializeField] private AudioClip clickSound = null;

    private AudioSource audioSource;
    private bool loading = false;

    void Awake()
    {
        audioSource = GetComponent<AudioSource>();
        PlayerPrefs.SetInt("StandardLevel", 1);
        PlayerPrefs.Save();
        if (audioSource) audioSource.volume = PlayerPrefs.GetFloat("SoundVolume");
        if (Camera.main.GetComponent<AudioSource>()) Camera.main.GetComponent<AudioSource>().volume = PlayerPrefs.GetFloat("MusicVolume");
        endingUI.enabled = true;
        creditsUI.enabled = false;
    }

    void Update()
    {
        if (audioSource) audioSource.volume = PlayerPrefs.GetFloat("SoundVolume");
        if (Camera.main.GetComponent<AudioSource>()) Camera.main.GetComponent<AudioSource>().volume = PlayerPrefs.GetFloat("MusicVolume");
        if (!creditsUI.enabled) credits.anchoredPosition = new Vector2(0, creditsPositionY);
        if (!loading)
        {
            loadingText.enabled = false;
        } else
        {
            loadingText.enabled = true;
        }
    }

    public void toMainMenu()
    {
        if (audioSource && clickSound) audioSource.PlayOneShot(clickSound, PlayerPrefs.GetFloat("SoundVolume"));
        StartCoroutine(loadScene("Main Menu"));
    }

    public void clickCredits()
    {
        if (audioSource && clickSound) audioSource.PlayOneShot(clickSound, PlayerPrefs.GetFloat("SoundVolume"));
        if (!creditsUI.enabled)
        {
            endingUI.enabled = false;
            creditsUI.enabled = true;
            StartCoroutine(scrollCredits());
        } else
        {
            endingUI.enabled = true;
            creditsUI.enabled = false;
            StopCoroutine(scrollCredits());
        }
    }

    IEnumerator scrollCredits()
    {
        while (creditsUI.enabled)
        {
            yield return new WaitForEndOfFrame();
            if (creditsUI.enabled) credits.anchoredPosition -= new Vector2(0, creditsScrollSpeed);
            if (credits.anchoredPosition.y <= -creditsPositionY)
            {
                endingUI.enabled = true;
                creditsUI.enabled = false;
                yield break;
            }
        }
    }

    IEnumerator loadScene(string scene)
    {
        if (!loading)
        {
            AsyncOperation load = SceneManager.LoadSceneAsync(scene);
            loading = true;
            while (!load.isDone)
            {
                loadingText.text = "Loading: " + Mathf.Floor(load.progress * 100) + "%";
                endingUI.enabled = false;
                creditsUI.enabled = false;
                yield return null;
            }
            loading = false;
            loadingText.text = "Loading: 0%";
        }
    }
}