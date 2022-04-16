#if UNITY_EDITOR
using System.Collections;
using System.Collections.Generic;
using System.IO;
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

    class CopySettingManager : BaseManager {

        private List<CopySettingData> m_datas = new List<CopySettingData>();
        public bool IsUpdate { get; set; } = false;

        public CopySettingManager(IPipeline pipeline) : base(pipeline) {
            Load();
        }

        public ReorderableList CreateReorderableList() {
            var reorderableList = new ReorderableList(m_datas, typeof(string));
            reorderableList.list = m_datas;
            return reorderableList;
        }

        public void Update() {
            if (IsUpdate) {
                Save();
            }
        }

        public void Add() {
            m_datas.Add(new CopySettingData());
        }

        public void Remove(int index) {
            m_datas.RemoveAt(index);
        }

        private void Save() {
            var jsonStr = CopySettingDataJson.ToJson(m_datas);
            SaveLoad.Save(jsonStr, SaveLoad.GetSaveDataPath(CONST.COPY_SETTING_FILE_NAME));
            IsUpdate = false;
        }

        private void Load() {
            var jsonStr = SaveLoad.Load(SaveLoad.GetSaveDataPath(CONST.COPY_SETTING_FILE_NAME));
            m_datas = CopySettingDataJson.FromJson(jsonStr);
            IsUpdate = false;
        }

        public void GUIDrawItem(Rect rect, int index) {
            var item = m_datas[index];
            var sourceDirectory = EditorGUI.TextField(rect, item.Extension);
            if (item.Extension != sourceDirectory) {
                item.Extension = sourceDirectory;
                IsUpdate = true;
            }
        }

        public bool IsCopiable(FileInfo file) {
            foreach (var item in m_datas) {
                if (file.Name.Contains(item.Extension)) {
                    return true;
                }
            }
            return false;
        }
    }
}
#endif
