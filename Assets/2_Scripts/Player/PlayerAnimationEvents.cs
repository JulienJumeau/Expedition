using System;
using UnityEngine;

public class PlayerAnimationEvents : MonoBehaviour
{
    public void Hit()
    {
        Debug.Log("Hit");
    }

    #region Events

    public event EventHandler OnFootStep;

    private void FootStep() => OnFootStep?.Invoke(this, EventArgs.Empty);

    #endregion

}
