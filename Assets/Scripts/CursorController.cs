using UnityEngine;

public class CursorController : MonoBehaviour {
    [SerializeField] Texture2D _defaultCursor;

    private void Start() {
        Cursor.SetCursor(_defaultCursor, new Vector2(5.0f, 0.0f), CursorMode.Auto);
    }
}
