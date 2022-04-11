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

        ReorderableList _reorderableList = null;
        static Vector2 _scrollVec2;

        public override void Init(IPipeline pipeline) {
            base.Init(pipeline);
            InitGUI();
        }

        void InitGUI() {
            _reorderableList = _pipeline.CopyPathManager.CreateReorderableList();
            _reorderableList.drawElementCallback = DrawElement;
            _reorderableList.onChangedCallback = Changed;
            _reorderableList.drawHeaderCallback = DrawHeader;
            _reorderableList.onAddCallback = AddCallback;
            _reorderableList.onRemoveCallback = RemoveCallback;
            _reorderableList.drawNoneElementCallback = NoneElement;

            _reorderableList.headerHeight = 0;
            _reorderableList.footerHeight = 0;
            _reorderableList.elementHeight = CONST.GUI_ITEM_SIZE_HEIGHT * 3;
        }

        public override void Update() {
            _pipeline.CopyPathManager.DisplayProgressBar();
        }

        public override void OnGUI() {
            EditorGUI.BeginDisabledGroup(_pipeline.CopyPathManager.IsNowCopy);

            DrawToolbar();

            _scrollVec2 = GUILayout.BeginScrollView(_scrollVec2);
            _reorderableList.DoLayoutList();
            GUILayout.EndScrollView();

            _pipeline.CopyPathManager.Update();

            EditorGUI.EndDisabledGroup();
        }

        void DrawHeader(Rect rect) {
            EditorGUI.LabelField(rect, "");
        }

        // 入れ替え時に呼び出す
        void Changed(ReorderableList list) {
            _pipeline.CopyPathManager.IsApply = true;
        }

        void AddCallback(ReorderableList list) {
            _pipeline.CopyPathManager.Add();
        }

        void RemoveCallback(ReorderableList list) {
            _pipeline.CopyPathManager.Remove(list.index);
        }

        void DrawElement(Rect rect, int index, bool isActive, bool isFocused) {
            _pipeline.CopyPathManager.GUIDrawItem(rect, index);
        }

        void NoneElement(Rect rect) {
            EditorGUI.LabelField(rect, "Empty");
        }

        void DrawToolbar() {
            using (new EditorGUILayout.HorizontalScope()) {
                EditorGUI.BeginDisabledGroup(_pipeline.CopyPathManager.IsDisabledApply);
                if (GUILayout.Button(new GUIContent(EditorGUIUtility.IconContent(CONST.REFRESH).image), EditorStyles.miniButton)) {
                    _pipeline.CopyPathManager.Load();
                    InitGUI();
                }
                if (GUILayout.Button("Apply", EditorStyles.miniButton)) {
                    _pipeline.CopyPathManager.Save();
                }
                EditorGUI.EndDisabledGroup();

                if (GUILayout.Button("Add", EditorStyles.miniButton)) {
                    _pipeline.CopyPathManager.Add();
                }

                if (GUILayout.Button("All Copy", EditorStyles.miniButton)) {
                    _pipeline.CopyPathManager.CopyAll();
                }

                if (GUILayout.Button("All Remove", EditorStyles.miniButton)) {
                    _pipeline.CopyPathManager.RemoveAll();
                }

                GUILayout.FlexibleSpace();
            }
        }

    }
}
#endif
