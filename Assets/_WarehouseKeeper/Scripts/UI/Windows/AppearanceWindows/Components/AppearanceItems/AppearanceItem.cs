using System;
using Game.Pools;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace WarehouseKeeper._WarehouseKeeper.Scripts.UI.Windows.AppearanceWindows.Components.AppearanceItems
{
internal class AppearanceItem : BasePooledObject
{
    [SerializeField] private RectTransform root;
    [SerializeField] private Button selfButton;
    
    [Space, SerializeField] private Image icon;
    [SerializeField] private Image background;
    [SerializeField] private TextMeshProUGUI description;
    
    public string CodeId { get; set; }
    public event Action<AppearanceItem> OnClickItem;
    public override bool IsUiElement => true;

    private void Start()
    {
        selfButton.onClick.AddListener(ClickItem);
    }

    public void Setup(Sprite iconSprite, Color backgroundColor, string descriptionText)
    {
        icon.sprite = iconSprite;
        background.color = backgroundColor;
        description.gameObject.SetActive(!string.IsNullOrEmpty(descriptionText));
        description.text = descriptionText;
    }
    
    private void ClickItem() => OnClickItem?.Invoke(this);
    
    public override void OnRelease()
    {
        CodeId = null;
        OnClickItem = null;
    }

}
}