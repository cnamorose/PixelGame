using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class InventoryManager : MonoBehaviour
{
    public PlayerData playerdata;

    [Header("Item Icons")]
    public Image pen;
    public Image paper;
    public Image usb;
    public Image paper2;

    void Start()
    {
        UpdateInventoryUI();
    }

    public void UpdateInventoryUI()
    {
        pen.enabled = playerdata.hasPen;
        paper.enabled = playerdata.hasPaper;
        usb.enabled = playerdata.hasUsb;
        paper2.enabled = playerdata.hasPaper2;
    }
}
