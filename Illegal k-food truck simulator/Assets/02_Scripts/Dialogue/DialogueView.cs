using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

namespace Dialogue
{
    /// <summary>
    /// 다이얼로그 UI 뷰 계층 / Dialogue UI View Layer
    /// UI 바인딩 및 선택지 버튼 동적 생성 담당
    /// </summary>
    public class DialogueView : MonoBehaviour
    {
        [Header("초상화 / Portraits")]
        [SerializeField] private Image leftPortraitImage;     // 왼쪽 초상화 (Player용)
        [SerializeField] private Image rightPortraitImage;    // 오른쪽 초상화 (NPC용)
        [SerializeField] private Image centerPortraitImage;   // 중앙 초상화 (System용)

        [Header("텍스트 UI / Text UI")]
        [SerializeField] private TMP_Text nameText;           // 화자 이름
        [SerializeField] private TMP_Text contentText;        // 대화 내용

        [Header("선택지 UI / Choice UI")]
        [SerializeField] private Transform choicesRoot;       // 선택지 버튼들의 부모
        [SerializeField] private Button choiceButtonPrefab;   // 선택지 버튼 프리팹

        // 현재 생성된 선택지 버튼들 / Currently generated choice buttons
        private List<Button> currentChoiceButtons = new List<Button>();

        /// <summary>
        /// 다이얼로그 라인을 화면에 렌더링 / Render dialogue line to screen
        /// </summary>
        /// <param name="line">렌더링할 다이얼로그 라인</param>
        public void RenderLine(DialogueLine line)
        {
            if (line == null)
            {
                Debug.LogWarning("DialogueLine is null");
                return;
            }

            // 텍스트 바인딩 / Text binding
            if (nameText != null)
                nameText.text = line.speakerName ?? "";

            if (contentText != null)
                contentText.text = line.content ?? "";

            // 화자 유형에 따른 초상화 설정 / Set portrait based on speaker type
            SetPortraitImage(line.speakerType, line.speakerImage);
        }

        /// <summary>
        /// 화자 유형에 따라 초상화 이미지 설정 / Set portrait image based on speaker type
        /// </summary>
        /// <param name="speakerType">화자 유형</param>
        /// <param name="speakerImage">화자 이미지</param>
        private void SetPortraitImage(SpeakerType speakerType, Sprite speakerImage)
        {
            // 모든 초상화 숨김 / Hide all portraits
            if (leftPortraitImage != null) leftPortraitImage.gameObject.SetActive(false);
            if (rightPortraitImage != null) rightPortraitImage.gameObject.SetActive(false);
            if (centerPortraitImage != null) centerPortraitImage.gameObject.SetActive(false);

            // 화자 유형에 따른 초상화 설정 / Set portrait based on speaker type
            Image targetPortrait = null;
            switch (speakerType)
            {
                case SpeakerType.Player:
                    targetPortrait = leftPortraitImage;
                    break;
                case SpeakerType.NPC:
                    targetPortrait = rightPortraitImage;
                    break;
                case SpeakerType.System:
                    targetPortrait = centerPortraitImage;
                    break;
            }

            if (targetPortrait != null && speakerImage != null)
            {
                targetPortrait.sprite = speakerImage;
                targetPortrait.gameObject.SetActive(true);
            }
        }

        /// <summary>
        /// 선택지들을 표시 / Show choices
        /// </summary>
        /// <param name="choices">선택지 리스트 (텍스트, 다음ID)</param>
        /// <param name="onPicked">선택지 선택 콜백</param>
        public void ShowChoices(List<(string text, int nextId)> choices, Action<int> onPicked)
        {
            // 기존 선택지 버튼 제거 / Clear existing choice buttons
            ClearChoices();

            if (choices == null || choices.Count == 0)
            {
                Debug.LogWarning("No choices to show");
                return;
            }

            if (choiceButtonPrefab == null || choicesRoot == null)
            {
                Debug.LogError("Choice button prefab or choices root is not assigned");
                return;
            }

            // 선택지 루트 먼저 활성화 / Activate choices root first
            choicesRoot.gameObject.SetActive(true);

            // 새 선택지 버튼 생성 / Create new choice buttons
            foreach (var choice in choices)
            {
                Button choiceButton = Instantiate(choiceButtonPrefab, choicesRoot);
                
                // 버튼 텍스트 설정 / Set button text
                TMP_Text buttonText = choiceButton.GetComponentInChildren<TMP_Text>();
                if (buttonText != null)
                {
                    buttonText.text = choice.text;
                }
                else
                {
                    Debug.LogWarning("Choice button prefab doesn't have a TMP_Text component");
                }

                // 클릭 이벤트 설정 / Set click event
                int nextId = choice.nextId; // 클로저 문제 방지 / Prevent closure issue
                choiceButton.onClick.AddListener(() => onPicked?.Invoke(nextId));

                currentChoiceButtons.Add(choiceButton);

                // 버튼 활성화 / Activate button
                choiceButton.gameObject.SetActive(true);
            }

            // Layout을 강제로 갱신 / Force layout refresh
            Canvas.ForceUpdateCanvases();
        }

        /// <summary>
        /// 모든 선택지 버튼 제거 / Clear all choice buttons
        /// </summary>
        public void ClearChoices()
        {
            foreach (Button button in currentChoiceButtons)
            {
                if (button != null)
                {
                    button.onClick.RemoveAllListeners();
                    DestroyImmediate(button.gameObject);
                }
            }
            currentChoiceButtons.Clear();
        }

        /// <summary>
        /// 다이얼로그 뷰 초기화 / Initialize dialogue view
        /// </summary>
        public void Initialize()
        {
            ClearChoices();
            
            // 텍스트 초기화 / Initialize texts
            if (nameText != null) nameText.text = "";
            if (contentText != null) contentText.text = "";
            
            // 모든 초상화 초기화 / Initialize all portraits
            InitializePortrait(leftPortraitImage);
            InitializePortrait(rightPortraitImage);
            InitializePortrait(centerPortraitImage);
        }

        /// <summary>
        /// 초상화 초기화 헬퍼 메서드 / Portrait initialization helper method
        /// </summary>
        /// <param name="portraitImage">초기화할 초상화 이미지</param>
        private void InitializePortrait(Image portraitImage)
        {
            if (portraitImage != null)
            {
                portraitImage.sprite = null;
                portraitImage.gameObject.SetActive(false);
            }
        }

        /// <summary>
        /// 다이얼로그 뷰 숨김 / Hide dialogue view
        /// </summary>
        public void Hide()
        {
            gameObject.SetActive(false);
        }

        /// <summary>
        /// 다이얼로그 뷰 표시 / Show dialogue view
        /// </summary>
        public void Show()
        {
            gameObject.SetActive(true);
        }
    }
}
