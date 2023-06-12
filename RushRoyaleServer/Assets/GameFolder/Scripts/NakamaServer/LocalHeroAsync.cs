using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LocalHeroAsync : Hero
{
    private string _spawnId;

    public string SpawnId => _spawnId;

    public void RemoteSpawn(Vector2 position)
    {
        MultiplayerManager.Instance.Send(MultiplayerManager.Code.Transform, MatchDataJson.SetTransform(position));
    }

    public void RemoteAttack()
    {

    }
}
