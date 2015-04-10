﻿using InterXLib.Patterns.MVC;
using UltimaXNA.Core.Patterns.IoC;

namespace UltimaXNA.Ultima.Login
{
    class LoginModel : AUltimaModel
    {
        private States.SceneManager m_SceneManager;

        public LoginModel(IContainer container)
            : base(container)
        {
            
        }

        protected override AView CreateView()
        {
            return new LoginView(this);
        }

        protected override void OnInitialize()
        {
            Engine.UserInterface.Cursor = new UI.UltimaCursor();
            m_SceneManager = new States.SceneManager(Container);
            m_SceneManager.ResetToLoginScreen();
        }

        protected override void OnDispose()
        {
            Engine.UserInterface.Reset();
            m_SceneManager.CurrentScene = null;
            m_SceneManager = null;
        }

        public override void Update(double totalTime, double frameTime)
        {
            m_SceneManager.Update(totalTime, frameTime);
        }
    }
}
