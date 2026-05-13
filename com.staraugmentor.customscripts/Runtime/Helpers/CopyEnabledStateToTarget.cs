using UnityEngine;

namespace StarCooperation.Helpers
{
    public class CopyEnabledStateToTarget: MonoBehaviour
    {
        [Tooltip("Target inherits this gameobject's enabled state")]
        [SerializeField] GameObject target;

        void OnEnable()
        {
            if (target != null) target.gameObject.SetActive(true);
        }

        void OnDisable()
        {
            if (target != null) target.gameObject.SetActive(false);
        }
    }
}
