using System;
using UnityEngine;

public class PenguinAnimationEvent : MonoBehaviour
{
   
    [SerializeField] private Penguin _penguin;

    public void Hit()
    {
        Debug.Log("Hit");
    }

    public void FootStepsPenguin()
    {
        _penguin._audioSource.clip = _penguin._audioClipPenguinFootstepsRunnning[UnityEngine.Random.Range(0, _penguin._audioClipPenguinFootstepsRunnning.Length - 1)];
        _penguin._audioSource.PlayOneShot(_penguin._audioSource.clip);
    }

    #region Events

    public event EventHandler OnFootStepRun;

    private void FootStepRun() => OnFootStepRun?.Invoke(this, EventArgs.Empty);

    #endregion

}
