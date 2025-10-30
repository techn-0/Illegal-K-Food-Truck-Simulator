using UnityEngine;

namespace Dialogue
{
    /// <summary>
    /// 화자 유형 / Speaker Type
    /// </summary>
    public enum SpeakerType 
    { 
        Player,     // 플레이어
        NPC,        // NPC
        System      // 시스템
    }

    /// <summary>
    /// 다이얼로그 라인 데이터 / Dialogue Line Data
    /// CSV에서 로드되는 개별 대화 라인 정보
    /// </summary>
    [System.Serializable]
    public class DialogueLine
    {
        public int id;                              // 고유 ID
        public string speakerName;                  // 화자 이름
        public string content;                      // 대화 내용
        public SpeakerType speakerType;            // 화자 유형
        public UnityEngine.Sprite speakerImage;    // 화자 초상화 (리소스 로딩 또는 주소 가능)

        // --- 분기 최소 필드 / Branching minimum fields ---
        public bool isChoice;                      // 선택지 유무
        public string choicesRaw;                  // 선택지 원시 데이터 "텍스트|다음ID;텍스트|다음ID"
        public int nextId;                         // 기본 직진 목적지 (선택지 없을 때만 사용)
    }
}
