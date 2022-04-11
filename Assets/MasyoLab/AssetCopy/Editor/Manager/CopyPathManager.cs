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

            var count = _datas.RemoveAll(v => v.IsRemove);
            if (count != 0) {
                IsApply = true;
            }
        }

        public void Draw(Rect rect, int index) {
            var item = Datas[index];

            {
                var btnSizeW = 25f;
                var size = rect;
                size.height = CONST.GUI_ITEM_SIZE_HEIGHT;
                size.height -= 2;
                size.y += 1;
                size.width -= btnSizeW;
                var sourceDirectory = EditorGUI.TextField(size, "Source Directory", item.SourceDirectory, _textFieldStyle);
                if (item.SourceDirectory != sourceDirectory) {
                    item.SourceDirectory = sourceDirectory;
                    IsApply = true;
                }

                size.x += size.width;
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

            {
                var size = rect;
                size.height = CONST.GUI_ITEM_SIZE_HEIGHT;
                size.y += CONST.GUI_ITEM_SIZE_HEIGHT;
                size.height -= 2;
                size.y += 1;

                var obj = EditorGUI.ObjectField(size, item.FolderAsset, typeof(DefaultAsset), false);
                if (item.FolderAsset != obj) {
                    var path = AssetDatabase.GetAssetPath(obj);
                    if (AssetDatabase.IsValidFolder(path)) {
                        if (AssetDatabase.TryGetGUIDAndLocalFileIdentifier(obj, out string folder, out long _)) {
                            item.FolderGUID = folder;
                            item.FolderAsset = obj;
                            IsApply = true;
                        }
                    }
                }
            }

            {
                var size = rect;
                size.height = CONST.GUI_ITEM_SIZE_HEIGHT;
                size.y += CONST.GUI_ITEM_SIZE_HEIGHT * 2;
                size.height -= 2;
                size.y += 1;
                size.width = size.width / 2;

                if (GUI.Button(size, "Copy")) {
                    item.IsCopy = true;
                }

                size.x += size.width;
                if (GUI.Button(size, "Remove")) {
                    item.IsRemove = true;
                }
            }
        }

        /// <summary>
        /// aa
        /// </summary>
        /// <see href="https://docs.microsoft.com/ja-jp/dotnet/standard/io/how-to-copy-directories">reference</see>
        /// <param name="sourceDir"></param>
        /// <param name="destinationDir"></param>
        /// <param name="recursive"></param>
        private static async Task CopyDirectoryAsync(string sourceDir, string destinationDir, bool recursive, System.Action resect) {
            if (!Directory.Exists(sourceDir)) {
                resect.Invoke();
                throw new DirectoryNotFoundException($"Source directory not found: {sourceDir}");
            }
            if (!Directory.Exists(sourceDir)) {
                resect.Invoke();
                throw new DirectoryNotFoundException($"Destination directory not found: {destinationDir}");
            }

            // Get information about the source directory
            var dir = new DirectoryInfo(sourceDir);

            // Check if the source directory exists
            if (!dir.Exists) {
                resect.Invoke();
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
                    string newDestinationDir = Path.Combine(destinationDir, subDir.Name);
                    await CopyDirectoryAsync(subDir.FullName, newDestinationDir, true, resect);
                }
            }
        }

        private async void CopyDirectory() {
            bool isUpdate = false;
            foreach (var item in _datas) {
                if (!item.IsCopy) {
                    continue;
                }

                IsNowCopy = true;

                await CopyDirectoryAsync(item.SourceDirectory, item.DestinationDirectory, true, () => {
                    item.IsCopy = false;
                    item.IsRemove = false;
                });
                Console.WriteLine("ファイルがコピーされました");
                Console.ReadKey();
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
