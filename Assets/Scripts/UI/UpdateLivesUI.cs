using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class UpdateLivesUI : MonoBehaviour
{
    [SerializeField] private GameObject lifeIconPrefab;
    [SerializeField] private Transform lifeParent;

    [Header("UI Refresh Settings")]
    [SerializeField] private float refreshRateSeconds = 0.15f; 

    private LifeController _playerLifeController;
    private List<GameObject> _icons = new List<GameObject>();
    private Coroutine _monitorCoroutine;
    private int _lastReportedHp = -1; 


    private void OnEnable()
    {
        if (RespawnManager.Instance != null)
        {
            RespawnManager.Instance.OnPlayerSpawned += OnPlayerSpawned;
            RespawnManager.OnGameOver += ClearUI;
        }
    }

    private void OnDisable()
    {
        if (RespawnManager.Instance != null)
        {
            RespawnManager.Instance.OnPlayerSpawned -= OnPlayerSpawned;
            RespawnManager.OnGameOver -= ClearUI;
        }   

        if (_monitorCoroutine != null)
        {
            StopCoroutine(_monitorCoroutine);
            _monitorCoroutine = null;
        }
    }

    private void OnPlayerSpawned(GameObject player)
    {
        _playerLifeController = player.GetComponent<LifeController>();

        if (_playerLifeController == null)
        {
            Debug.LogError("Il Player spawnato non ha un componente LifeController.");
            return;
        }

 
        if (_monitorCoroutine != null) StopCoroutine(_monitorCoroutine);

        _lastReportedHp = _playerLifeController.GetHp();
        UpdateLivesDisplay(_lastReportedHp); 

        _monitorCoroutine = StartCoroutine(MonitorHealthRoutine());
    }


    private IEnumerator MonitorHealthRoutine()
    {
        WaitForSeconds waitTime = new WaitForSeconds(refreshRateSeconds);

        while (_playerLifeController != null && _playerLifeController.isActiveAndEnabled)
        {
            int currentHp = _playerLifeController.GetHp();

            if (currentHp != _lastReportedHp)
            {
                UpdateLivesDisplay(currentHp);
                _lastReportedHp = currentHp;
            }

            yield return waitTime;
        }
    }


    private void UpdateLivesDisplay(int currentHp)
    {
        foreach (var icon in _icons)
            Destroy(icon);
        _icons.Clear();

        for (int i = 0; i < currentHp; i++)
        {
            GameObject newIcon = Instantiate(lifeIconPrefab, lifeParent);
            newIcon.SetActive(true);
            _icons.Add(newIcon);
        }
    }

    private void ClearUI()
    {
       if (_monitorCoroutine != null)
        {
            StopCoroutine(_monitorCoroutine);
            _monitorCoroutine = null;
        }

        foreach (var icon in _icons)
            Destroy(icon);
        _icons.Clear();

        _playerLifeController = null;
    }
}
