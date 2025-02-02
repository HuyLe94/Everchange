﻿#region Script Synopsis
    //A monobehavior that is attached to any object that receives collisions from bullet/laser shots and instantiates explosions if set and applies damage to the object.
    //Examples: Any object that receives damage (player, enemy, etc).
    //Learn more about the collision system at: https://neondagger.com/variabullet2d-system-guide/#collision-system
#endregion

using UnityEngine;
using System.Collections;

using Combat;

namespace ND_VariaBULLET
{
    public class ShotCollisionDamage : ShotCollision, IShotCollidable
    {
        [Tooltip("Sets the name of the explosion prefab to be instantiated when HP = 0.")]
        public string DeathExplosion;

        //[Tooltip("Health Points. Reduces according to incoming IDamager.DMG value upon collision.")]
        //public float HP = 10;

        [Range(0.1f, 8f)]
        [Tooltip("Changes the size of the last explosion (when HP = 0).")]
        public float FinalExplodeFactor = 2;

        [Tooltip("Enables indicating damage by flickering color (via DamageColor setting) when HP is reducing.")]
        public bool DamageFlicker;

        [Range(5, 40)]
        [Tooltip("Sets the duration frames for the DamageFlicker effect upon collision.")]
        public int FlickerDuration = 6;

        [Tooltip("Sets the color the object flickers to when HP is reducing and DamageFlicker is enabled.")]
        public Color DamageColor;
        private Color NormalColor;
        private SpriteRenderer rend;

        // ACCESSORS
        public Combatant Combatant { get; set; }

        void Awake()
        {
            this.Combatant = this.GetComponent<Combatant>();
            if (this.Combatant == null)
                throw new MissingComponentException($"{this.name} is missing component: {typeof(Combatant)}");
        }
        void Start()
        {
            this.Combatant.OnDeath.Add((Combatant c) =>
            {
                if (DeathExplosion != "")
                {
                    string explosion = DeathExplosion;
                    GameObject finalExplode = GlobalShotManager.Instance.ExplosionRequest(explosion, this);

                    finalExplode.transform.position = this.transform.position;
                    finalExplode.transform.parent = null;
                    finalExplode.transform.localScale = new Vector2(finalExplode.transform.localScale.x * FinalExplodeFactor, finalExplode.transform.localScale.y * FinalExplodeFactor);
                }
            });
            rend = GetComponent<SpriteRenderer>();
            NormalColor = rend.color;
        }

        public new IEnumerator OnLaserCollision(CollisionArgs sender)
        {
            if (CollisionFilter.collisionAccepted(sender.gameObject.layer, CollisionList))
            {
                setDamage(sender);
                CollisionFilter.setExplosion(LaserExplosion, ParentExplosion, this.transform, new Vector2(sender.point.x, sender.point.y), 0, this);
                yield return setFlicker();
            }
        }

        public new IEnumerator OnCollisionEnter2D(Collision2D collision)
        {
            if (CollisionFilter.collisionAccepted(collision.gameObject.layer, CollisionList))
            {
                setDamage(collision);
                CollisionFilter.setExplosion(BulletExplosion, ParentExplosion, this.transform, collision.contacts[0].point, 0, this);
                yield return setFlicker();
            }
        }

        protected virtual void setDamage(Collision2D collision)
        {
            var am = collision.gameObject.GetComponent<IAmmo>();
            if (am != null)
            {
                this.Combatant.TakeDamage(
                collision.gameObject.GetComponent<IAmmo>());
            }
        }

        protected virtual void setDamage(CollisionArgs collisionArgs)
        {
            if (collisionArgs.gameObject.GetComponent<IAmmo>() != null)
            {
                this.Combatant.TakeDamage(
                collisionArgs.gameObject.GetComponent<IAmmo>());
            }
        }

        protected IEnumerator setFlicker()
        {
            if (rend == null)
            {
                Utilities.Warn("No SpriteRenderer attached. Cannot flicker during damage.", this);
                yield return null;
            }

            if (DamageFlicker)
            {
                bool flicker = false;
                for (int i = 0; i < FlickerDuration * 2; i++)
                {
                    flicker = !flicker;

                    if (flicker)
                        rend.color = DamageColor;
                    else
                        rend.color = NormalColor;

                    yield return null;
                };

                rend.color = NormalColor;
            }
        }
    }
}