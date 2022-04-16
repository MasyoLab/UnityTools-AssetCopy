#if UNITY_EDITOR
using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine.Events;

//=========================================================
//
//  developer : MasyoLab
//  github    : https://github.com/MasyoLab/UnityTools-AssetCopy
//
//=========================================================

namespace MasyoLab.Editor.AssetCopy {

    struct SaveLoad {
        public static void Save(string jsonData, string filePath) {
            // パス無し
            if (string.IsNullOrEmpty(filePath)) {
                return;
            }

            // 保存処理
            System.IO.File.WriteAllText(filePath, jsonData);
        }

        public static string Load(string filePath) {
            // パス無し
            if (string.IsNullOrEmpty(filePath)) {
                return string.Empty;
            }

            if (!System.IO.File.Exists(filePath)) {
                return string.Empty;
            }

            var reader = new System.IO.StreamReader(filePath);
            string jsonStr = reader.ReadLine();
            reader.Close();

            return jsonStr;
        }

        public static void SaveFile(string jsonData, string directory, string filename) {
            directory = directory == string.Empty ? CONST.ASSETS : directory;

            // ファイルパス
            var filePath = EditorUtility.SaveFilePanel(CONST.SAVE, directory, filename, CONST.FILE_EXTENSION);
            if (string.IsNullOrEmpty(filePath)) {
                return;
            }

            Save(jsonData, filePath);
        }

        public static string LoadFile(string directory) {
            directory = directory == string.Empty ? CONST.ASSETS : directory;

            // ファイルパス
            var filePath = EditorUtility.OpenFilePanel(CONST.LOAD, directory, CONST.FILE_EXTENSION);
            if (string.IsNullOrEmpty(filePath)) {
                return string.Empty;
            }

            return Load(filePath);
        }

        public static bool LoadFolder(string directory, System.Action<string> success) {
            directory = directory == string.Empty ? CONST.ASSETS : directory;

            // ファイルパス
            var filePath = EditorUtility.OpenFolderPanel(CONST.SELECT, directory, string.Empty);
            if (string.IsNullOrEmpty(filePath)) {
                return false;
            }

            success?.Invoke(filePath);
            return true;
        }

        /// <summary>
        /// 保存先
        /// </summary>
        /// <returns></returns>
        public static string GetSaveDataPath(string fileName, string ext = CONST.FILE_EXTENSION) {
            var filePath = $"{UnityEngine.Application.dataPath.RemoveAtLast(CONST.ASSETS)}{CONST.LIBRARY}/{CONST.FOLDER_NAME}";

            if (!System.IO.File.Exists(filePath)) {
                System.IO.Directory.CreateDirectory(filePath);
            }

            return $"{filePath}/{fileName}.{ext}";
        }
    }
}
#endif
