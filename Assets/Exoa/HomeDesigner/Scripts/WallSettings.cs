using System;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace Exoa.Designer
{
    public class WallSettings : MonoBehaviour
    {
        public GameObject root;

        public RectTransform itemsContainer;
        public TMP_Text titleTxt;
        public GameObject itemPrefab;

        private List<Button> btnList;

        public enum Mode { Floor, InteriorWall, FloorAndInteriorWall, ExteriorWall, Ceiling, Roof, Module, Outside };
        private Mode currentMode;

        private MaterialPopup materialPopup;

        void Start()
        {


        }

        public void ShowMode(Mode mode)
        {
            materialPopup = GetComponentInParent<MaterialPopup>();

            itemsContainer.ClearChildren();
            currentMode = mode;


            string title = "";
            string folder = "";
            switch (mode)
            {
                case Mode.Floor:
                    title = "Floor Materials";
                    folder = HDSettings.FLOOR_MATERIALS_FOLDER;
                    break;
                case Mode.InteriorWall:
                    title = "Interior Wall Materials";
                    folder = HDSettings.WALL_MATERIALS_FOLDER;
                    break;
                case Mode.ExteriorWall:
                    title = "Exterior Wall Materials";
                    folder = HDSettings.EXTERIOR_WALL_MATERIALS_FOLDER;
                    break;
                case Mode.Ceiling:
                    title = "Ceiling Materials";
                    folder = HDSettings.CEILING_MATERIALS_FOLDER;
                    break;
                case Mode.Roof:
                    title = "Roof Materials";
                    folder = HDSettings.ROOF_MATERIALS_FOLDER;
                    break;
                case Mode.Outside:
                    title = "Outside Materials";
                    folder = HDSettings.OUTSIDE_MATERIALS_FOLDER;
                    break;
                case Mode.Module:
                    title = "Color Variants";
                    folder = null;
                    break;
            }
            titleTxt.text = title;
            btnList = new List<Button>();
            itemsContainer.ClearChildren();

            GameObject inst = null;
            RawImage img = null;
            Button btn = null;

            List<Material> mats = new List<Material>(); ;
            List<Color> colors = new List<Color>();

            if (folder != null)
            {
                mats.AddRange(Resources.LoadAll<Material>(folder));
            }
            else if (mode == Mode.Module && materialPopup.Module != null)
            {
                for (int i = 0; i < materialPopup.Module.variants.Count; i++)
                {
                    if (materialPopup.Module.type == ModuleColorVariants.Type.Materials &&
                        materialPopup.Module.variants[i].material != null)
                    {
                        mats.Add(materialPopup.Module.variants[i].material);
                    }
                    else if (materialPopup.Module.type == ModuleColorVariants.Type.Colors)
                    {
                        colors.Add(materialPopup.Module.variants[i].color);
                    }

                }
            }
            foreach (Material m in mats)
            {
                inst = Instantiate(itemPrefab, itemsContainer);
                img = inst.GetComponent<RawImage>();
                img.texture = GetDiffuseTexture(m);
                btn = inst.GetComponent<Button>();
                btn.onClick.AddListener(() => SelectMaterial(currentMode, m));

                btnList.Add(btn);
            }
            foreach (Color c in colors)
            {
                inst = Instantiate(itemPrefab, itemsContainer);
                img = inst.GetComponent<RawImage>();
                img.color = c;
                btn = inst.GetComponent<Button>();
                btn.onClick.AddListener(() => SelectMaterial(currentMode, c));

                btnList.Add(btn);
            }
            //print("btnList:" + btnList.Count + " mode:" + mode);
        }

        private void SelectMaterial(Mode mode, Color c)
        {
            switch (mode)
            {
                case Mode.Module: materialPopup.Module.ApplyModuleColor(c); break;
            }
        }

        private void SelectMaterial(Mode mode, Material m)
        {
            Material mat = new Material(m);
            switch (mode)
            {
                case Mode.Ceiling: materialPopup.Room.ApplyCeilingMaterial(mat); break;
                case Mode.Roof: materialPopup.Building.ApplyRoofMaterial(mat); break;
                case Mode.ExteriorWall: materialPopup.Building.ApplyExteriorWallMaterial(mat); break;
                case Mode.InteriorWall: materialPopup.Room.ApplyInteriorWallMaterial(mat); break;
                case Mode.Floor: materialPopup.Room.ApplyFloorMaterial(mat); break;
                case Mode.Outside: materialPopup.Room.ApplyFloorMaterial(mat); break;
                case Mode.Module: materialPopup.Module.ApplyModuleMaterial(mat); break;
            }

        }

        private Texture GetSpecTexture(Material m)
        {
            if (m.HasProperty("_SpecularTexture2D")) return m.GetTexture("_SpecularTexture2D");
            if (m.HasProperty("_Spec")) return m.GetTexture("_Spec");
            if (m.HasProperty("_SpecTex")) return m.GetTexture("_SpecTex");
            return null;
        }
        private Texture GetDiffuseTexture(Material m)
        {
            if (m.HasProperty("_DiffuseTexture2D")) return m.GetTexture("_DiffuseTexture2D");
            if (m.HasProperty("_Diffuse")) return m.GetTexture("_Diffuse");
            if (m.HasProperty("_MainTex")) return m.GetTexture("_MainTex");
            return null;
        }
        private Texture GetBumpTexture(Material m)
        {
            if (m.HasProperty("_NormalTexture2D")) return m.GetTexture("_NormalTexture2D");
            if (m.HasProperty("_Normal")) return m.GetTexture("_Normal");
            if (m.HasProperty("_NormalTex")) return m.GetTexture("_NormalTex");
            return null;
        }
    }
}
