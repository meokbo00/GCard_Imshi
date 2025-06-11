using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class OptionManagement : MonoBehaviour
{
    public GameObject targetObject;

    public void Toggle()
    {
        if (targetObject != null)
        {
            bool isActive = targetObject.activeSelf;
            targetObject.SetActive(!isActive);
        }
    }
}