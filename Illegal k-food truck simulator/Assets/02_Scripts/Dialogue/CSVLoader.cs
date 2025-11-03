using System.Collections.Generic;
using System.Text;
using UnityEngine;

namespace Dialogue
{
    /// <summary>
    /// CSV 파일을 DialogueLine 리스트로 변환하는 로더 / CSV to DialogueLine list converter
    /// </summary>
    public static class CSVLoader
    {
        /// <summary>
        /// TextAsset CSV를 받아 DialogueLine 리스트를 반환 / Convert TextAsset CSV to DialogueLine list
        /// </summary>
        /// <param name="csvAsset">CSV TextAsset</param>
        /// <returns>DialogueLine 리스트</returns>
        public static List<DialogueLine> LoadDialogue(TextAsset csvAsset)
        {
            List<DialogueLine> dialogueLines = new List<DialogueLine>();
            
            if (csvAsset == null)
            {
                Debug.LogWarning("CSV Asset is null");
                return dialogueLines;
            }

            // 따옴표를 고려하여 행을 분리 (따옴표 내부의 줄바꿈은 동일 행으로 처리)
            List<string> rows = SplitCsvRows(csvAsset.text);
            
            if (rows.Count <= 1)
            {
                Debug.LogWarning("CSV has no data rows");
                return dialogueLines;
            }

            // 첫 줄은 헤더이므로 스킵 / Skip header row
            for (int i = 1; i < rows.Count; i++)
            {
                string line = rows[i];
                if (string.IsNullOrWhiteSpace(line)) continue;

                DialogueLine dialogueLine = ParseCSVLine(line);
                if (dialogueLine != null)
                {
                    dialogueLines.Add(dialogueLine);
                }
            }

            return dialogueLines;
        }

        /// <summary>
        /// CSV 텍스트를 따옴표 인지 상태를 고려해서 행 단위로 분리 / Split CSV text into rows respecting quotes
        /// </summary>
        private static List<string> SplitCsvRows(string csvText)
        {
            List<string> rows = new List<string>();
            if (string.IsNullOrEmpty(csvText)) return rows;

            StringBuilder current = new StringBuilder();
            bool inQuotes = false;

            for (int i = 0; i < csvText.Length; i++)
            {
                char c = csvText[i];

                // CRLF 정규화: '\r'은 무시 (줄바꿈은 '\n'만 사용)
                if (c == '\r')
                    continue;

                if (c == '"')
                {
                    // 이스케이프 따옴표 처리 ("")
                    if (inQuotes && i + 1 < csvText.Length && csvText[i + 1] == '"')
                    {
                        current.Append('"');
                        i++; // 다음 따옴표 스킵
                    }
                    else
                    {
                        inQuotes = !inQuotes;
                        current.Append(c);
                    }
                }
                else if (c == '\n' && !inQuotes)
                {
                    rows.Add(current.ToString());
                    current.Clear();
                }
                else
                {
                    current.Append(c);
                }
            }

            // 마지막 행 추가
            if (current.Length > 0)
            {
                rows.Add(current.ToString());
            }

            return rows;
        }

        /// <summary>
        /// CSV 라인을 파싱하여 DialogueLine 객체 생성 / Parse CSV line to DialogueLine object
        /// </summary>
        /// <param name="csvLine">CSV 라인</param>
        /// <returns>DialogueLine 객체 또는 null</returns>
        private static DialogueLine ParseCSVLine(string csvLine)
        {
            List<string> fields = ParseCSVFields(csvLine);
            
            if (fields.Count < 8)
            {
                Debug.LogWarning($"CSV line has insufficient fields: {csvLine}");
                return null;
            }

            DialogueLine line = new DialogueLine();

            // 필드 파싱 / Parse fields
            line.id = ParseInt(fields[0]);
            line.speakerName = fields[1];
            line.content = fields[2];
            line.speakerType = ParseSpeakerType(fields[3]);
            line.speakerImage = LoadSpeakerImage(fields[4]);
            line.isChoice = ParseBool(fields[5]);
            line.choicesRaw = fields[6];
            line.nextId = ParseInt(fields[7]);

            return line;
        }

        /// <summary>
        /// CSV 라인을 필드별로 분리 (따옴표 처리 포함) / Split CSV line into fields (with quote handling)
        /// </summary>
        /// <param name="csvLine">CSV 라인</param>
        /// <returns>필드 리스트</returns>
        private static List<string> ParseCSVFields(string csvLine)
        {
            List<string> fields = new List<string>();
            StringBuilder currentField = new StringBuilder();
            bool inQuotes = false;

            for (int i = 0; i < csvLine.Length; i++)
            {
                char c = csvLine[i];

                if (c == '"')
                {
                    // 따옴표 내부의 이스케이프 따옴표 처리 ("")
                    if (inQuotes && i + 1 < csvLine.Length && csvLine[i + 1] == '"')
                    {
                        currentField.Append('"');
                        i++; // 다음 따옴표 스킵
                    }
                    else
                    {
                        inQuotes = !inQuotes;
                    }
                }
                else if (c == ',' && !inQuotes)
                {
                    fields.Add(currentField.ToString().Trim());
                    currentField.Clear();
                }
                else if (c == '\r')
                {
                    // CR는 무시 (행 분리는 SplitCsvRows에서 처리)
                    continue;
                }
                else
                {
                    currentField.Append(c);
                }
            }

            // 마지막 필드 추가 / Add last field
            fields.Add(currentField.ToString().Trim());

            return fields;
        }

        /// <summary>
        /// 문자열을 int로 변환 (실패 시 0) / Convert string to int (0 on failure)
        /// </summary>
        private static int ParseInt(string value)
        {
            if (int.TryParse(value, out int result))
                return result;
            return 0;
        }

        /// <summary>
        /// 문자열을 bool로 변환 (실패 시 false) / Convert string to bool (false on failure)
        /// </summary>
        private static bool ParseBool(string value)
        {
            return value != null && value.Trim().ToLower() == "true";
        }

        /// <summary>
        /// 문자열을 SpeakerType으로 변환 / Convert string to SpeakerType
        /// </summary>
        private static SpeakerType ParseSpeakerType(string value)
        {
            if (System.Enum.TryParse<SpeakerType>(value, true, out SpeakerType result))
                return result;
            return SpeakerType.System;
        }

        /// <summary>
        /// 화자 이미지를 03_Resource 폴더에서 로드 / Load speaker image from 03_Resource folder
        /// </summary>
        /// <param name="fileName">파일명</param>
        /// <returns>Sprite 또는 null</returns>
        private static Sprite LoadSpeakerImage(string fileName)
        {
            if (string.IsNullOrEmpty(fileName))
                return null;

#if UNITY_EDITOR
            // 03_Resource/Sprite/Portraits 경로에서 로드 (에디터 전용)
            string resourcePath = $"Assets/03_Resource/Sprite/Portraits/{fileName}.png";
            Sprite sprite = UnityEditor.AssetDatabase.LoadAssetAtPath<Sprite>(resourcePath);
            
            if (sprite == null)
            {
                Debug.LogWarning($"Could not load speaker image: {resourcePath}");
            }

            return sprite;
#else
            // 런타임 빌드에서는 Addressables/Resources 사용을 권장 (현재는 null 반환)
            return null;
#endif
        }
    }
}
