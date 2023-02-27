using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace GercStudio.DragRacingFramework
{
    public class DemoInfo : MonoBehaviour
    {
        public GameObject mainCanvas;
        public GameObject firstBoard;
        public GameObject secondBoard;

        public Button resume;

        [HideInInspector] public MenuManager menuManager;

        private bool showMenu;
        private bool canShowMenu;

        void Start()
        {
            menuManager = FindObjectOfType<MenuManager>();

            canShowMenu = CheckPlayerPrefs("ShowMenu");

            if (!menuManager)
                return;

            if (canShowMenu)
            {
                if (mainCanvas)
                    mainCanvas.SetActive(true);

                if (firstBoard)
                    firstBoard.SetActive(true);

                if (secondBoard)
                    secondBoard.SetActive(true);

                if (resume)
                    resume.onClick.AddListener(OpenNextBoard);
                
                StartCoroutine(ShowInfoTimer());
            }
            else
            {
                if (mainCanvas)
                    mainCanvas.SetActive(false);
            }
        }

        private void Update()
        {
            if (menuManager.openAnyMenu && mainCanvas.activeSelf)
            {
                StopAllCoroutines();

                if (mainCanvas)
                    mainCanvas.SetActive(false);
            }
        }

        void OpenNextBoard()
        {
            if (firstBoard)
                firstBoard.SetActive(false);

            if (secondBoard)
                secondBoard.SetActive(true);

            StartCoroutine(ShowInfoTimer());
        }

        bool CheckPlayerPrefs(string value)
        {
            return menuManager.gameAssets.valuesToSave.selectedCar == -1;

            // if (PlayerPrefs.HasKey(value))
            // {
            //     if (PlayerPrefs.GetInt(value) == 0)
            //     {
            //         PlayerPrefs.SetInt(value, 1);
            //         return true;
            //     }
            //
            //     return false;
            // }
            //
            // PlayerPrefs.SetInt(value, 1);
        }

        IEnumerator ShowInfoTimer()
        {
            yield return new WaitForSeconds(7);

            if (mainCanvas)
                mainCanvas.SetActive(false);
        }
    }
}
