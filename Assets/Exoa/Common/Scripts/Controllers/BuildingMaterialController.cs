using Exoa.Designer;
using Exoa.Events;
using System;
using UnityEngine;
using static Exoa.Designer.DataModel;

namespace Exoa.Designer
{
    public class BuildingMaterialController : MonoBehaviour
    {
        public MeshRenderer exteriorWalls;
        public MeshRenderer roof;
        public MeshCollider exteriorWallsCol;
        public MeshCollider roofCol;

        private string exteriorWallMatName;
        private string roofMatName;

        private bool areWallsDisplayed = true;
        private bool areRoofsDisplayed = true;

        void OnDestroy()
        {
            GameEditorEvents.OnRequestButtonAction -= OnRequestButtonAction;
            GameEditorEvents.OnRenderForScreenshot -= OnRenderForScreenshot;
            AppController.OnAppStateChange -= OnAppStateChange;
        }

        void Start()
        {
            GameEditorEvents.OnRequestButtonAction += OnRequestButtonAction;
            GameEditorEvents.OnRenderForScreenshot += OnRenderForScreenshot;
            AppController.OnAppStateChange += OnAppStateChange;

            bool showRoofAndWalls = AppController.Instance.State == AppController.States.PreviewBuilding ||
                                    AppController.Instance.State == AppController.States.PlayMode;
            ShowExteriorWalls(showRoofAndWalls);
            ShowRoof(showRoofAndWalls);
        }

        private void OnAppStateChange(AppController.States state)
        {
            if (state == AppController.States.PreviewBuilding)
            {
                ShowRoof(true);
                ShowExteriorWalls(true);
            }
            if (state == AppController.States.Draw)
            {
                ShowRoof(false);
            }
        }

        private void OnRequestButtonAction(GameEditorEvents.Action action, bool active)
        {
            switch (action)
            {
                case GameEditorEvents.Action.ToggleExteriorWalls: ToggleExteriorWalls(); break;
                case GameEditorEvents.Action.ShowExteriorWalls: ShowExteriorWalls(active); break;
                case GameEditorEvents.Action.ToggleRoof: ToggleRoof(); break;
                case GameEditorEvents.Action.ShowRoof: ShowRoof(active); break;
            }
        }

        private bool exteriorWallsEnabledBeforeScreenshot;
        private bool roofEnabledBeforeScreenshot;
        private void OnRenderForScreenshot(bool preRender)
        {
            HDLogger.Log("OnRenderForScreenshot preRender:" + preRender, HDLogger.LogCategory.Screenshot);
            if (preRender)
            {
                exteriorWallsEnabledBeforeScreenshot = exteriorWalls.enabled;
                roofEnabledBeforeScreenshot = roof.enabled;

                exteriorWalls.enabled = true;
                roof.enabled = false;
            }
            else
            {
                exteriorWalls.enabled = exteriorWallsEnabledBeforeScreenshot;
                roof.enabled = roofEnabledBeforeScreenshot;
            }

        }
        void Update()
        {
            if (Inputs.ToggleExteriorWalls())
            {
                ToggleExteriorWalls();
            }
            if (Inputs.ToggleRoof())
            {
                ToggleRoof();
            }
        }

        private void ToggleExteriorWalls()
        {
            areWallsDisplayed = !areWallsDisplayed;
            ShowExteriorWalls(areWallsDisplayed);
        }
        private void ShowExteriorWalls(bool show)
        {
            HDLogger.Log("Show Ext Walls show:" + show, HDLogger.LogCategory.Building);
            areWallsDisplayed = show;
            exteriorWalls.enabled = areWallsDisplayed;
            if (exteriorWallsCol != null)
                exteriorWallsCol.enabled = areWallsDisplayed;
        }

        private void ToggleRoof()
        {
            areRoofsDisplayed = !areRoofsDisplayed;
            ShowRoof(areRoofsDisplayed);
        }
        private void ShowRoof(bool active)
        {
            HDLogger.Log("Show Roof show:" + active, HDLogger.LogCategory.Building);
            areRoofsDisplayed = active;
            roof.enabled = areRoofsDisplayed;
            if (roofCol != null) roofCol.enabled = areRoofsDisplayed;
        }

        public void ApplyExteriorWallMaterial(Material mat)
        {
            exteriorWalls.material = mat;
            exteriorWallMatName = mat.name;
        }

        public void ApplyRoofMaterial(Material mat)
        {
            roof.material = mat;
            roofMatName = mat.name;
        }

#if INTERIOR_MODULE
        public BuildingSetting GetBuildingSetting()
        {
            BuildingSetting rs = new BuildingSetting();
            rs.exteriorWallMat = exteriorWallMatName;
            rs.roofMat = roofMatName;
            return rs;
        }

        public void SetBuildingSetting(BuildingSetting bs)
        {
            //print("SetBuildingSetting " + exteriorWallMat);

            if (!string.IsNullOrEmpty(bs.exteriorWallMat))
            {
                Material m = Resources.Load<Material>(HDSettings.EXTERIOR_WALL_MATERIALS_FOLDER + bs.exteriorWallMat);
                if (m != null) ApplyExteriorWallMaterial(m);
            }
            if (!string.IsNullOrEmpty(bs.roofMat))
            {
                Material m = Resources.Load<Material>(HDSettings.ROOF_MATERIALS_FOLDER + bs.roofMat);
                if (m != null) ApplyRoofMaterial(m);
            }
        }
#else
    public object GetBuildingSetting()
    {
        return null;
    }

    public void SetBuildingSetting(object bs)
    {

    }
#endif
    }
}
