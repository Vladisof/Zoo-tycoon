using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;
using Button = UnityEngine.UI.Button;

public class ShopUnlock : MonoBehaviour
{
    public PlayerTouchMovement playerTouchMovement;
    public Button buttonUnlock;
    public Button buttonClose;
    public GameObject shop;
    
    private void Start()
    {
        buttonUnlock.onClick.AddListener(UnlockShop);
        buttonClose.onClick.AddListener(CloseShop);
    }
    
    private void CloseShop()
    {
        shop.SetActive(false);
        playerTouchMovement.OnActivate();
    }
    
    private void UnlockShop()
    {
        shop.SetActive(true);
        playerTouchMovement.OnDeactivate();
    }
    
}
