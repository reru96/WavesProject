using System;
using UnityEngine;

public class ShowCanvas : MonoBehaviour
{
    public CanvasGroup canvas;
    void Start()
    {
        HideCanvas();
    }

    public void OnEnable()
    {
        RespawnManager.OnGameOver += ShowCanvasMenu;

    }

    public void OnDisable()
    {
        RespawnManager.OnGameOver -= ShowCanvasMenu;
    }

    public void HideCanvas()
    {
        canvas.alpha = 0f;
        canvas.blocksRaycasts = false;
        canvas.interactable = false;
    }

    public void ShowCanvasMenu()
    {
        canvas.alpha = 1f;
        canvas.blocksRaycasts = true;
        canvas.interactable = true;
    }
}