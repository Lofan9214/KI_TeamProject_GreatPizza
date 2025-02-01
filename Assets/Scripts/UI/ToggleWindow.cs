using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class ToggleWindow : MonoBehaviour
{
    public Button nextButton;
    public IngredientToggler prefab;
    public Transform[] contents;


    private void Start()
    {
        nextButton.onClick.AddListener(() => SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex));

        var ingList = DataTableManager.IngredientTable.GetList();

        foreach (var ing in ingList)
        {
            if (SaveLoadManager.Data.unlocks.TryGetValue(ing.ingredientID, out bool val))
            {
                var tog = Instantiate(prefab, contents[(int)ing.type - 1]);
                tog.Init(ing.ingredientID, val);
            }
        }
    }
}
