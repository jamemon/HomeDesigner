using DG.Tweening;
using System;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace Exoa.Designer
{
    public class ModuleMenuController : MonoBehaviour
    {
        public static ModuleMenuController Instance;

        public GameObject menuItemPrefab;
        public RectTransform menuContainer;
        //public GameObject[] modulePrefabs;
        public float initialScale = 0.3f;
        public Vector3 initialOffset;
        public ModuleMenuScrollRect scrollRect;

        void Awake()
        {
            Instance = this;
        }

        public void Hide()
        {
            (transform as RectTransform).DOAnchorPosY(-140, .4f).SetEase(Ease.InCubic);
        }

        public void Show()
        {
            (transform as RectTransform).DOAnchorPosY(0, .4f).SetEase(Ease.OutCubic);
        }

        public void DisplayPrefabs(GameObject[] prefabs)
        {
            menuContainer.ClearChildren();
            //GameObject[] prefabs = (LevelController.instance != null ? LevelController.instance.prefabs : LevelEditorController.instance.prefabs);
            float width = 0;
            foreach (GameObject modulePrefab in prefabs)
            {
                GameObject btnInst = Instantiate(menuItemPrefab, menuContainer);
                RectTransform btnInstRect = btnInst.GetComponent<RectTransform>();
                ModuleMenuItem item = btnInst.GetComponent<ModuleMenuItem>();
                //Button btnInstButton = btnInst.GetComponent<Button>();
                RawImage btnInstImage = btnInst.GetComponentInChildren<RawImage>();
                btnInst.name = modulePrefab.name;
                btnInstRect.localScale = Vector3.one;
                btnInstRect.localPosition = Vector3.zero;

                item.OnSelectModule.AddListener(OnSelectModuleItem);
                item.OnExitItemZone.AddListener(OnExitItemZone);

                //btnInstButton.onClick.AddListener(() => OnClickMenuItem(modulePrefab.name));
                width = btnInstRect.rect.width;
                string thumbPath = HDSettings.MODULE_THUMBNAIL_FOLDER + modulePrefab.name;
                Texture2D t = Resources.Load<Texture2D>(thumbPath);
                if (t != null)
                {
                    btnInstImage.texture = t;
                }
                else
                {
                    Debug.LogError("Could not find thumbnail:" + thumbPath);
                }

            }



        }

        private void OnExitItemZone(ModuleMenuItem arg0, PointerEventData data)
        {
            //print("OnExitItemZone");
            //scrollRect.OnEndDrag(data);
            //scrollRect.StopMovement();
            //scrollRect.
        }

        private void OnSelectModuleItem(ModuleMenuItem item, PointerEventData data)
        {
            OnSelectModuleItem(item.name);
            //scrollRect.horizontalNormalizedPosition
            //scrollRect.StopMovement();
        }
        private void OnSelectModuleItem(string name)
        {
            //print("OnSelectModuleItem name:" + name + " InteriorDesigner.instance:" + InteriorDesigner.instance);
#if LEVEL_DESIGNER
			if (LevelDesigner.instance != null)
				LevelDesigner.instance.SelectPrefab(name);
#endif
#if INTERIOR_MODULE
            if (InteriorDesigner.instance != null)
                InteriorDesigner.instance.SelectPrefab(name);
#endif

        }

    }
}
