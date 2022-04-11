#if UNITY_EDITOR
using System;
using System.IO;
using System.Threading.Tasks;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using UnityEditorInternal;

//=========================================================
//
//  developer : MasyoLab
//  github    : https://github.com/MasyoLab/UnityTools-AssetCopy
//
//=========================================================

namespace MasyoLab.Editor.AssetCopy {

    class CopyPathManager : BaseManager {

        private List<CopyPathData> _datas = new List<CopyPathData>();
        public IReadOnlyList<CopyPathData> Datas => _datas;
        private GUIStyle _textFieldStyle = null;
        public bool IsApply { private set; get; } = false;
        public bool IsDisabledApply => !IsApply;
        public bool IsNowCopy { private set; get; } = false;
        public bool IsDisabledOperatable => !IsNowCopy;
        private Stack<CopyPathData> _stack = new Stack<CopyPathData>();
        private List<CopyPathData> _remove = new List<CopyPathData>();

        public CopyPathManager(IPipeline pipeline) : base(pipeline) {
            _textFieldStyle = new GUIStyle(EditorStyles.textField) {
                alignment = TextAnchor.MiddleLeft,
            };
            Load();
        }

        public ReorderableList CreateReorderableList() {
            var reorderableList = new ReorderableList(_datas, typeof(CopyPathData));
            reorderableList.list = _datas;
            return reorderableList;
        }

        public void Update() {

            CopyDirectory();

            if (_remove.Count != 0) {
                _datas.RemoveAll(v => v.IsRemove);
                _remove.Clear();
                IsApply = true;
            }
        }

        public void GUIDrawItem(Rect rect, int index) {
            var item = Datas[index];
            GUIItemLine1(rect, item);
            GUIItemLine2(rect, item);
            GUIItemLine3(rect, item);
        }

        private static Rect RectAdjustment(Rect rect, int line) {
            var size = rect;
            size.height = CONST.GUI_ITEM_SIZE_HEIGHT;
            size.y += CONST.GUI_ITEM_SIZE_HEIGHT * Mathf.Max(0, line - 1);
            size.height -= 1;
            size.y += 1;
            return size;
        }

        private void GUIItemLine1(Rect rect, CopyPathData item) {
            var size = RectAdjustment(rect, 1);

            var btnSizeW = 25f;
            var anchor = 1f;
            size.width -= (btnSizeW + anchor);
            var sourceDirectory = EditorGUI.TextField(size, "Source Directory", item.SourceDirectory, _textFieldStyle);
            if (item.SourceDirectory != sourceDirectory) {
                item.SourceDirectory = sourceDirectory;
                IsApply = true;
            }

            size.x += (size.width + anchor);
            size.width = btnSizeW;
            if (GUI.Button(size, "...")) {
                SaveLoad.LoadFolder(item.SourceDirectory, (success) => {
                    if (item.SourceDirectory != success) {
                        item.SourceDirectory = success;
                        IsApply = true;
                    }
                });
            }
        }

        private void GUIItemLine2(Rect rect, CopyPathData item) {
            var size = RectAdjustment(rect, 2);
            var obj = EditorGUI.ObjectField(size, "Destination Directory", item.FolderAsset, typeof(DefaultAsset), false);
            if (item.FolderAsset == obj) {
                return;
            }

            if (!AssetDatabase.IsValidFolder(AssetDatabase.GetAssetPath(obj))) {
                return;
            }

            if (AssetDatabase.TryGetGUIDAndLocalFileIdentifier(obj, out string folder, out long _)) {
                item.FolderGUID = folder;
                item.FolderAsset = obj;
                IsApply = true;
            }
        }

        private void GUIItemLine3(Rect rect, CopyPathData item) {
            var size = RectAdjustment(rect, 3);
            size.width /= 2f;

            if (GUI.Button(size, "Copy")) {
                item.IsCopy = true;
                _stack.Push(item);
            }

            size.x += size.width;
            if (GUI.Button(size, "Remove")) {
                item.IsRemove = true;
                _remove.Add(item);
            }
        }

        /// <summary>
        /// aa
        /// </summary>
        /// <see href="https://docs.microsoft.com/ja-jp/dotnet/standard/io/how-to-copy-directories">reference</see>
        /// <param name="sourceDir"></param>
        /// <param name="destinationDir"></param>
        /// <param name="recursive"></param>
        private static async Task CopyDirectoryAsync(string sourceDir, string destinationDir, bool recursive) {
            if (!Directory.Exists(sourceDir)) {
                throw new DirectoryNotFoundException($"Source directory not found: {sourceDir}");
            }
            if (!Directory.Exists(destinationDir)) {
                throw new DirectoryNotFoundException($"Destination directory not found: {destinationDir}");
            }

            // Get information about the source directory
            var dir = new DirectoryInfo(sourceDir);

            // Check if the source directory exists
            if (!dir.Exists) {
                throw new DirectoryNotFoundException($"Source directory not found: {dir.FullName}");
            }

            // Cache directories before we start copying
            DirectoryInfo[] dirs = dir.GetDirectories();

            // Create the destination directory
            Directory.CreateDirectory(destinationDir);

            // Get the files in the source directory and copy to the destination directory
            foreach (FileInfo file in dir.GetFiles()) {
                string targetFilePath = Path.Combine(destinationDir, file.Name);
                file.CopyTo(targetFilePath, true);
                await Task.Delay(1);
            }

            // If recursive and copying subdirectories, recursively call this method
            if (recursive) {
                foreach (DirectoryInfo subDir in dirs) {
                    try {
                        string newDestinationDir = Path.Combine(destinationDir, subDir.Name);
                        await CopyDirectoryAsync(subDir.FullName, newDestinationDir, true);
                    }
                    catch (DirectoryNotFoundException dirEx) {
                        throw dirEx;
                    }
                }
            }
        }

        private async void CopyDirectory() {
            if (IsNowCopy) {
                return;
            }

            bool isUpdate = false;

            if (_stack.Count != 0) {
                IsNowCopy = true;
                await Task.Delay(1000);
            }

            for (; _stack.Count != 0;) {
                var item = _stack.Pop();

                try {
                    await CopyDirectoryAsync(item.SourceDirectory, item.DestinationDirectory, true);
                }
                catch (DirectoryNotFoundException dirEx) {
                    item.IsCopy = false;
                    item.IsRemove = false;
                    Debug.LogError(dirEx);
                    continue;
                }

                Debug.Log("ファイルがコピーされました");
                item.IsCopy = false;
                item.IsRemove = false;
                isUpdate = true;
            }

            if (isUpdate) {
                AssetDatabase.Refresh();
            }

            IsNowCopy = false;
        }

        public void Add() {
            _datas.Add(new CopyPathData());
            IsApply = true;
        }

        public void Remove(int index) {
            _datas.RemoveAt(index);
        }

        public void Save() {
            var jsonStr = CopyPathDataJson.ToJson(_datas);
            SaveLoad.Save(jsonStr, SaveLoad.GetSaveDataPath(CONST.JSON_EXT));
            IsApply = false;
        }

        public void Load() {
            var jsonStr = SaveLoad.Load(SaveLoad.GetSaveDataPath(CONST.JSON_EXT));
            _datas = CopyPathDataJson.FromJson(jsonStr);
            IsApply = false;
        }
    }
}
#endif
