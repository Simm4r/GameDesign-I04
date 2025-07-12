using System.Collections;
using UnityEngine;
using Unity.Cinemachine;

public class CinemachineOneShotSwitcher : MonoBehaviour
{
    [System.Serializable]
    public class CameraStep
    {
        public CinemachineCamera virtualCamera;
        public float duration; // Quanto tempo resta attiva prima di passare alla prossima
    }

    [Header("Sequenza delle camere")]
    public CameraStep[] sequence;

    [Header("Priorità")]
    public int activePriority = 20;
    public int inactivePriority = 10;

    private void Start()
    {
        foreach (var step in sequence)
        {
            if (step.virtualCamera != null)
                step.virtualCamera.Priority = inactivePriority;
        }

        StartCoroutine(RunSequence());
    }

    private IEnumerator RunSequence()
    {
        for (int i = 0; i < sequence.Length; i++)
        {
            var currentStep = sequence[i];

            if (currentStep.virtualCamera != null)
            {
                currentStep.virtualCamera.Priority = activePriority;

                // Abbassa la priorità della camera precedente (se esiste)
                if (i > 0 && sequence[i - 1].virtualCamera != null)
                {
                    sequence[i - 1].virtualCamera.Priority = inactivePriority;
                }

                yield return new WaitForSeconds(currentStep.duration);
            }
        }

        // Alla fine spegni l’ultima camera
        var lastCam = sequence[sequence.Length - 1].virtualCamera;
        if (lastCam != null)
            lastCam.Priority = inactivePriority;
    }
}