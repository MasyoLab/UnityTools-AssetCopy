#if UNITY_EDITOR
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

    class CopySettingWindow : BaseWindow {

        private Vector2 m_scrollVec2;
        private ReorderableList m_reorderableList = null;

        public override void Init(IPipeline pipeline) {
            base.Init(pipeline);
            InitGUI();
        }

        private void InitGUI() {
            m_reorderableList = m_pipeline.CopySettingManager.CreateReorderableList();
            m_reorderableList.drawElementCallback = DrawElement;
            m_reorderableList.onChangedCallback = Changed;
            m_reorderableList.drawHeaderCallback = HeaderElement;
            m_reorderableList.onAddCallback = AddCallback;
            m_reorderableList.onRemoveCallback = RemoveCallback;
            m_reorderableList.drawNoneElementCallback = NoneElement;
        }

        public override void OnGUI() {
            m_scrollVec2 = GUILayout.BeginScrollView(m_scrollVec2);
            m_reorderableList.DoLayoutList();
            GUILayout.EndScrollView();

            m_pipeline.CopySettingManager.Update();
        }

        private void AddCallback(ReorderableList list) {
            m_pipeline.CopySettingManager.Add();
        }

        private void HeaderElement(Rect rect) {
            EditorGUI.LabelField(rect, "Set extension to copy");
        }

        private void NoneElement(Rect rect) {
            EditorGUI.LabelField(rect, "Empty");
        }

        private void RemoveCallback(ReorderableList list) {
            m_pipeline.CopySettingManager.Remove(list.index);
        }

        private void Changed(ReorderableList list) {
            m_pipeline.CopySettingManager.IsUpdate = true;
        }

        private void DrawElement(Rect rect, int index, bool isActive, bool isFocused) {
            m_pipeline.CopySettingManager.GUIDrawItem(rect, index);
        }
    }
}
#endif
