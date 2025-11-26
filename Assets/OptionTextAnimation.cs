using UnityEngine;

public class OptionTextAnimation : MonoBehaviour
{

    [SerializeField] private float _amplitudeScale = 10f;

    [SerializeField] private float _offsetAmount = 0.5f;

    private float _phaseOffset;

    private void Start()
    {
        // Usa l’ordine nella gerarchia come offset
        int index = transform.GetSiblingIndex();


        _phaseOffset = index * _offsetAmount;
    }

    private void Update()
    {

        float newX = Mathf.Sin(Time.time + _phaseOffset) * _amplitudeScale;

        transform.localPosition = new Vector3(
            newX,
            transform.localPosition.y,
            transform.localPosition.z
        );
    }

}
