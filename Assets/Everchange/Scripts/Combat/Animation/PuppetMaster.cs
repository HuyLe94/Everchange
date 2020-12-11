﻿using System;
using UnityEngine;

using Utilities;

#pragma warning disable
namespace Combat.Animation
{
    /// <summary>
    /// Handles the transition of animations based on Combatant 
    /// behaviour.
    /// <list type="bullet">
    /// <item>For now, all characters will look in the direction that their weapons are pointing.</item>
    /// </list>
    /// </summary>
    public class PuppetMaster
    {
        /// <summary>
        /// The Combatant that will be animated by this PuppetMaster.
        /// </summary>
        public WeaponWielder Puppet { get; set; }
        Boolean PuppetIsPlayer
        { get => this.Puppet.GetComponent<PlayerMovement>() != null; }

        private Animator CharacterAnimator { get; set; }
        private bool SpriteOnWrapper { get; set; }
        public bool Active { get; set; }
        public PuppetMaster(Animator animator, WeaponWielder puppet, bool spriteOnWrapper = false)
        {
            this.CharacterAnimator = animator;
            this.Puppet = puppet;
            this.Active = true;
            this.SpriteOnWrapper = spriteOnWrapper;
        }

        /// <summary>
        /// Assigns animations to the puppet. 
        /// This method should be called in Combatant.Update().
        /// </summary>
        public void PullTheStrings()
        {
            if (this.Active)
            {
                if (this.Puppet.IsAlive())
                {
                    // DETERMINE if player is moving.
                    Boolean puppetIsMoving = this.Puppet.GetComponent<Rigidbody2D>().velocity.magnitude > 0;
                    // FIXME using mouse-cursor for AI puppet master
                    // CALCULATE the vector from the puppet's weapon to it's chest
                    var target = (!this.Puppet.Disarmed()
                        ? this.Puppet.RangedWeapon.GetGameObject().transform.position
                        : Camera.main.ScreenToWorldPoint(this.Puppet.GetComponent<PlayerMovement>().GetCursorPosition()));
                    var v = target - this.Puppet.GetBodyTransform(Combatant.BodyPart.Chest).position;
                    v = v.normalized;
                    // CALCULATE the direction the weapon is facing
                    var direction = PhysicsTool.DirectionFromHorizontal(v);
                    var dashing = this.Puppet.ActiveState == Combatant.CombatantState.Dashing;
                    // IF player is moving
                    if (puppetIsMoving)
                    {
                        // SET run animation based on direction
                        if (!this.SpriteOnWrapper)
                        {
                            switch (direction)
                            {
                                case PhysicsTool.Direction.Down:
                                    if (!dashing)
                                        SetState(AnimationState.RunDown);
                                    else
                                        SetState(AnimationState.DashDown);
                                    break;
                                case PhysicsTool.Direction.Up:
                                    if (!dashing)
                                        SetState(AnimationState.RunUp);
                                    else
                                        SetState(AnimationState.DashUp);
                                    break;
                                case PhysicsTool.Direction.Left:
                                    if (!dashing)
                                        SetState(AnimationState.RunLeft);
                                    else
                                        SetState(AnimationState.DashLeft);
                                    break;
                                case PhysicsTool.Direction.Right:
                                    if (!dashing)
                                        SetState(AnimationState.RunRight);
                                    else
                                        SetState(AnimationState.DashRight);
                                    break;
                            }
                        } else
                        {
                            SetState(AnimationState.RunUp);
                        }
                    }
                    else // ELSE
                    {
                        // SET idle animation based on direction
                        var ai = this.Puppet.GetComponent<AI.AIWeaponWielder>();
                        if (ai != null && !ai.InCombat() || this.SpriteOnWrapper)
                        {
                            SetState(AnimationState.Idle);
                        }
                        else
                        {
                            switch (direction)
                            {
                                case PhysicsTool.Direction.Down:
                                    SetState(AnimationState.IdleDown);
                                    break;
                                case PhysicsTool.Direction.Left:
                                    SetState(AnimationState.IdleLeft);
                                    break;
                                case PhysicsTool.Direction.Up:
                                    SetState(AnimationState.IdleUp);
                                    break;
                                case PhysicsTool.Direction.Right:
                                    SetState(AnimationState.IdleRight);
                                    break;
                                default:
                                    SetState(AnimationState.Idle);
                                    break;
                            }
                        }
                    }
                }
                else
                {
                    this.CharacterAnimator.StopPlayback();
                    this.CharacterAnimator.enabled = false;
                }
            }
        }

        private void SetState(AnimationState nextState)
        {
            var IsIdle = this.CharacterAnimator.GetBool("IsIdle");
            var speed = !this.PuppetIsPlayer ? this.Puppet.GetComponent<AI.AIWeaponWielder>().speed : 12f;

            if (!IsIdle && nextState == AnimationState.Idle)
            {
                this.CharacterAnimator.SetBool("IsIdle", true);
                //this.CharacterAnimator.StopPlayback();
                this.CharacterAnimator.speed = .2f;
                this.CharacterAnimator.Play(AnimationState.Idle.ToString(), 0);
            }
            else if (!this.CharacterAnimator.GetCurrentAnimatorStateInfo(0).IsName(nextState.ToString()))
            {
                this.CharacterAnimator.SetBool("IsIdle", false);
                //this.CharacterAnimator.StopPlayback();
                this.CharacterAnimator.speed = 0.08f * speed;
                this.CharacterAnimator.Play(nextState.ToString(), 0);
            }
        }
    }
}
