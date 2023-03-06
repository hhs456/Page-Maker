using UnityEngine;
using UnityEditor;
using System.Collections.Generic;

public class PageMaker : EditorWindow {
    private Vector2 mousePosition; // 滑鼠點擊的位置
    private bool isDragging = false; // 是否正在拖動子視窗
    private Rect draggingWindowRect; // 正在拖動的子視窗矩形
    private int windowCount = 0; // 已經生成的子視窗數量
    private float menuHeight = 30f; // 選單高度
    private List<PageWindow> windows = new List<PageWindow>();    

    private static PageMaker Instance;

    [MenuItem("Window/Page Maker")]
    static void Init() {
        // 創建唯一的實例
        Instance = (PageMaker)EditorWindow.GetWindow(typeof(PageMaker));
        Instance.Show();
    }

    void OnGUI() {
        BeginWindows(); // 開始繪製子視窗

        for (int i = 0; i < windows.Count; i++) {
            GUI.color = windows[i].color; // 設定顏色
            windows[i].rect = GUI.Window(i, windows[i].rect, DrawWindow, "Window " + i.ToString());
            GUI.color = Color.white; // 重設顏色
        }

        EndWindows(); // 結束繪製子視窗

        if (Event.current.type == EventType.MouseDown && Event.current.button == 1) {
            CreateMenu();
        }
        else if (Event.current.type == EventType.MouseUp && Event.current.button == 0) {
            if (isDragging) {
                isDragging = false;
            }
        }

        if (isDragging) {
            draggingWindowRect.x = Event.current.mousePosition.x - draggingWindowRect.width / 2f;
            draggingWindowRect.y = Event.current.mousePosition.y - draggingWindowRect.height / 2f;
        }
    }

    private void CreateMenu() {
        GenericMenu menu = new GenericMenu();
        menu.AddItem(new GUIContent("Create Window"), false, CreateWindowCallback);
        menu.AddSeparator("");
        menu.AddItem(new GUIContent("Arrange Windows/Tile Horizontally"), false, TileHorizontallyCallback);
        menu.AddItem(new GUIContent("Arrange Windows/Tile Vertically"), false, TileVerticallyCallback);
        menu.ShowAsContext();
    }

    private void TileHorizontallyCallback() {
        float windowHeight = position.height - menuHeight;
        float windowWidth = position.width;
        float windowCount = windows.Count;
        float windowHeightPerWindow = windowHeight / windowCount;
        float y = 0;
        foreach (PageWindow window in windows) {
            window.rect = new Rect(0, y, windowWidth, windowHeightPerWindow);
            y += windowHeightPerWindow;
        }
    }

    private void TileVerticallyCallback() {
        float windowWidth = position.width / 2;
        float windowHeight = position.height - menuHeight;
        float windowCount = windows.Count;
        float windowWidthPerWindow = windowWidth / windowCount;
        float x = 0;
        float y = 0;
        int i = 0;
        foreach (PageWindow window in windows) {
            if (i % 2 == 0) {
                window.rect = new Rect(x, y, windowWidth, windowHeight);
            }
            else {
                window.rect = new Rect(x + windowWidth, y, windowWidth, windowHeight);
                y += windowHeight;
            }
            i++;
        }
    }

    private void DrawWindow(int id) {
        GUI.Box(new Rect(0, 0, 200, 20), "Window " + id.ToString()); // 標題欄
        if (GUI.Button(new Rect(180, 0, 20, 20), "X")) { // 關閉按鈕
            PageMaker.Instance.RemoveWindow(id);
        }
        GUI.DragWindow(); // 可以拖曳子視窗        

        GUILayout.BeginHorizontal();
        GUILayout.FlexibleSpace();

        // 顏色選擇器按鈕
        if (GUILayout.Button("Color")) {
            Color newColor = EditorGUI.ColorField(new Rect(Event.current.mousePosition.x, Event.current.mousePosition.y, 0, 0), windows[id].color);
            windows[id].color = newColor;
        }

        GUILayout.EndHorizontal();
    }
    public void RemoveWindow(int id) {
        windows.RemoveAt(id);
        windowCount--;
    }


    private void CreateWindowCallback() {              
        
        Rect windowRect = new Rect(mousePosition.x, mousePosition.y, 200, 200); // 計算子視窗位置和大小
        Color windowColor = Color.white; // 設定初始顏色為白色
        PageWindow window = new PageWindow(windowRect, windowColor);
        windows.Add(window);
        windowCount++;

        isDragging = true;
        draggingWindowRect = window.rect;
    }

    public class PageWindow {
        public Rect rect;
        public Color color;

        public PageWindow(Rect rect, Color color) {
            this.rect = rect;
            this.color = color;
        }
    }
}
