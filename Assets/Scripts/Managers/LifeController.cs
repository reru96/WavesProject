using UnityEngine;
using UnityEngine.Events;
using UnityEngine.SceneManagement;

public class LifeController : MonoBehaviour
{
    public Object objectSO;

    public UnityEvent OnDeath;

    public int GetHp () => (int)objectSO.currentHp;
    public int GetMaxHp() => (int)objectSO.maxHp;

    public void Awake()
    {
        objectSO.currentHp = objectSO.maxHp;
    }

    public void TakeDamage(int amount)
    {
        if (amount <= 0) return;

        if (amount > 0)
        {
            SetHp(objectSO.currentHp - amount);
        }
    }
    public void SetHp(float hp)
    {
        float oldHp = objectSO.currentHp;
        objectSO.currentHp = Mathf.Clamp(hp, 0, objectSO.maxHp);

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
            case DeathAction.Die:
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
