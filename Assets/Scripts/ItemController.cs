using UnityEngine;

public class ItemController : MonoBehaviour
{
    [SerializeField] [Range(0, 9)] private int _typeIndex;

    public int GetTypeIndex() {
        return _typeIndex;
    }
}
