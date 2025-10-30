using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Events;

namespace Dialogue
{
    /// <summary>
    /// 다이얼로그 매니저 / Dialogue Manager
    /// 다이얼로그 진행 제어, 노드 전환, 입력 처리 담당
    /// </summary>
    public class DialogueManager : MonoBehaviour
    {
        [Header("다이얼로그 설정 / Dialogue Settings")]
        [SerializeField] private TextAsset csv;                    // CSV 다이얼로그 데이터
        [SerializeField] private DialogueView view;                // 다이얼로그 뷰 참조

        [Header("이벤트 / Events")]
        [SerializeField] private UnityEvent OnDialogueEnd;         // 다이얼로그 종료 이벤트

        // 내부 데이터 / Internal data
        private List<DialogueLine> dialogueLines;                 // 로드된 다이얼로그 라인들
        private Dictionary<int, DialogueLine> dialogueDictionary; // ID로 빠른 검색용
        private DialogueLine currentLine;                         // 현재 표시 중인 라인
        private bool isDialogueActive = false;                    // 다이얼로그 활성 상태

        /// <summary>
        /// 초기화 / Initialize
        /// </summary>
        private void Awake()
        {
            // CSV 데이터 로드 / Load CSV data
            LoadDialogueData();
            
            // 뷰 초기화 / Initialize view
            if (view != null)
            {
                view.Initialize();
                view.Hide();
            }
        }

        /// <summary>
        /// 업데이트 - 입력 처리 / Update - Input handling
        /// </summary>
        private void Update()
        {
            if (!isDialogueActive) return;

            // 선택지가 없을 때만 Space/LeftMouse로 진행 가능 / Progress with Space/LeftMouse only when no choices
            if (currentLine != null && !currentLine.isChoice)
            {
                if (Input.GetKeyDown(KeyCode.Space) || Input.GetMouseButtonDown(0))
                {
                    OnNextButtonClicked();
                }
            }
        }

        /// <summary>
        /// 다이얼로그 시작 / Start dialogue
        /// </summary>
        /// <param name="startId">시작할 다이얼로그 ID</param>
        public void StartDialogue(int startId)
        {
            if (dialogueDictionary == null || dialogueDictionary.Count == 0)
            {
                Debug.LogError("No dialogue data loaded. Cannot start dialogue.");
                return;
            }

            if (view == null)
            {
                Debug.LogError("DialogueView is not assigned. Cannot start dialogue.");
                return;
            }

            if (!TryGetLine(startId, out DialogueLine startLine))
            {
                Debug.LogError($"Cannot find dialogue line with ID: {startId}");
                return;
            }

            isDialogueActive = true;
            view.Show();
            ShowDialogueLine(startLine);
        }

        /// <summary>
        /// 다이얼로그 라인 표시 / Show dialogue line
        /// </summary>
        /// <param name="line">표시할 라인</param>
        private void ShowDialogueLine(DialogueLine line)
        {
            if (line == null) return;

            currentLine = line;
            
            // 뷰에 라인 렌더링 / Render line to view
            view.RenderLine(line);

            // 선택지 처리 / Handle choices
            if (line.isChoice)
            {
                var choices = ChoiceParser.Parse(line.choicesRaw);
                if (ChoiceParser.HasValidChoices(choices))
                {
                    view.ShowChoices(choices, OnChoiceSelected);
                }
                else
                {
                    Debug.LogWarning($"Line {line.id} marked as choice but has no valid choices. Treating as regular line.");
                    // 선택지가 유효하지 않으면 일반 라인으로 처리
                    view.ClearChoices();
                }
            }
            else
            {
                view.ClearChoices();
            }
        }

        /// <summary>
        /// 선택지 선택 처리 / Handle choice selection
        /// </summary>
        /// <param name="nextId">선택된 다음 ID</param>
        private void OnChoiceSelected(int nextId)
        {
            if (nextId <= 0)
            {
                EndDialogue();
                return;
            }

            if (TryGetLine(nextId, out DialogueLine nextLine))
            {
                ShowDialogueLine(nextLine);
            }
            else
            {
                Debug.LogError($"Cannot find dialogue line with ID: {nextId}");
                EndDialogue();
            }
        }

        /// <summary>
        /// 다음 버튼 클릭 처리 / Handle next button click
        /// </summary>
        private void OnNextButtonClicked()
        {
            if (currentLine == null) return;

            // 선택지가 있는 라인에서는 다음 버튼 비활성화
            if (currentLine.isChoice) return;

            // 다음 라인으로 이동 또는 종료
            if (currentLine.nextId <= 0)
            {
                EndDialogue();
            }
            else
            {
                if (TryGetLine(currentLine.nextId, out DialogueLine nextLine))
                {
                    ShowDialogueLine(nextLine);
                }
                else
                {
                    Debug.LogError($"Cannot find dialogue line with ID: {currentLine.nextId}");
                    EndDialogue();
                }
            }
        }

        /// <summary>
        /// 다이얼로그 종료 / End dialogue
        /// </summary>
        private void EndDialogue()
        {
            isDialogueActive = false;
            currentLine = null;
            
            if (view != null)
            {
                view.ClearChoices();
                view.Hide();
            }

            // 종료 이벤트 발생 / Trigger end event
            OnDialogueEnd?.Invoke();
            
            Debug.Log("Dialogue ended");
        }

        /// <summary>
        /// ID로 다이얼로그 라인 검색 / Find dialogue line by ID
        /// </summary>
        /// <param name="id">찾을 라인 ID</param>
        /// <param name="line">찾은 라인 (out)</param>
        /// <returns>찾기 성공 여부</returns>
        private bool TryGetLine(int id, out DialogueLine line)
        {
            line = null;
            
            if (dialogueDictionary == null)
                return false;

            return dialogueDictionary.TryGetValue(id, out line);
        }

        /// <summary>
        /// CSV 다이얼로그 데이터 로드 / Load CSV dialogue data
        /// </summary>
        private void LoadDialogueData()
        {
            if (csv == null)
            {
                Debug.LogError("CSV TextAsset is not assigned");
                return;
            }

            // CSV 로드 / Load CSV
            dialogueLines = CSVLoader.LoadDialogue(csv);
            
            // 딕셔너리 생성 (ID로 빠른 검색용) / Create dictionary for fast ID lookup
            dialogueDictionary = new Dictionary<int, DialogueLine>();
            
            foreach (var line in dialogueLines)
            {
                if (dialogueDictionary.ContainsKey(line.id))
                {
                    Debug.LogWarning($"Duplicate dialogue ID found: {line.id}. Skipping duplicate.");
                    continue;
                }
                
                dialogueDictionary[line.id] = line;
            }

            Debug.Log($"Loaded {dialogueLines.Count} dialogue lines from CSV");
        }

        /// <summary>
        /// 다이얼로그 활성 상태 확인 / Check if dialogue is active
        /// </summary>
        /// <returns>다이얼로그 활성 여부</returns>
        public bool IsDialogueActive()
        {
            return isDialogueActive;
        }

        /// <summary>
        /// 다이얼로그 강제 종료 / Force end dialogue
        /// </summary>
        public void ForceEndDialogue()
        {
            EndDialogue();
        }

        /// <summary>
        /// 에디터에서 테스트용 / For testing in editor
        /// </summary>
        [ContextMenu("Test Start Dialogue (ID: 100)")]
        private void TestStartDialogue()
        {
            if (Application.isPlaying)
            {
                StartDialogue(100);
            }
        }

        /// <summary>
        /// 에디터에서 데이터 리로드용 / For reloading data in editor
        /// </summary>
        [ContextMenu("Reload CSV Data")]
        private void ReloadCSVData()
        {
            LoadDialogueData();
        }
    }
}
