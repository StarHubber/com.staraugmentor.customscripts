using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
namespace StarCooperation.Localization
{
    public class IDCatcher : MonoBehaviour
    {
        [SerializeField]
        private TextMeshProUGUI Text;
        private string Key;
        // Start is called before the first frame update
        void Start()
        {
            Invoke("Delayer", .02f);
        }

        private void Delayer()
        {
            Key = GetComponent<LegacyLocalization.LocalizedTextAuto>().key;
            string[] test = Key.Split('_');
            Text.SetText(test[1]);
        }
        // Update is called once per frame
        void Update()

        {


            //Text.SetText(Key);

        }
    }
}