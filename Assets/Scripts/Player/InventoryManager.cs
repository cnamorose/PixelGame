using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class InventoryManager : MonoBehaviour
{
    [Header("Item Icons")]
    public Image pen;
    public Image paper;
    public Image usb;
    public Image paper2;

    // 아이템 소유 여부
    public bool hasPen;
    public bool hasPaper;
    public bool hasUsb;
    public bool hasPaper2;

    void Start()
    {
        UpdateInventoryUI();
    }

    public void UpdateInventoryUI()
    {
        pen.enabled = hasPen;
        paper.enabled = hasPaper;
        usb.enabled = hasUsb;
        paper2.enabled = hasPaper2;
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Alpha1))
        {
            Debug.Log("1 눌림");
            hasPen = true;
            UpdateInventoryUI();
        }

        if (Input.GetKeyDown(KeyCode.Alpha2))
        {
            hasPaper = true;
            UpdateInventoryUI();
        }
    }
}
