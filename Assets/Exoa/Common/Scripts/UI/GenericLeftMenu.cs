using DG.Tweening;
using UnityEngine;
using UnityEngine.UI;

namespace Exoa.Designer
{
    public class GenericLeftMenu : MonoBehaviour
    {

        public Button bgButton;
        public Button openButton;
        [HideInInspector]
        public bool isOpen;
        private RectTransform rt;
        public float closedX = -240f;
        public float openX = 240f;
        public float openSeed = 1f;
        public Ease ease;

        public virtual void Start()
        {
            rt = GetComponent<RectTransform>();
            Vector3 anchoredPosition = rt.anchoredPosition;
            anchoredPosition.x = closedX;
            rt.anchoredPosition = anchoredPosition;
            openButton.onClick.AddListener(OnClickOpen);
            bgButton?.onClick.AddListener(OnClickOpen);
            bgButton?.gameObject.SetActive(false);


            BuildMenu();
            //LevelEditorController.instance.onLevelLoaded.AddListener(OnLevelLoaded);

        }

        private void OnLevelLoaded()
        {
            //BuildMenu();
        }

        public virtual void BuildMenu()
        {

        }

        private void OnClickOpen()
        {
            isOpen = !isOpen;
            Open(!isOpen);


        }
        virtual public void Close()
        {
            Open(true);
        }
        virtual public void Open(bool close)
        {
            rt.DOAnchorPosX(close ? closedX : openX, openSeed).SetEase(ease);
            isOpen = !close;
            bgButton.gameObject.SetActive(true);
            bgButton.GetComponent<Image>().DOColor(new Color(0, 0, 0, close ? 0 : .4f), openSeed).OnComplete(() =>
            {
                bgButton.gameObject.SetActive(isOpen);
            });
            transform.SetSiblingIndex(transform.parent.childCount - 1);

        }
    }
}
