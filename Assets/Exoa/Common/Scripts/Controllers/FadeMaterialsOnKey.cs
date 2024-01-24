using DG.Tweening;
using Exoa.Designer;
using Exoa.Events;
using System;
using System.Collections.Generic;
using UnityEngine;

namespace Exoa.Designer
{
    public class FadeMaterialsOnKey : MonoBehaviour
    {
        public List<Material> gizmos;
        private bool areGizmosDisplayed = true;

        void OnDestroy()
        {
            GameEditorEvents.OnRequestButtonAction -= OnRequestButtonAction;
            AppController.OnAppStateChange -= OnAppStateChange;
        }


        void Start()
        {
            GameEditorEvents.OnRequestButtonAction += OnRequestButtonAction;
            AppController.OnAppStateChange += OnAppStateChange;
            ShowGizmos(true);
        }



        private void OnAppStateChange(AppController.States state)
        {
            if (state == AppController.States.Draw)
            {
                ShowGizmos(true);
            }
        }

        private Material SetAlpha(Material m, float alpha)
        {
            Color c = m.color;
            c.a = alpha;
            m.color = c;
            return m;
        }

        private void OnRequestButtonAction(GameEditorEvents.Action action, bool active)
        {
            if (action == GameEditorEvents.Action.ToggleGizmos)
                ToggleGizmos();

        }

        void Update()
        {
            if (Inputs.ToggleGizmo())
            {
                ToggleGizmos();
            }
        }


        private void ToggleGizmos()
        {
            areGizmosDisplayed = !areGizmosDisplayed;
            ShowGizmos(areGizmosDisplayed);
        }

        private void ShowGizmos(bool show)
        {
            areGizmosDisplayed = show;
            gizmos.ForEach(m => m.DOFade(areGizmosDisplayed ? .8f : 0f, 1));
        }
    }
}
