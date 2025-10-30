using System.Collections.Generic;
using UnityEngine;

namespace Dialogue
{
    /// <summary>
    /// 선택지 문자열 파서 / Choice string parser
    /// "텍스트|다음ID;텍스트|다음ID" 형식을 파싱
    /// </summary>
    public static class ChoiceParser
    {
        /// <summary>
        /// 선택지 원시 데이터를 파싱하여 선택지 리스트 반환 / Parse raw choice data to choice list
        /// </summary>
        /// <param name="raw">원시 선택지 데이터 (예: "예|102;아니요|201")</param>
        /// <returns>선택지 리스트 (텍스트, 다음ID)</returns>
        public static List<(string text, int nextId)> Parse(string raw)
        {
            List<(string text, int nextId)> choices = new List<(string text, int nextId)>();

            // 공백 또는 빈 문자열 안전 처리 / Safe handling of empty or whitespace strings
            if (string.IsNullOrWhiteSpace(raw))
            {
                return choices;
            }

            // 세미콜론으로 개별 선택지 분리 / Split individual choices by semicolon
            string[] choiceParts = raw.Split(';');

            foreach (string choicePart in choiceParts)
            {
                string trimmedChoice = choicePart.Trim();
                if (string.IsNullOrEmpty(trimmedChoice)) continue;

                // 파이프로 텍스트와 ID 분리 / Split text and ID by pipe
                string[] parts = trimmedChoice.Split('|');
                
                if (parts.Length == 2)
                {
                    string choiceText = parts[0].Trim();
                    string nextIdStr = parts[1].Trim();

                    // ID 파싱 / Parse ID
                    if (int.TryParse(nextIdStr, out int nextId))
                    {
                        if (!string.IsNullOrEmpty(choiceText))
                        {
                            choices.Add((choiceText, nextId));
                        }
                    }
                    else
                    {
                        Debug.LogWarning($"Invalid choice ID format: {nextIdStr} in choice: {trimmedChoice}");
                    }
                }
                else
                {
                    Debug.LogWarning($"Invalid choice format (expected 'text|id'): {trimmedChoice}");
                }
            }

            return choices;
        }

        /// <summary>
        /// 선택지 유효성 검사 / Validate choices
        /// </summary>
        /// <param name="choices">선택지 리스트</param>
        /// <returns>유효한 선택지가 있는지 여부</returns>
        public static bool HasValidChoices(List<(string text, int nextId)> choices)
        {
            return choices != null && choices.Count > 0;
        }
    }
}
