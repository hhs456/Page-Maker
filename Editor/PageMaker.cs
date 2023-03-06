using UnityEngine;
using UnityEditor;
using System.Collections.Generic;

public class PageMaker : EditorWindow {
    private Vector2 mousePosition; // �ƹ��I������m
    private bool isDragging = false; // �O�_���b��ʤl����
    private Rect draggingWindowRect; // ���b��ʪ��l�����x��
    private int windowCount = 0; // �w�g�ͦ����l�����ƶq
    private float menuHeight = 30f; // ��氪��
    private List<PageWindow> windows = new List<PageWindow>();    

    private static PageMaker Instance;

    [MenuItem("Window/Page Maker")]
    static void Init() {
        // �Ыذߤ@�����
        Instance = (PageMaker)EditorWindow.GetWindow(typeof(PageMaker));
        Instance.Show();
    }

    void OnGUI() {
        BeginWindows(); // �}�lø�s�l����

        for (int i = 0; i < windows.Count; i++) {
            GUI.color = windows[i].color; // �]�w�C��
            windows[i].rect = GUI.Window(i, windows[i].rect, DrawWindow, "Window " + i.ToString());
            GUI.color = Color.white; // ���]�C��
        }

        EndWindows(); // ����ø�s�l����

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
        GUI.Box(new Rect(0, 0, 200, 20), "Window " + id.ToString()); // ���D��
        if (GUI.Button(new Rect(180, 0, 20, 20), "X")) { // �������s
            PageMaker.Instance.RemoveWindow(id);
        }
        GUI.DragWindow(); // �i�H�즲�l����        

        GUILayout.BeginHorizontal();
        GUILayout.FlexibleSpace();

        // �C���ܾ����s
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
        
        Rect windowRect = new Rect(mousePosition.x, mousePosition.y, 200, 200); // �p��l������m�M�j�p
        Color windowColor = Color.white; // �]�w��l�C�⬰�զ�
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
