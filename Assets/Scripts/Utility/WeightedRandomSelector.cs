using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class WeightedRandomSelector
{
    private List<float> weights;
    private float weightSum;

    public WeightedRandomSelector(List<float> weights)
    {
        this.weights = weights;

        weightSum = weights.Sum();
    }

    public int SelectIndex()
    {
        int selectedIndex = -1;
        float selector = Random.Range(0f, weightSum);
        float selectionLowerBound = 0f;

        for (int i = 0; i < weights.Count; ++i)
        {
            if (selector >= selectionLowerBound &&
                selector <= selectionLowerBound + weights[i])
            {
                selectedIndex = i;
                break;
            }

            selectionLowerBound += weights[i];
        }

        return selectedIndex;
    }
}
