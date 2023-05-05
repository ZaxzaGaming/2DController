using CoreSystem.CoreComponent;
using System.Collections;
using UnityEngine;

namespace CoreSystem
{
    public class Core : MonoBehaviour
    {
        public Movement Movement { get; private set; }

        private void Awake()
        {
            Movement = GetComponentInChildren<Movement>();

            if (!Movement)
            {
                Debug.LogError("Missing Core Component");
            }
        }

    }
}