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
        if (RespawnManager.Instance != null)
        {
            UpdateLives();

            RespawnManager.Instance.OnPlayerReady += UpdateLives;
        }
    }

    private void OnDestroy()
    {
        if (RespawnManager.Instance != null)
        {
            RespawnManager.Instance.OnPlayerReady -= UpdateLives;
        }
    }

    private void Update()
    {
        UpdateLives();
    }

    public void UpdateLives()
    {
        if (RespawnManager.Instance == null) return;

       var player = RespawnManager.Instance.GetPlayer();
       var life = player.GetComponent<LifeController>();
       int hp = life.GetHp();

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
