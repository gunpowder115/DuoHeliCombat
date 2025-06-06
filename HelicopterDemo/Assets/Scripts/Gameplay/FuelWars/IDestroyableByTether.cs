using UnityEngine;

namespace Assets.Scripts.Gameplay.FuelWars
{
    public interface IDestroyableByTether
    {
        void CallToDestroy(in Vector3 destroyDir);
    }
}