using System.Collections;
using System.Collections.Generic;
using UnityEngine;
namespace StarCooperation
{
    public static class LB11Sync
    {
        public static void SwitchOnMessage(string[] message, bool value)
        {
            if (value)
            {
                switch (message[3])
                {
                    case "Menu":
                        Menu(message, value);
                        break;

                    case "Tab":
                        Tab(message);
                        break;
                    case "Endstecker":
                        Endstecker(message);

                        break;
                    case "Null":
                        break;
                    default:
                        break;
                }
            }
            else
            {
                Synced_LB11.toggleIDs[message[1]].uiToggle.GetComponent<ToggleEar>().Toggle.isOn = value;
            }


        }
        private static void Menu(string[] message, bool value)
        {
            Synced_LB11.toggleIDs[message[1]].uiToggle.GetComponent<ToggleEar>().Toggle.isOn = value;
            //SteckerHandler.Instance.RegisterMenuClick(Synced_LB11.toggleIDs[message[1]].uiToggle.GetComponent<ToggleEar>().Stecker, value);
        }

        private static void Endstecker(string[] message)
        {
            SteckerHandler.Instance.RegisterEndsteckerClick(Synced_LB11.toggleIDs[message[1]].uiToggle.GetComponent<ToggleEar>());
        }

        private static void Tab(string[] message)
        {
            for (int i = 0; i < TabController.Instance.tablist.Count; i++)
            {
                if (TabController.Instance.tablist[i].correspStecker == Synced_LB11.toggleIDs[message[1]].uiToggle.GetComponent<ToggleEar>().Stecker)
                {
                    TabController.Instance.tablist[i].DoMagic(TabController.Instance.tablist[i]);
                }
            }
        }


    }
}