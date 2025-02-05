﻿using CrowdedRoles.Components;
using UnityEngine;

namespace CrowdedRoles.UI
{
    public enum SetActiveReason : byte
    {
        Hud = 0,
        Death = 1,
        Revival = 2
    }
    public abstract class CooldownButton
    {
        public abstract float MaxTimer { get; }
        /// <summary>
        /// Sprite being assigned on button sprite. Gets called once in <see cref="Components.CustomButtonManager.Start"/>
        /// </summary>
        public abstract Sprite DefaultSprite { get; }
        /// <summary>
        /// Position behaviour
        /// </summary>
        public abstract IPosition Position { get; }

        public GameObject gameObject => CustomButtonManager.gameObject;
        internal int alignIndex = -1;

        public float Timer
        {
            get => CustomButtonManager.Timer;
            set => CustomButtonManager.Timer = value;
        }
        
        /// <summary>
        /// Changes button sprite. Use this instead of renderer
        /// </summary>
        public Sprite Sprite
        {
            get => CustomButtonManager.Renderer.sprite;
            set
            {
                CustomButtonManager.Renderer.sprite = value;
                CooldownHelpers.SetCooldownNormalizedUvs(CustomButtonManager.Renderer); // CustomButtonManager.Renderer.SetCooldownNormalizedUvs();
            } 
        }

        private bool _visible = true;
        private Color _oldTimerTextColor;

        /// <summary>
        /// Make button Visible or not. Difference from <see cref="Active"/> is that cooldown, <see cref="OnUpdate"/> and stuff still executes
        /// <br/>If button is invisible, clicks will not be handled
        /// </summary>
        public bool Visible
        {
            get => _visible;
            set
            {
                // ReSharper disable once AssignmentInConditionalExpression
                if(_visible = value)
                {
                    CustomButtonManager.TimerText.color = _oldTimerTextColor;
                    Triggered = _triggered; // trigger property
                } else
                {
                    _oldTimerTextColor = CustomButtonManager.TimerText.color;
                    CustomButtonManager.TimerText.color = CustomButtonManager.Renderer.color = Color.clear;
                }
            }
        }

        /// <summary>
        /// <see cref="GameObject.SetActive"/>
        /// </summary>
        public bool Active
        {
            get => CustomButtonManager.gameObject.active;
            set => CustomButtonManager.gameObject.SetActive(value);
        }

        // ReSharper disable once IdentifierTypo
        // ReSharper disable once StringLiteralTypo
        private static readonly int Desat = Shader.PropertyToID("_Desat");
        private bool _triggered;

        /// <summary>
        /// Changes sprite color (like kill button when it has a target)
        /// </summary>
        public bool Triggered
        {
            get => _triggered;
            set
            {
                _triggered = value;
                if (value)
                {
                    CustomButtonManager.Renderer.color = Palette.EnabledColor;
                    CustomButtonManager.Renderer.material.SetFloat(Desat, 0f);
                }
                else
                {
                    CustomButtonManager.Renderer.color = Palette.DisabledClear;
                    CustomButtonManager.Renderer.material.SetFloat(Desat, 1f);
                }
            }
        }

        public bool IsCoolingDown => Timer > 0f;
        public bool IsEffectEnabled
        {
            get => CustomButtonManager.IsEffectEnabled;
            set => CustomButtonManager.IsEffectEnabled = value;
        }

        public CustomButtonManager CustomButtonManager { get; internal set; } = null!;

        public virtual bool DontDestroyOnGameStart => false;
        public virtual float EffectDuration => 0f;
        public virtual Vector2 Size => new(125, 125);

        public abstract bool OnClick();
        public abstract bool CanUse();

        public virtual bool ShouldSetActive(bool expected, SetActiveReason reason) => expected;
        public virtual bool ShouldCooldown() => PlayerControl.LocalPlayer.CanMove;
        public virtual void OnEffectStart() {}
        public virtual void OnEffectEnd() {}
        public virtual void OnCooldownEnd() {}

        public virtual void OnStart() {}
        public virtual void OnUpdate() {}
    }
}