using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class UpdateLivesUI : MonoBehaviour
{
    [SerializeField] private GameObject lifeIcon;
    [SerializeField] private Transform lifeParent;

    private List<GameObject> icons = new List<GameObject>();

    private void Start()
    {
        UpdateLives();
    }

    private void Update()
    {
        UpdateLives();
    }

    private void OnEnable()
    {
        if (RespawnManager.Instance != null)
            RespawnManager.Instance.OnPlayerReady += UpdateLives;
    }

    private void OnDisable()
    {
        if (RespawnManager.Instance != null)
            RespawnManager.Instance.OnPlayerReady -= UpdateLives;
    }

    public void UpdateLives()
    {

        int life = RespawnManager.Instance.LeftTry;

        foreach (var icon in icons)
            Destroy(icon);
        icons.Clear();

        for (int i = 0; i < life; i++)
        {
            GameObject newIcon = Instantiate(lifeIcon, lifeParent);
            newIcon.SetActive(true);
            icons.Add(newIcon);
        }
    }
}
