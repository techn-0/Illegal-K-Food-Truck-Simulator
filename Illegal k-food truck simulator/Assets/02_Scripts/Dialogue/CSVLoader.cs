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

            string[] lines = csvAsset.text.Split('\n');
            
            if (lines.Length <= 1)
            {
                Debug.LogWarning("CSV has no data rows");
                return dialogueLines;
            }

            // 첫 줄은 헤더이므로 스킵 / Skip header row
            for (int i = 1; i < lines.Length; i++)
            {
                string line = lines[i].Trim();
                if (string.IsNullOrEmpty(line)) continue;

                DialogueLine dialogueLine = ParseCSVLine(line);
                if (dialogueLine != null)
                {
                    dialogueLines.Add(dialogueLine);
                }
            }

            return dialogueLines;
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
                    inQuotes = !inQuotes;
                }
                else if (c == ',' && !inQuotes)
                {
                    fields.Add(currentField.ToString().Trim());
                    currentField.Clear();
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
            return value.ToLower() == "true";
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
        /// 화자 이미지를 Resources에서 로드 / Load speaker image from Resources
        /// </summary>
        /// <param name="fileName">파일명</param>
        /// <returns>Sprite 또는 null</returns>
        private static Sprite LoadSpeakerImage(string fileName)
        {
            if (string.IsNullOrEmpty(fileName))
                return null;

            string resourcePath = $"Portraits/{fileName}";
            Sprite sprite = Resources.Load<Sprite>(resourcePath);
            
            if (sprite == null)
            {
                Debug.LogWarning($"Could not load speaker image: {resourcePath}");
            }

            return sprite;
        }
    }
}
