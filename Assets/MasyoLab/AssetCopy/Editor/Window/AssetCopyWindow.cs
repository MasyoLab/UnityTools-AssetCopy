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

    class AssetCopyWindow : BaseWindow {

        private ReorderableList m_reorderableList = null;
        private static Vector2 m_scrollVec2;

        public override void Init(IPipeline pipeline) {
            base.Init(pipeline);
            InitGUI();
        }

        private void InitGUI() {
            m_reorderableList = m_pipeline.CopyPathManager.CreateReorderableList();
            m_reorderableList.drawElementCallback = DrawElement;
            m_reorderableList.onChangedCallback = Changed;
            m_reorderableList.drawHeaderCallback = DrawHeader;
            m_reorderableList.onAddCallback = AddCallback;
            m_reorderableList.onRemoveCallback = RemoveCallback;
            m_reorderableList.drawNoneElementCallback = NoneElement;

            m_reorderableList.headerHeight = 0;
            m_reorderableList.elementHeight = CONST.GUI_ITEM_SIZE_HEIGHT * 3;
        }

        public override void Update() {
            m_pipeline.CopyPathManager.DisplayProgressBar();
        }

        public override void OnGUI() {
            EditorGUI.BeginDisabledGroup(m_pipeline.CopyPathManager.IsDisabledOperatable);

            DrawToolbar();

            m_scrollVec2 = GUILayout.BeginScrollView(m_scrollVec2);
            m_reorderableList.DoLayoutList();
            GUILayout.EndScrollView();

            m_pipeline.CopyPathManager.Update();

            EditorGUI.EndDisabledGroup();
        }

        private void DrawHeader(Rect rect) {
            EditorGUI.LabelField(rect, "");
        }

        // 入れ替え時に呼び出す
        private void Changed(ReorderableList list) {
            m_pipeline.CopyPathManager.IsApply = true;
        }

        private void AddCallback(ReorderableList list) {
            m_pipeline.CopyPathManager.Add();
        }

        private void RemoveCallback(ReorderableList list) {
            m_pipeline.CopyPathManager.Remove(list.index);
        }

        private void DrawElement(Rect rect, int index, bool isActive, bool isFocused) {
            m_pipeline.CopyPathManager.GUIDrawItem(rect, index);
        }

        private void NoneElement(Rect rect) {
            EditorGUI.LabelField(rect, "Empty");
        }

        private void DrawToolbar() {

            using (new EditorGUILayout.HorizontalScope()) {
                EditorGUI.BeginDisabledGroup(m_pipeline.CopyPathManager.IsDisabledApply);
                if (GUILayout.Button(new GUIContent(EditorGUIUtility.IconContent(CONST.REFRESH).image), EditorStyles.miniButton)) {
                    m_pipeline.CopyPathManager.Load();
                    InitGUI();
                }
                if (GUILayout.Button("Apply", EditorStyles.miniButton)) {
                    m_pipeline.CopyPathManager.Save();
                }
                EditorGUI.EndDisabledGroup();

                if (GUILayout.Button("Add", EditorStyles.miniButton)) {
                    m_pipeline.CopyPathManager.Add();
                }

                if (GUILayout.Button("All Copy", EditorStyles.miniButton)) {
                    m_pipeline.CopyPathManager.CopyAll();
                }

                if (GUILayout.Button("All Remove", EditorStyles.miniButton)) {
                    m_pipeline.CopyPathManager.RemoveAll();
                }

                GUILayout.FlexibleSpace();
            }
        }
    }
}
#endif
