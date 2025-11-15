using UnityEngine;
using UnityEngine.InputSystem;

public class InputManager : MonoBehaviour
{
    public PathManager pathManager;
    public float ampStep = 0.1f;
    public float waveStep = 0.5f;

    public float scrollStep = 1f;

    public bool isDragging { get; private set; } = false;
    private Vector2 dragStartPos;

    void Update()
    {
        HandleInput();
    }

    // MODIFICA PRINCIPALE: Struttura semplificata con metodi separati
    // Ogni controllo (amplitude, wavelength, scroll) ha il suo metodo dedicato

    private void HandleInput()
    {
        // Tasto destro O tasto W/S controlla amplitude
        // W aumenta, S diminuisce
        if (Mouse.current.rightButton.isPressed || Input.GetKey(KeyCode.W) || Input.GetKey(KeyCode.S))
        {
            HandleAmplitude();
        }

        // Tasto sinistro O tasto A/D controlla wavelength
        // D aumenta, A diminuisce
        if (Mouse.current.leftButton.isPressed ||Input.GetKey(KeyCode.A) || Input.GetKey(KeyCode.D))
        {
            HandleWavelength();
        }

        HandleScroll();
    }

    // METODO 1: Gestisce amplitude con tasto destro O W/S
    private void HandleAmplitude()
    {
        // CONTROLLO TASTIERA: W aumenta, S diminuisce
        if (Input.GetKey(KeyCode.W))
        {
            pathManager.CurrentAmplitude = Mathf.Clamp(
                pathManager.CurrentAmplitude + ampStep * Time.deltaTime,
                pathManager.minAmplitude,
                pathManager.maxAmplitude
            );
            return; // Esci per evitare conflitti con il drag
        }

        if (Input.GetKey(KeyCode.S))
        {
            pathManager.CurrentAmplitude = Mathf.Clamp(
                pathManager.CurrentAmplitude - ampStep * Time.deltaTime,
                pathManager.minAmplitude,
                pathManager.maxAmplitude
            );
            return;
        }

        // CONTROLLO MOUSE: solo se nessun tasto tastiera è premuto
        // Inizializza il drag quando premi il tasto
        if (Mouse.current.rightButton.wasPressedThisFrame)
        {
            dragStartPos = Mouse.current.position.ReadValue();
            isDragging = true;
        }

        if (isDragging)
        {
            Vector2 currentPos = Mouse.current.position.ReadValue();
            float deltaX = currentPos.x - dragStartPos.x;

            // Soglia minima per evitare modifiche accidentali
            if (Mathf.Abs(deltaX) > 5f)
            {
                // Trascina a destra: aumenta amplitude
                // Trascina a sinistra: diminuisce amplitude
                if (deltaX > 0)
                {
                    pathManager.CurrentAmplitude = Mathf.Clamp(
                        pathManager.CurrentAmplitude + ampStep * Time.deltaTime,
                        pathManager.minAmplitude,
                        pathManager.maxAmplitude
                    );
                }
                else
                {
                    pathManager.CurrentAmplitude = Mathf.Clamp(
                        pathManager.CurrentAmplitude - ampStep * Time.deltaTime,
                        pathManager.minAmplitude,
                        pathManager.maxAmplitude
                    );
                }

                dragStartPos = currentPos;
            }
        }

        // Termina il drag quando rilasci
        if (Mouse.current.rightButton.wasReleasedThisFrame)
        {
            isDragging = false;
        }
    }

    // METODO 2: Gestisce wavelength con tasto sinistro O A/D
    private void HandleWavelength()
    {
        // CONTROLLO TASTIERA: D aumenta, A diminuisce
        if (Input.GetKey(KeyCode.D))
        {
            pathManager.CurrentWavelength = Mathf.Clamp(
                pathManager.CurrentWavelength + waveStep * Time.deltaTime,
                pathManager.minWavelength,
                pathManager.maxWavelength
            );
            return; // Esci per evitare conflitti con il drag
        }

        if (Input.GetKey(KeyCode.A))
        {
            pathManager.CurrentWavelength = Mathf.Clamp(
                pathManager.CurrentWavelength - waveStep * Time.deltaTime,
                pathManager.minWavelength,
                pathManager.maxWavelength
            );
            return;
        }

        // CONTROLLO MOUSE: solo se nessun tasto tastiera è premuto
        // Inizializza il drag quando premi il tasto
        if (Mouse.current.leftButton.wasPressedThisFrame)
        {
            dragStartPos = Mouse.current.position.ReadValue();
            isDragging = true;
        }

        if (isDragging)
        {
            Vector2 currentPos = Mouse.current.position.ReadValue();
            float deltaX = currentPos.x - dragStartPos.x;

            // Soglia minima per evitare modifiche accidentali
            if (Mathf.Abs(deltaX) > 5f)
            {
                // Trascina a destra: aumenta wavelength
                // Trascina a sinistra: diminuisce wavelength
                if (deltaX > 0)
                {
                    pathManager.CurrentWavelength = Mathf.Clamp(
                        pathManager.CurrentWavelength + waveStep * Time.deltaTime,
                        pathManager.minWavelength,
                        pathManager.maxWavelength
                    );
                }
                else
                {
                    pathManager.CurrentWavelength = Mathf.Clamp(
                        pathManager.CurrentWavelength - waveStep * Time.deltaTime,
                        pathManager.minWavelength,
                        pathManager.maxWavelength
                    );
                }

                dragStartPos = currentPos;
            }
        }

        // Termina il drag quando rilasci
        if (Mouse.current.leftButton.wasReleasedThisFrame)
        {
            isDragging = false;
        }
    }

    // METODO 3: Gestisce scroll per segment length (invariato)
    private void HandleScroll()
    {
        float scrollValue = Mouse.current.scroll.ReadValue().y;

        if (scrollValue > 0f)
        {
            pathManager.SetSegmentLength(pathManager.SegmentLength + scrollStep);
        }
        else if (scrollValue < 0f)
        {
            pathManager.SetSegmentLength(pathManager.SegmentLength - scrollStep);
        }
    }
}