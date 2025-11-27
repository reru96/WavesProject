using UnityEngine;
using UnityEngine.EventSystems;
using DG.Tweening;
using System.Collections;

public class ButtonAnimationTransform : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
{
    [SerializeField] private MainMenuWave _mainMenuWave;
    [SerializeField] private float _amplitudeScale = 10f;

    [SerializeField] private float _offsetAmount = 0.5f;

    [SerializeField] private float _exitDelay = 0.2f;

    [Range(1.1f,3f)][SerializeField] private float _scaleIncrement = 1.25f;

    private Vector3 _originalScale;

    private float _phaseOffset;
    private Coroutine _exitRoutine;

    private void Start()
    {
        // Usa l’ordine nella gerarchia come offset
        int index = transform.GetSiblingIndex();


        _phaseOffset = index * _offsetAmount;
        _originalScale = transform.localScale;
    }

    private void Update()
    {
        // Movimento legato all'amplitude dell’onda
        float amplitude = _mainMenuWave.CurrentAmplitude;
        float newY = Mathf.Sin(Time.time + _phaseOffset) * amplitude * _amplitudeScale;

        transform.localPosition = new Vector3(
            transform.localPosition.x,
            newY,
            transform.localPosition.z
        );
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        if (_exitRoutine != null)
        {
            StopCoroutine(_exitRoutine);
            _exitRoutine = null;
        }
        _mainMenuWave.ImproveAmplitude();
        transform.localScale = _originalScale * _scaleIncrement;
        Debug.Log("BUTTON ENTER");
    }


    public void OnPointerExit(PointerEventData eventData)
    {
        _exitRoutine = StartCoroutine(DelayedExit());
        transform.localScale = _originalScale;
        Debug.Log("BUTTON EXIT");
    }

    private IEnumerator DelayedExit()
    {
        yield return new WaitForSeconds(_exitDelay);

        _mainMenuWave.RestoreAmplitude();
        _exitRoutine = null;
    }

}
