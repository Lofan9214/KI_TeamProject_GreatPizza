using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Cutter : MonoBehaviour, IClickable, IDragable
{
    public CuttingSlot currentTable;
    public Transform cutterPreview;

    private bool isCutting = false;
    public SpriteRenderer spriteRenderer { get; private set; }
    private SpriteRotator rotator;
    public SpriteRotatorData spritesData;

    public AutoCutter autoCutter;

    private IngameGameManager gameManager;

    public bool CuttingLock { get; set; }
    public bool CuttingCanceled { get; set; }

    private AudioSource audioSource;

    private bool upgraded;

    private void Awake()
    {
        spriteRenderer = GetComponent<SpriteRenderer>();
        audioSource = GetComponent<AudioSource>();
        rotator = GetComponent<SpriteRotator>();
        CuttingLock = false;
        CuttingCanceled = false;
    }

    private void Start()
    {
        gameManager = GameObject.FindGameObjectWithTag("GameController")?.GetComponent<IngameGameManager>();
        string id = DataTableManager.StoreTable.GetTypeList(StoreTable.Type.CuttingBuddy)[0].storeID;
        upgraded = SaveLoadManager.Data.upgrades[id];
        if (upgraded)
        {
            rotator.targetSprite = spritesData.sprites[1];
            rotator.rotate = spritesData.rotates[1];
        }
        else
        {
            rotator.targetSprite = spritesData.sprites[0];
            rotator.rotate = spritesData.rotates[0];
        }
        rotator.RotateSprite();
    }

    public void OnPressObject(Vector2 position)
    {
        if (currentTable.IsEmpty)
        {
            return;
        }
        CuttingCanceled = false;
        if (upgraded)
        {
            if (gameManager.npc.Recipe.cutting == 3)
            {
                autoCutter.PlayAnimation();
                spriteRenderer.enabled = false;
            }
            return;
        }

        isCutting = true;
        cutterPreview.gameObject.SetActive(true);
        Vector2 offset = position - (Vector2)cutterPreview.transform.position;
        var dir = offset.normalized;
        cutterPreview.transform.up = dir;
        spriteRenderer.enabled = false;
        currentTable.CurrentPizza.CircleCollider.enabled = false;
    }

    public void OnDrag(Vector3 pos, Vector3 deltaPos)
    {
        if (isCutting)
        {
            Vector2 offset = cutterPreview.transform.position - pos;

            if (offset.sqrMagnitude < 1f)
            {
                isCutting = false;
                cutterPreview.gameObject.SetActive(false);
                spriteRenderer.enabled = true;
                currentTable.CurrentPizza.CircleCollider.enabled = true;
                CuttingCanceled = true;
                return;
            }
            var dir = offset.normalized;
            cutterPreview.transform.up = dir;
        }
    }

    public void OnDragEnd(Vector3 pos, Vector3 deltaPos)
    {
        if (!isCutting)
            return;
        isCutting = false;

        if (!CuttingLock)
        {
            audioSource.Play();
            currentTable.CurrentPizza.Cut(cutterPreview.rotation * Quaternion.Euler(0f, 0f, 90f));
        }

        cutterPreview.gameObject.SetActive(false);
        spriteRenderer.enabled = true;
        currentTable.CurrentPizza.CircleCollider.enabled = true;
    }

    public void PlayCutSound()
    {
        audioSource.Play();
    }
}
