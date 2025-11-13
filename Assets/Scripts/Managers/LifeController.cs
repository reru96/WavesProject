using System;
using UnityEngine;
using UnityEngine.SceneManagement;

public class LifeController : MonoBehaviour
{
    public ObjectSO objectSO;

    public int currentHp = 1;
    public event Action OnDeath;
    public event Action OnHit;

    public int GetHp() => currentHp;
    public int GetMaxHp() => objectSO.maxHp;

    public void Awake()
    {
        currentHp = objectSO.maxHp;
    }

    public void OnEnable()
    {
        OnHit += HitAction;
        OnDeath += Death;
    }

    public void OnDisable()
    {
        OnHit -= HitAction;
        OnDeath -= Death;
    }
  
    public void HitAction()
    {
        AudioManager.Instance.PlaySfxRandomPitch(objectSO.hitSound);
        // qui possono andare animazioni o effetti particellari
    }

    public void Death()
    {
        AudioManager.Instance.PlaySfx(objectSO.deathSound);
    }

    public void TakeDamage(int amount)
    {
        if (amount <= 0) return;

        if (amount > 0)
        {
            OnHit?.Invoke();
            SetHp(currentHp - amount);
        }
    }

    public void SetHp(int hp)
    {
        int oldHp = currentHp;
        currentHp = Mathf.Clamp(hp, 0, objectSO.maxHp);
        
        if (CompareTag("Player"))
            RespawnManager.Instance.NotifyLifeChanged();


        if (oldHp > 0 && currentHp == 0)
        {
            OnDeath?.Invoke();
            HandleDeath();
        }
    }

    public void HandleDeath()
    {
        switch (objectSO.deathType)
        {
            case DeathAction.None:
                break;
            case DeathAction.Destroy:
                Destroy(gameObject);
                break;
            case DeathAction.Respawn:
                RespawnManager.Instance.PlayerDied();
                break;
            case DeathAction.Disable:
                ObjectPooler.Instance.ReturnToPool(gameObject);
                break;
            case DeathAction.SceneReload:
                SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
                break;
        }
    }
}
