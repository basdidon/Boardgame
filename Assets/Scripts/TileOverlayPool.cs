using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UIElements;

public class TileOverlayPool : MonoBehaviour
{
    public static TileOverlayPool Instance { get; private set; }

    public enum OverlayType { Focus, Green, }

    // overlay object pool
    public int poolSize;
    public GameObject focusOverlay;
    GameObject[] overlayPool;

    public Material focusOverlayMaterial;
    public Material greenOverlayMaterial;

    public VisualTreeAsset focusTreeAsset;
    public VisualTreeAsset greenTreeAsset;

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(this);
        }
        else
        {
            Instance = this;
        }

        overlayPool = new GameObject[poolSize];

        for (int i = 0; i < poolSize; i++)
        {
            overlayPool[i] = Instantiate(focusOverlay, transform);
            overlayPool[i].SetActive(false);
        }
    }

    public GameObject GetFocusOverlay(OverlayType type)
    {
        var obj = overlayPool.FirstOrDefault(obj => !obj.activeInHierarchy);
        obj.GetComponent<Renderer>().material = type switch
        {
            OverlayType.Focus => focusOverlayMaterial,
            OverlayType.Green => greenOverlayMaterial,
            _ => throw new System.Exception(),
        };
        return obj;
    }

}