using System;
using Game.Pools;
using Game.Shops;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace WarehouseKeeper.UI.Windows.ShopWindows
{
internal class ShopItem : BasePooledObject
{
    [SerializeField] private Image _icon;
    [SerializeField] private TextMeshProUGUI _priceText;
    [SerializeField] private TextMeshProUGUI _nameText;
    [SerializeField] private Button _itemButton;
    [SerializeField] private RectTransform _priceRoot;

    public override bool IsUiElement => true;
    public event Action<ShopItem> OnClickItem; 
    public GameProduct product; 

    private void Start()
    {
        _itemButton.onClick.AddListener(ClickItem);
    }

    public void Setup(GameProduct gameProduct, Sprite icon, string price, string productName)
    {
        product = gameProduct;
        _icon.sprite = icon;
        _priceText.text = price;
        _nameText.text = productName;
        LayoutRebuilder.ForceRebuildLayoutImmediate(_nameText.rectTransform);
        if (_priceRoot != null)
            LayoutRebuilder.ForceRebuildLayoutImmediate(_priceRoot);
    }

    private void ClickItem() => OnClickItem?.Invoke(this);
    
    public override void OnRelease()
    {
        OnClickItem = null;
        product = null;
    }
}
}
