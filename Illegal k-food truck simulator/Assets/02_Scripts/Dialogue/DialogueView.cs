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
        [Header("패널 설정 / Panel Settings")]
        [SerializeField] private RectTransform leftPanel;     // Player용 (왼쪽)
        [SerializeField] private RectTransform rightPanel;    // NPC용 (오른쪽)
        [SerializeField] private RectTransform centerPanel;   // System용 (중앙)

        [Header("텍스트 UI / Text UI")]
        [SerializeField] private TMP_Text nameText;           // 화자 이름
        [SerializeField] private TMP_Text contentText;        // 대화 내용

        [Header("이미지 UI / Image UI")]
        [SerializeField] private Image portraitImage;         // 화자 초상화

        [Header("선택지 UI / Choice UI")]
        [SerializeField] private Transform choicesRoot;       // 선택지 버튼들의 부모
        [SerializeField] private Button choiceButtonPrefab;   // 선택지 버튼 프리팹

        [Header("진행 버튼 / Progress Button")]
        [SerializeField] private Button nextButton;           // 다음 버튼 (선택지 없을 때)

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

            // 패널 활성화/비활성화 및 앵커 설정 / Panel activation and anchor setting
            SetPanelVisibility(line.speakerType);

            // 텍스트 바인딩 / Text binding
            if (nameText != null)
                nameText.text = line.speakerName ?? "";

            if (contentText != null)
                contentText.text = line.content ?? "";

            // 초상화 바인딩 / Portrait binding
            if (portraitImage != null)
            {
                portraitImage.sprite = line.speakerImage;
                portraitImage.gameObject.SetActive(line.speakerImage != null);
            }

            // 다음 버튼 표시/숨김 (선택지가 없을 때만 표시) / Show/hide next button
            if (nextButton != null)
                nextButton.gameObject.SetActive(!line.isChoice);
        }

        /// <summary>
        /// 화자 유형에 따라 패널 표시 설정 / Set panel visibility based on speaker type
        /// </summary>
        /// <param name="speakerType">화자 유형</param>
        private void SetPanelVisibility(SpeakerType speakerType)
        {
            // 모든 패널 비활성화 / Deactivate all panels
            if (leftPanel != null) leftPanel.gameObject.SetActive(false);
            if (rightPanel != null) rightPanel.gameObject.SetActive(false);
            if (centerPanel != null) centerPanel.gameObject.SetActive(false);

            // 화자 유형에 따라 해당 패널 활성화 / Activate corresponding panel
            switch (speakerType)
            {
                case SpeakerType.Player:
                    if (leftPanel != null) leftPanel.gameObject.SetActive(true);
                    break;
                case SpeakerType.NPC:
                    if (rightPanel != null) rightPanel.gameObject.SetActive(true);
                    break;
                case SpeakerType.System:
                    if (centerPanel != null) centerPanel.gameObject.SetActive(true);
                    break;
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

                // 클릭 이벤트 설정 / Set click event
                int nextId = choice.nextId; // 클로저 문제 방지 / Prevent closure issue
                choiceButton.onClick.AddListener(() => onPicked?.Invoke(nextId));

                currentChoiceButtons.Add(choiceButton);
            }

            // 다음 버튼 숨김 (선택지가 있을 때) / Hide next button when choices are present
            if (nextButton != null)
                nextButton.gameObject.SetActive(false);
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
        /// 다음 버튼 클릭 이벤트 설정 / Set next button click event
        /// </summary>
        /// <param name="onNext">다음 버튼 클릭 콜백</param>
        public void SetNextButtonCallback(Action onNext)
        {
            if (nextButton != null)
            {
                nextButton.onClick.RemoveAllListeners();
                nextButton.onClick.AddListener(() => onNext?.Invoke());
            }
        }

        /// <summary>
        /// 다이얼로그 뷰 초기화 / Initialize dialogue view
        /// </summary>
        public void Initialize()
        {
            ClearChoices();
            
            // 모든 패널 비활성화 / Deactivate all panels
            SetPanelVisibility(SpeakerType.System);
            
            if (nameText != null) nameText.text = "";
            if (contentText != null) contentText.text = "";
            if (portraitImage != null) 
            {
                portraitImage.sprite = null;
                portraitImage.gameObject.SetActive(false);
            }
            if (nextButton != null) nextButton.gameObject.SetActive(false);
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
