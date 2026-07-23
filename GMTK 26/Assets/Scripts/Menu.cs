using UnityEngine;
using UnityEngine.InputSystem;

public class Menu : MonoBehaviour
{
    private int _floor = 1;
    
    public void HoverFloor(int num)
    {
        if (!Mouse.current.leftButton.wasPressedThisFrame) return;
        _floor = num;
    }
}
