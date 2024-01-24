using DG.Tweening;
using UnityEngine;
using UnityEngine.UI;

namespace Exoa.Designer
{
    public class BaseStaticPopup : MonoBehaviour
    {
        public GameObject contentGo;
        public GameObject bgGo;
        protected Button bgBtn;
        public Button closeBtn;
        protected Image bgImg;
        public bool clickBgToClosePopup;
        public Color openBgColor = new Color(0, 0, 0, .4f);
        public Color closeBgColor = new Color(0, 0, 0, 0);

        protected bool shown;

        virtual protected void Awake()
        {

            if (bgGo != null)
            {
                bgImg = bgGo.GetComponent<Image>();
                bgBtn = bgGo.GetComponent<Button>();
                if (bgBtn != null && clickBgToClosePopup)
                    bgBtn.onClick.AddListener(Hide);
            }

            closeBtn?.onClick.AddListener(Hide);
            Hide();
        }

        public void Hide()
        {
            //HDLogger.Log("Popup Hide " + gameObject.name, HDLogger.LogCategory.General);
            contentGo.SetActive(false);
            if (bgBtn != null) bgBtn.enabled = false;
            if (bgImg != null) bgImg.raycastTarget = false;
            if (bgImg != null) bgImg.DOColor(closeBgColor, 1);
            shown = false;
        }
        public void Show()
        {
            //HDLogger.Log("Popup Show " + gameObject.name, HDLogger.LogCategory.General);
            //print("Show Popup");
            contentGo.SetActive(true);
            if (bgImg != null) bgImg.raycastTarget = true;
            if (bgImg != null) bgImg.DOColor(openBgColor, 1).OnComplete(() =>
            {
                if (bgBtn != null) bgBtn.enabled = true;
            });
            shown = true;
        }
        public void Show(bool v)
        {
            shown = v;
            if (shown) Show();
            else Hide();
        }
        public void Toggle()
        {
            shown = !shown;
            if (shown) Show();
            else Hide();
        }
    }
}
