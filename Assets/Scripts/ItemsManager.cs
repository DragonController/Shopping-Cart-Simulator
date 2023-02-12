using UnityEngine;

public class ItemsManager : MonoBehaviour {
    private void Update() {
        foreach (int index in GameCreationParams.itemIndices) {
            // print(index);
        }
    }
}
