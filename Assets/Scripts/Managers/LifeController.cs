using System;
using UnityEngine;
using UnityEngine.SceneManagement;

public class LifeController : MonoBehaviour
{
    public Object objectSO;

    public event Action OnDeath;
    public event Action OnHit;

    public int GetHp() => objectSO.currentHp;
    public int GetMaxHp() => objectSO.maxHp;

    public bool isInvincible = false;

    public void Awake()
    {
        objectSO.currentHp = objectSO.maxHp;
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
        if (isInvincible) return;

        if (amount <= 0) return;

        if (amount > 0)
        {
            OnHit?.Invoke();
            SetHp(objectSO.currentHp - amount);
        }
    }

    public void SetHp(int hp)
    {
        int oldHp = objectSO.currentHp;
        objectSO.currentHp = Mathf.Clamp(hp, 0, objectSO.maxHp);
        
        if (CompareTag("Player"))
            RespawnManager.Instance.NotifyLifeChanged();


        if (oldHp > 0 && objectSO.currentHp == 0)
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
