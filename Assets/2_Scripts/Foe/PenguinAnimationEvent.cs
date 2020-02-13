using System;
using UnityEngine;

public class PenguinAnimationEvent : MonoBehaviour
{
    public void Hit()
    {
        Debug.Log("Hit");
    }

    #region Events

    public event EventHandler OnFootStepRun;

    private void FootStepRun() => OnFootStepRun?.Invoke(this, EventArgs.Empty);

    #endregion

}
