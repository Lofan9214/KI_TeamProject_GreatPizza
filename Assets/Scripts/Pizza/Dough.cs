using UnityEngine;

public class Dough : MonoBehaviour
{
    public enum Layer
    {
        None,
        Source,
        Cheese,
    }

    public DrawIngredient sourceLayer;
    public DrawIngredient cheeseLayer;

    private void Start()
    {
        SetLayer(Layer.None);
    }

    public void SetLayer(Layer layer)
    {
        sourceLayer.enabled = false;
        cheeseLayer.enabled = false;
        switch (layer)
        {
            case Layer.Source:
                sourceLayer.enabled = true;
                break;
            case Layer.Cheese:
                cheeseLayer.enabled = true;
                break;
        }
    }
}
