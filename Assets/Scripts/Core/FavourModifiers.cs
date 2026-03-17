using UnityEngine;

public static class FavourModifiers
{
    // Returns a multiplier based on favour value
    // High favour (50+) = bonus, low favour (negative) = penalty, neutral = 1.0
    public static float GetModifier (int favour)
    {
        if (favour >= 80) return 1.5f;
        if (favour >= 50) return 1.25f;
        if (favour >= 20) return 1.0f;
        if (favour >= 0)  return 1.0f;
        if (favour >= -20) return 0.85f;
        if (favour >= -50) return 0.7f;
        return 0.5f;
    }

    // Returns a flat relationship modifier
    public static int GetRelationshipBonus (int aphroditeFavour)
    {
        if (aphroditeFavour >= 80) return 3;
        if (aphroditeFavour >= 50) return 2;
        if (aphroditeFavour >= 20) return 1;
        if (aphroditeFavour < -20) return -1;
        if (aphroditeFavour < -50) return -2;
        return 0;
    }
}