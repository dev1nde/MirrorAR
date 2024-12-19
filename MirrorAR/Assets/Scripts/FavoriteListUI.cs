using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;

public class FavoriteListUI : MonoBehaviour
{
    public GameObject productPrefab;
    public Transform productListContainer;
    public List<Product> favoriteProducts;
    private bool hasInitializedUI = false;
    public Text noFavoritesText; 
    private void Awake()
    {
        hasInitializedUI = false;
    }

    private void Start()
    {
        if (hasInitializedUI == false)
        {
            favoriteProducts = new List<Product>(FavoriteListManager.favoriteProducts);
            float yOffset = 0f;
            if (favoriteProducts.Count == 0)
            {
                                noFavoritesText.gameObject.SetActive(true);
            }
            else
            {
                                noFavoritesText.text = string.Empty;
            }
            foreach (Product product in favoriteProducts)
            {
                GameObject productUI = Instantiate(productPrefab, productListContainer);
                Image imageComponent = productUI.GetComponentInChildren<Image>();
                Text[] textComponents = productUI.GetComponentsInChildren<Text>();
                Button buttonComponent = productUI.GetComponentInChildren<Button>();

                imageComponent.sprite = product.image;
                textComponents[0].text = product.name;
                textComponents[1].text = product.price;

                                RectTransform rectTransform = productUI.GetComponent<RectTransform>();
                rectTransform.anchoredPosition = new Vector2(rectTransform.anchoredPosition.x, -yOffset);

                                yOffset += rectTransform.sizeDelta.y + 300f; 
            }
            productPrefab.SetActive(false);
            hasInitializedUI = true;
        }
    }
}
