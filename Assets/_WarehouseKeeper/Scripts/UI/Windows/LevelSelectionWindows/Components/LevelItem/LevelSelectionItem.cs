using System;
using Game;
using Game.Pools;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using WarehouseKeeper.Levels;

namespace WarehouseKeeper.UI.Windows.LevelSelections
{
internal class LevelSelectionItem : BasePooledObject
{
    [SerializeField] private TextMeshProUGUI levelNumber;
    [SerializeField] private GameObject lockImage;
    [SerializeField] private Button levelButton;
    [SerializeField] private Image backgroundImage;
    [SerializeField] private Sprite unlockedBackgroundSprite;
    [SerializeField] private Sprite lockedBackgroundSprite;
    [SerializeField] private StarsView _stars;

    [Header("Info"), SerializeField] private GameObject infoRoot;
    [SerializeField] private Button infoButton;
    
    [Header("Purchase"), SerializeField] private GameObject purchaseRoot;
    [SerializeField] private TextMeshProUGUI priceText;
    
    public event Action<LevelSelectionItem> OnClickItem; 
    public event Action<LevelSelectionItem> OnClickInfo;
    public override bool IsUiElement => true;

    public int LevelId { get; set; } = -1;
    public int LevelPrice { get; set; } = 0;

    private void Start()
    {
        levelButton.onClick.AddListener(ClickItem);
        infoButton.onClick.AddListener(ClickInfo);
    }

    public void SetupLockedState(int purchasePrice, bool canBuy)
    {
        backgroundImage.sprite = lockedBackgroundSprite;
        priceText.text = purchasePrice.ToString();
        
        lockImage.SetActive(true);
        levelNumber.gameObject.SetActive(false);
        purchaseRoot.SetActive(canBuy);
        _stars.SetActive(false);
        infoRoot.SetActive(false);
    }
    
    public void SetupUnlockedState(LevelData levelData)
    {
        if (levelData == null)
        {
            Log.InternalError();
            return;
        }

        LevelId = levelData.Id;
        levelNumber.text = (levelData.Id + 1).ToString();
        backgroundImage.sprite = unlockedBackgroundSprite;
        _stars.StaticSetup(levelData.StarsReceived);
        
        lockImage.SetActive(false);
        levelNumber.gameObject.SetActive(true);
        purchaseRoot.SetActive(false);
        _stars.SetActive(true);
        infoRoot.SetActive(true);
    }

    private void ClickItem() => OnClickItem?.Invoke(this);
    private void ClickInfo() => OnClickInfo?.Invoke(this);
    
    public override void OnRelease()
    {
        LevelId = -1;
        LevelPrice = 0;
        OnClickItem = null;
        OnClickInfo = null;
    }
}
}