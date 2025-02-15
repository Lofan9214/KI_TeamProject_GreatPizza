using Cinemachine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Kitchen : MonoBehaviour
{
    public PackingTable packingTable;
    public Transform cameraStartPosition;
    public IngredientTableManager ingredientTable;
    public OvenEnter ovenEnter;
    public OvenExit ovenExit;
    public CuttingSlot cuttingTableSlot;
    public BoxCollider2D[] trashBins;
    private PolygonCollider2D confineCollider;

    public Cutter cutter;

    private void Awake()
    {
        confineCollider = GetComponent<PolygonCollider2D>();
    }

    public void Set(CinemachineConfiner2D confiner)
    {
        confiner.m_BoundingShape2D = confineCollider;
        confiner.gameObject.transform.position = cameraStartPosition.position;
    }

    public void SetPizzaBox()
    {
        packingTable.SetPizzaBox(1);
    }

    public void Init()
    {
        ingredientTable.Init();
    }
}
