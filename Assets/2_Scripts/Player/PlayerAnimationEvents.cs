using System;
using UnityEngine;

public class PlayerAnimationEvents : MonoBehaviour
{
    public void Hit()
    {
        Debug.Log("Hit");
    }

    #region Events

    public event EventHandler OnFootStepWalk;

    private void FootStepWalk() => OnFootStepWalk?.Invoke(this, EventArgs.Empty);


    public event EventHandler OnFootStepRun;

    private void FootStepRun() => OnFootStepRun?.Invoke(this, EventArgs.Empty);


    public event EventHandler OnFootStepCrouch;

    private void FootStepCrouch() => OnFootStepCrouch?.Invoke(this, EventArgs.Empty);

    #endregion

}
