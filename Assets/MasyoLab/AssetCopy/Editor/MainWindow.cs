#if UNITY_EDITOR
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

//=========================================================
//
//  developer : MasyoLab
//  github    : https://github.com/MasyoLab/UnityTools-AssetCopy
//
//=========================================================

namespace MasyoLab.Editor.AssetCopy {

    class MainWindow : EditorWindow {

        private class Pipeline : IPipeline {
            public EditorWindow Root { get; set; } = null;
            public Rect WindowSize { get; set; } = Rect.zero;

            private CopyPathManager m_copyPathManager = null;
            public CopyPathManager CopyPathManager {
                get {
                    if (m_copyPathManager == null) {
                        m_copyPathManager = new CopyPathManager(this);
                    }
                    return m_copyPathManager;
                }
            }

            private CopySettingManager m_copySettingManager = null;
            public CopySettingManager CopySettingManager {
                get {
                    if (m_copySettingManager == null) {
                        m_copySettingManager = new CopySettingManager(this);
                    }
                    return m_copySettingManager;
                }
            }
        }

        private BaseWindow m_guiWindow = null;
        private List<BaseWindow> m_windows = new List<BaseWindow>();
        private Pipeline m_pipeline = new Pipeline();

        /// <summary>
        /// ウィンドウを追加
        /// </summary>
        [MenuItem(CONST.MENU_ITEM)]
        private static void Init() {
            GetWindow<MainWindow>(CONST.EDITOR_WINDOW_NAME);
        }

        private void OnGUI() {
            DrawToolbar();
            if (m_guiWindow == null) {
                GetWindowClass<AssetCopyWindow>();
            }
            m_pipeline.WindowSize = new Rect(0, EditorStyles.toolbar.fixedHeight, position.width, position.height - EditorStyles.toolbar.fixedHeight);
            m_guiWindow.OnGUI();
        }

        private void Update() {
            m_guiWindow?.Update();
        }

        private void DrawToolbar() {
            using (new EditorGUILayout.HorizontalScope(EditorStyles.toolbar)) {
                if (GUILayout.Button(new GUIContent("Menu"), EditorStyles.toolbarDropDown)) {
                    OpenMenu();
                }
                if (GUILayout.Button(new GUIContent("Home"), EditorStyles.toolbarButton)) {
                    GetWindowClass<AssetCopyWindow>();
                }
                GUILayout.FlexibleSpace();
            }
        }

        private void OpenMenu() {
            // Now create the menu, add items and show it
            var menu = new GenericMenu();

            menu.AddItem(new GUIContent("Copy Setting"), false, (call) => {
                GetWindowClass<CopySettingWindow>();
            }, "Setting");

            menu.AddItem(new GUIContent("Help"), false, (call) => {
            }, "Help");

            menu.DropDown(new Rect(0, EditorStyles.toolbar.fixedHeight, 0f, 0f));
        }

        private _Ty GetWindowClass<_Ty>() where _Ty : BaseWindow, new() {
            if (m_guiWindow is _Ty) {
                return m_guiWindow as _Ty;
            }

            foreach (var item in m_windows) {
                if (!(item is _Ty)) {
                    continue;
                }
                item.Init(m_pipeline);
                m_guiWindow?.Close();
                m_guiWindow = item;
                return item as _Ty;
            }

            var newWindow = new _Ty();
            newWindow.Init(m_pipeline);
            m_windows.Add(newWindow);
            m_guiWindow?.Close();
            m_guiWindow = newWindow;
            return newWindow;
        }
    }
}
#endif
