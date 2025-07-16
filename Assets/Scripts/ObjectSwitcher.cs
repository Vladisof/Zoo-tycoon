using UnityEngine;

public class ObjectSwitcher : MonoBehaviour
{
    public GameObject object1; // Первый объект
    public GameObject object2; // Второй объект

    public void SwitchObjects1()
    {
        object1.SetActive(false);
        object2.SetActive(true);
    }
}