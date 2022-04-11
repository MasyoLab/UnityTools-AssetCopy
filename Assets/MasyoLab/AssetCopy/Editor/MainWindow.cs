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

        class Pipeline : IPipeline {
            public EditorWindow Root { get; set; } = null;
            public Rect WindowSize { get; set; } = Rect.zero;

            private CopyPathManager _copyPathManager = null;

            public CopyPathManager CopyPathManager {
                get {
                    if (_copyPathManager == null) {
                        _copyPathManager = new CopyPathManager(this);
                    }
                    return _copyPathManager;
                }
            }
        }

        BaseWindow _guiWindow = null;
        List<BaseWindow> _windows = new List<BaseWindow>();
        Pipeline _pipeline = new Pipeline();

        /// <summary>
        /// ウィンドウを追加
        /// </summary>
        [MenuItem(CONST.MENU_ITEM)]
        static void Init() {
            GetWindow<MainWindow>(CONST.EDITOR_WINDOW_NAME);
        }

        void OnGUI() {
            GetWindowClass<AssetCopyWindow>();
            _guiWindow.OnGUI();
        }

        _Ty GetWindowClass<_Ty>() where _Ty : BaseWindow, new() {
            if (_guiWindow is _Ty) {
                return _guiWindow as _Ty;
            }

            foreach (var item in _windows) {
                if (!(item is _Ty)) {
                    continue;
                }
                item.Init(_pipeline);
                _guiWindow?.Close();
                _guiWindow = item;
                return item as _Ty;
            }

            var newWin = new _Ty();
            newWin.Init(_pipeline);
            _windows.Add(newWin);
            _guiWindow?.Close();
            _guiWindow = newWin;
            return newWin;
        }
    }
}
#endif
