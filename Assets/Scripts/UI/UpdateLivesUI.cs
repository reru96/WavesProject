using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class UpdateLivesUI : MonoBehaviour
{
    [SerializeField] private GameObject lifeIcon;
    [SerializeField] private Transform lifeParent;

    private LifeController lifeController;
    private List<GameObject> icons = new List<GameObject>();

    private void Start()
    {
        if (RespawnManager.Instance != null)
        {
            RespawnManager.Instance.OnPlayerReady += OnPlayerReady;
        }
    }

    private void OnDestroy()
    {
        if (RespawnManager.Instance != null)
        {
            RespawnManager.Instance.OnPlayerReady -= OnPlayerReady;
        }
    }

    private void OnPlayerReady()
    {
        var player = RespawnManager.Instance.GetPlayer();
        if (player != null)
        {
            lifeController = player.GetComponent<LifeController>();
            UpdateLives();
        }
    }

    public void UpdateLives()
    {
        if (lifeController == null) return;

        int hp = lifeController.GetHp();

        foreach (var icon in icons)
            Destroy(icon);

        icons.Clear();

        for (int i = 0; i < hp; i++)
        {
            GameObject newIcon = Instantiate(lifeIcon, lifeParent);
            newIcon.SetActive(true);
            icons.Add(newIcon);
        }
    }
}
