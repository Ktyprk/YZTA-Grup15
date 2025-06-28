using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class PlayerState : MonoBehaviour
{
    public AnimatorOverrideController animatorController;
    protected PlayerController controller;

    public PlayerState(PlayerController controller)
    {
        this.controller = controller;
    }

    public virtual void Enter() { }
    public virtual void Exit() { }
    public virtual void Update() { }
    public virtual void FixedUpdate() { }
}