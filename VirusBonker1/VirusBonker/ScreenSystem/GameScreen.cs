/* Original source Farseer Physics Engine:
 * Copyright (c) 2014 Ian Qvist, http://farseerphysics.codeplex.com
 * Microsoft Permissive License (Ms-PL) v1.1
 */

#region File Description

//-----------------------------------------------------------------------------
// PlayerIndexEventArgs.cs
//
// XNA Community Game Platform
// Copyright (C) Microsoft Corporation. All rights reserved.
//-----------------------------------------------------------------------------

#endregion

using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input.Touch;

namespace tainicom.Aether.Physics2D.Samples.ScreenSystem
{
    /// <summary>
    /// Enum describes the screen transition state.
    /// </summary>
    public enum ScreenState
    {
        TransitionOn,
        Active,
        TransitionOff,
        Hidden,
    }

    /// <summary>
    /// A screen is a single layer that has update and draw logic, and which
    /// can be combined with other layers to build up a complex menu system.
    /// For instance the main menu, the options menu, the "are you sure you
    /// want to quit" message box, and the main game itself are all implemented
    /// as screens.
    /// </summary>
    public abstract class GameScreen
    {
        private GestureType _enabledGestures = GestureType.None;
        private bool _otherScreenHasFocus;

        public GameScreen()
        {
            ScreenState = ScreenState.TransitionOn;
            TransitionPosition = 1;
            TransitionOffTime = TimeSpan.Zero;
            TransitionOnTime = TimeSpan.Zero;
            HasCursor = false;
            HasVirtualStick = false;
        }

        public bool HasCursor { get; set; }

        public bool HasVirtualStick { get; set; }

        /// <summary>
        /// Normally when one screen is brought up over the top of another,
        /// the first screen will transition off to make room for the new
        /// one. This property indicates whether the screen is only a small
        /// popup, in which case screens underneath it do not need to bother
        /// transitioning off.
        /// Обычно, когда один экран поднимается поверх другого, первый экран отключится, чтобы освободить место для нового один. 
        /// Это свойство указывает, является ли экран небольшим всплывающее окно, в этом случае экраны под ним не нужно беспокоить переход выключен.
        /// </summary>
        public bool IsPopup { get; protected set; }

        /// <summary>
        /// Indicates how long the screen takes to
        /// transition on when it is activated.
        /// Указывает, сколько времени экран занимает переход на когда он активирован.
        /// </summary>
        public TimeSpan TransitionOnTime { get; protected set; }

        /// <summary>
        /// Indicates how long the screen takes to
        /// transition off when it is deactivated.
        /// Указывает, сколько времени экран занимает переход отключен, когда он деактивирован.
        /// </summary>
        public TimeSpan TransitionOffTime { get; protected set; }

        /// <summary>
        /// Gets the current position of the screen transition, ranging
        /// from zero (fully active, no transition) to one (transitioned
        /// fully off to nothing).
        /// Получает текущую позицию перехода экрана, начиная от нуля (полностью активен, без перехода) до единицы (с переходом полностью отключен ни к чему).
        /// </summary>
        public float TransitionPosition { get; protected set; }

        /// <summary>
        /// Gets the current alpha of the screen transition, ranging
        /// from 1 (fully active, no transition) to 0 (transitioned
        /// fully off to nothing).
        /// Получает текущую альфа перехода экрана, начиная от 1 (полностью активен, без перехода) до 0 (с переходом полностью отключен ни к чему).
        /// </summary>
        public float TransitionAlpha
        {
            get { return 1f - TransitionPosition; }
        }

        /// <summary>
        /// Gets the current screen transition state.
        /// Получает текущее состояние перехода экрана.
        /// </summary>
        public ScreenState ScreenState { get; protected set; }

        /// <summary>
        /// There are two possible reasons why a screen might be transitioning
        /// off. It could be temporarily going away to make room for another
        /// screen that is on top of it, or it could be going away for good.
        /// This property indicates whether the screen is exiting for real:
        /// if set, the screen will automatically remove itself as soon as the
        /// transition finishes.
        /// Есть две возможные причины перехода экрана выкл.
        /// Это может временно уйти, чтобы освободить место для другого экран, который находится сверху, или он может исчезнуть навсегда.
        /// Это свойство указывает, действительно ли экран выходит из режима реального времени:
        /// если установлено, экран автоматически удалится, как только переход заканчивается.
        /// </summary>
        public bool IsExiting { get; protected internal set; }

        /// <summary>
        /// Checks whether this screen is active and can respond to user input.
        /// Проверяет, активен ли этот экран и может ли он реагировать на ввод пользователя.
        /// </summary>
        public bool IsActive { get { return !_otherScreenHasFocus && (ScreenState == ScreenState.TransitionOn || ScreenState == ScreenState.Active); } }

        /// <summary>
        /// Gets the manager that this screen belongs to.
        /// Получает менеджера, которому принадлежит этот экран.
        /// </summary>
        public ScreenManager ScreenManager { get; internal set; }

        /// <summary>
        /// Gets the gestures the screen is interested in. Screens should be as specific
        /// as possible with gestures to increase the accuracy of the gesture engine.
        /// For example, most menus only need Tap or perhaps Tap and VerticalDrag to operate.
        /// These gestures are handled by the ScreenManager when screens change and
        /// all gestures are placed in the InputState passed to the HandleInput method.
        /// Получает жесты, которыми интересуется экран. Экраны должны быть конкретными по возможности с помощью жестов, чтобы повысить точность движка жестов.
        /// Например, большинству меню нужны только Tap или, возможно, Tap и VerticalDrag для работы.
        /// Эти жесты обрабатываются ScreenManager при смене экранов и все жесты помещаются в InputState, переданный методу HandleInput.
        /// </summary>
        public GestureType EnabledGestures
        {
            get { return _enabledGestures; }
            protected set
            {
                _enabledGestures = value;

                // the screen manager handles this during screen changes, but
                // if this screen is active and the gesture types are changing,
                // we have to update the TouchPanel ourself.
                //менеджер экрана обрабатывает это во время смены экрана, но если этот экран активен и типы жестов меняются, мы должны обновить TouchPanel самостоятельно.
                if (ScreenState == ScreenState.Active)
                    TouchPanel.EnabledGestures = value;
            }
        }

        /// <summary>
        /// Load graphics content for the screen.
        /// Загрузить графический контент для экрана
        /// </summary>
        public virtual void LoadContent()
        {
        }

        /// <summary>
        /// Unload content for the screen.
        /// </summary>
        public virtual void UnloadContent()
        {
        }

        /// <summary>
        /// Allows the screen to run logic, such as updating the transition position.
        /// Unlike HandleInput, this method is called regardless of whether the screen
        /// is active, hidden, or in the middle of a transition.
        /// Позволяет экрану запускать логику, например, обновлять позицию перехода.
        /// В отличие от HandleInput, этот метод вызывается независимо от того, является ли экран активен, скрыт или находится в середине перехода.
        /// </summary>
        public virtual void Update(GameTime gameTime, bool otherScreenHasFocus, bool coveredByOtherScreen)
        {
            _otherScreenHasFocus = otherScreenHasFocus;

            if (IsExiting)
            {
                // If the screen is going away to die, it should transition off.
                ScreenState = ScreenState.TransitionOff;

                // When the transition finishes, remove the screen.
                if (!UpdateTransition(gameTime, TransitionOffTime, 1))
                    ScreenManager.RemoveScreen(this);
            }
            else if (coveredByOtherScreen)
            {
                // If the screen is covered by another, it should transition off.
                ScreenState = UpdateTransition(gameTime, TransitionOffTime, 1) ? ScreenState.TransitionOff : ScreenState.Hidden;
            }
            else
            {
                // Otherwise the screen should transition on and become active.
                ScreenState = UpdateTransition(gameTime, TransitionOnTime, -1) ? ScreenState.TransitionOn : ScreenState.Active;
            }
        }

        /// <summary>
        /// Helper for updating the screen transition position.
        /// Помощник для обновления положения экрана перехода.
        /// </summary>
        private bool UpdateTransition(GameTime gameTime, TimeSpan time, int direction)
        {
            // How much should we move by?
            //Сколько мы должны пройти?
            float transitionDelta;

            if (time == TimeSpan.Zero)
                transitionDelta = 1f;
            else
                transitionDelta = (float)(gameTime.ElapsedGameTime.TotalSeconds / time.TotalSeconds);

            // Update the transition position.
            //Обновить позицию перехода.
            TransitionPosition += transitionDelta * direction;

            // Did we reach the end of the transition?
            //Достигли ли мы конца перехода?
            if (((direction < 0) && (TransitionPosition <= 0)) || ((direction > 0) && (TransitionPosition >= 1)))
            {
                TransitionPosition = MathHelper.Clamp(TransitionPosition, 0, 1);
                return false;
            }

            // Otherwise we are still busy transitioning.
            //В противном случае мы все еще заняты переходом.
            return true;
        }

        /// <summary>
        /// Allows the screen to handle user input. Unlike Update, this method
        /// is only called when the screen is active, and not when some other
        /// screen has taken the focus.
        /// Позволяет экран обрабатывать пользовательский ввод. В отличие от обновления, этот метод вызывается только когда экран активен, а не когда какой-то другой
        /// экран взял фокус.
        /// </summary>
        public virtual void HandleInput(InputHelper input, GameTime gameTime)
        {
        }

        /// <summary>
        /// This is called when the screen should draw rendertargets.
        /// Это вызывается, когда экран должен рисовать цели визуализации.
        /// </summary>
        public virtual void PreDraw(GameTime gameTime)
        {
        }

        /// <summary>
        /// This is called when the screen should draw itself.
        /// Это вызывается, когда экран должен нарисовать сам.
        /// </summary>
        public virtual void Draw(GameTime gameTime)
        {
        }

        /// <summary>
        /// Tells the screen to go away. Unlike ScreenManager.RemoveScreen, which
        /// instantly kills the screen, this method respects the transition timings
        /// and will give the screen a chance to gradually transition off.
        /// Говорит экрану уйти. В отличие от ScreenManager.RemoveScreen, который мгновенно убивает экран, этот метод учитывает время перехода 
        /// и даст экрану возможность постепенно переходить.
        /// </summary>
        public void ExitScreen()
        {
            if (TransitionOffTime == TimeSpan.Zero)
            {
                // If the screen has a zero transition time, remove it immediately.
                //Если экран имеет нулевое время перехода, немедленно удалите его.
                ScreenManager.RemoveScreen(this);
            }
            else
            {
                // Otherwise flag that it should transition off and then exit.
                //В противном случае отметьте, что он должен выключиться, а затем выйти
                IsExiting = true;
            }
        }
    }
}