using UnityEngine;
using static Types;

namespace Assets.Scripts.Controllers
{
    public interface IFindable
    {
        public GlobalSide2 Side { get; }
        public Vector3 Position { get; }
        public GameObject GameObject { get; }
    }
}
