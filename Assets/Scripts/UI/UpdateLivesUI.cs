using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class UpdateLivesUI : MonoBehaviour
{
    [SerializeField] private GameObject lifeIcon;
    [SerializeField] private Transform lifeParent;

    private List<GameObject> icons = new List<GameObject>();

    private void OnEnable()
    {
        if (RespawnManager.Instance != null)
            RespawnManager.Instance.OnLivesChanged += UpdateLives;
    }

    private void OnDisable()
    {
        if (RespawnManager.Instance != null)
            RespawnManager.Instance.OnLivesChanged -= UpdateLives;
    }

    private void Start()
    {
        UpdateLives(RespawnManager.Instance != null ? RespawnManager.Instance.LeftTry : 0);
    }

    private void UpdateLives(int lifeCount)
    {
        foreach (var icon in icons)
            Destroy(icon);
        icons.Clear();

        for (int i = 0; i < lifeCount; i++)
        {
            GameObject newIcon = Instantiate(lifeIcon, lifeParent);
            newIcon.SetActive(true);
            icons.Add(newIcon);
        }
    }
}
