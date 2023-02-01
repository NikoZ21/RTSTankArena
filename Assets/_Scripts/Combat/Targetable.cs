using Mirror;
using UnityEngine;

namespace _Scripts.Combat
{
    public class Targetable : NetworkBehaviour
    {
        [SerializeField] private Transform aimAtPoint = null;

        public Transform AimAtPoint
        {
            get => aimAtPoint;
        }
    }
}