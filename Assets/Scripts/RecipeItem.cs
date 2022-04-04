using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RecipeItem : object
{
    public GameObject fruit;
    public bool isHalf;

    public RecipeItem(GameObject fruit, bool isHalf)
    {
        this.fruit = fruit;
        this.isHalf = isHalf;
    }

    public override bool Equals(object other)
    {
        if (other == null)
        {
            return false;
        }
        RecipeItem other1 = other as RecipeItem;

        if (other1 == null || GetType() != other1.GetType())
        {
            return false;
        }

        return (fruit.name == other1.fruit.name && isHalf == other1.isHalf);
    }

    public override int GetHashCode()
    {
        return base.GetHashCode();
    }
}
