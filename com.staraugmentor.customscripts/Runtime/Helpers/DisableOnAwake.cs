using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace StarCooperation.Helpers
{
    public class DisableOnAwake: MonoBehaviour
    {

        void Awake()
        {
            gameObject.SetActive(false);
        }
    }
}
