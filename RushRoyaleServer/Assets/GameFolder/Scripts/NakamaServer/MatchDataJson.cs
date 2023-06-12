using System.Collections.Generic;
using Nakama.TinyJson;
using UnityEngine;

public static class MatchDataJson
{
    public static float[] SetTransform(Vector2 position)
    {
        float[] values =
        {
            position.x,
            position.y,
        };

        return values;
    }
}