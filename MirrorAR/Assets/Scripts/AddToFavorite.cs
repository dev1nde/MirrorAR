using UnityEngine;
using System;
using System.Collections.Generic;
using UnityEngine.UI;

[Serializable]
public class Product
{
    public Sprite image;
    public string name;
    public string price;
    public bool isFavorite;

    public event Action<int, bool> OnFavoriteStateChanged; 
    public int Index { get; set; } 
    public bool IsFavorite
    {
        get { return isFavorite; }
        set
        {
            if (isFavorite != value)
            {
                isFavorite = value;
                PlayerPrefs.SetInt(name + "_IsFavorite", isFavorite ? 1 : 0);
                PlayerPrefs.Save();
                OnFavoriteStateChanged?.Invoke(Index, isFavorite);             }
        }
    }
}

public class AddToFavorite : MonoBehaviour
{
    public Product product;
    public List<Product> favoriteProducts;
    public Sprite favorite_on;
    public Sprite favorite_off;
    public Image favoriteImage;

    private bool isInitialized = false;

    private void Awake()
    {
        isInitialized = false;
    }

    private void Start()
    {
        product.IsFavorite = PlayerPrefs.GetInt(product.name + "_IsFavorite") == 1;
        UpdateFavoriteState();
        favoriteProducts = new List<Product>(FavoriteListManager.favoriteProducts);
        if (!isInitialized && favoriteProducts.Count == 0)
        {
            isInitialized = true;
            favoriteProducts = new List<Product>(FavoriteListManager.favoriteProducts);

            if (product.IsFavorite && !favoriteProducts.Contains(product))
            {
                product.Index = favoriteProducts.Count; 
                favoriteProducts.Add(product);
            }      
            FavoriteListManager.favoriteProducts = new List<Product>(favoriteProducts);
        }
    }


    public void ToggleFavorite()
    {
        bool previousState = product.IsFavorite;
        product.IsFavorite = !product.IsFavorite;

        if (product.IsFavorite && !previousState)
        {
                        bool isDuplicate = false;
            foreach (Product p in favoriteProducts)
            {
                if (AreProductsEqual(p, product))
                {
                    isDuplicate = true;
                    break;
                }
            }

                        if (!isDuplicate)
            {
                product.Index = favoriteProducts.Count; 
                favoriteProducts.Add(product);
            }
        }
        else if (!product.IsFavorite)
        {
         RemoveItemByProduct(product);
        }
        UpdateFavoriteState();
        PlayerPrefs.SetInt(product.name + "_IsFavorite", product.IsFavorite ? 1 : 0);
        PlayerPrefs.Save();
        FavoriteListManager.favoriteProducts = new List<Product>(favoriteProducts);
    }


    private void UpdateFavoriteState()
    {
        if (product.IsFavorite)
        {
            favoriteImage.sprite = favorite_on;
        }
        else
        {
            favoriteImage.sprite = favorite_off;
        }
    }

    void RemoveItemByIndex(int index)
    {
        if (index >= 0 && index < favoriteProducts.Count)
        {
            favoriteProducts.RemoveAt(index);
            for (int i = index; i < favoriteProducts.Count; i++)
            {
                favoriteProducts[i].Index = i;             }
        }
        FavoriteListManager.favoriteProducts = new List<Product>(favoriteProducts);
    }
    private bool AreProductsEqual(Product productA, Product productB)
    {
        return productA.name == productB.name && productA.price == productB.price;
    }

        private void RemoveItemByProduct(Product productToRemove)
    {
        for (int i = 0; i < favoriteProducts.Count; i++)
        {
            if (AreProductsEqual(favoriteProducts[i], productToRemove))
            {
                favoriteProducts.RemoveAt(i);
                productToRemove.Index = -1;                 return;
            }
        }
    }
}
