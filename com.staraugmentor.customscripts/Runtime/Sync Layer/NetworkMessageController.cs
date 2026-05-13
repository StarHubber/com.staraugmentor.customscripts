using StarCooperation.Localization;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace StarCooperation
{
    //[DefaultExecutionOrder(-100)]	// Only for creation of message channel handler in Awake
    // Todo: (update: done) Remove Default Execution Order and use funktions/assignments properly in Start/Awake in both scripts - therefore, take SceneManager Datenflug and make it base.
    public class NetworkMessageController : MonoBehaviour
    {
        public static NetworkMessageController instance;

        [Header("What to do")]
        public bool isTransmitter;
        [SerializeField] private bool receive;
        public bool isReceiver
        {
            get
            {
                return receive;
            }
            set
            {
                if (receive != value)
                {
                    receive = value;
                    if (receive)
                    {
                        HoloRepair.Core.ContentAppInterface.OnMessageReceived += ParseReceivedNetworkMessages;
                    }
                    else
                    {
                        HoloRepair.Core.ContentAppInterface.OnMessageReceived -= ParseReceivedNetworkMessages;
                    }
				}
            }
        }

        [Header("Debug Settings")]
        public bool debugOutput = false;

        [Header("Special References")]
        public Button buttonCloseDetailViewTA;
        [HideInInspector] public string closeDetailViewMessage;

        // Privates
        private NetworkChannelHandler messageChannelHandler;// = new NetworkChannelHandler("ROUTED", new SceneManagerNamedMethodChannel("SMC"));

        private void Awake()
        {
            instance = this;

            messageChannelHandler = new NetworkChannelHandler("ROUTED", new SceneManagerNamedMethodChannel("SMC"));

            closeDetailViewMessage = buttonCloseDetailViewTA.gameObject.GetComponent<SyncedElementBase<Button>>().id;

            isReceiver = receive;
        }

        /// <summary>
        /// Registers a route and returns its id (used to send messages);
        /// </summary>
        public int RegisterSceneManagerRoutedMessage<T>(string methodName) where T : ISerializableMessagePayload, new()
        {
            messageChannelHandler.FindChannel<SceneManagerNamedMethodChannel>(out var channel);
            return channel.AddRoute<T>(methodName);
        }

        /// <summary>
        /// Send a message via a registered route
        /// </summary>
        public void SendSceneManagerRoutedMessage<T>(int routeID, T payload) where T : ISerializableMessagePayload, new()
        {
            messageChannelHandler.FindChannel<SceneManagerNamedMethodChannel>(out var channel);
            var msg = channel.CreateMessage(routeID, payload);
            SendNetworkMessage(messageChannelHandler.RoutedMessageKey, channel.ChannelID, msg);
        }

        public void SendNetworkMessage(params string[] message)
        {
            if (!isTransmitter)
            {
                return;
            }

            // Debug
            if (debugOutput)
            {
                string outputMessage = "";
                foreach (var str in message)
                {
                    outputMessage += str + " | ";
                }
                Debug.Log("Transmit message: " + outputMessage);
            }

            HoloRepair.Core.ContentAppInterface.SendNetworkMessage(message);
        }

        /// <summary>
        /// Receive routine for network messages.
        /// </summary>
        /// <param name="message"></param>
        public void ParseReceivedNetworkMessages(string[] message)
        {
            Debug.Log("Parsed network message.");

            if (!isReceiver)
            {
                return;
            }

            // Debug
            if (debugOutput)
            {
                string outputMessage = "Message received: ";
                foreach (var str in message)
                {
                    outputMessage += str + " | ";
                }
                Debug.Log(outputMessage);
            }


            // Datenflug stuff - at the beginning, because SceneManager could already be routed
            if (message[0] == messageChannelHandler.RoutedMessageKey)
            {
                if (message.Length == 3)
                {
                    var channelID = message[1];
                    if (!messageChannelHandler.FindChannel(channelID, out var channel))
                    {
#if UNITY_EDITOR
                        throw new Exception($"Unable to find routed message channel ({channel})");
#else
						Debug.LogError($"Unable to find routed message channel ({channel}), ignoring");
						return;
#endif
                    }
                    channel.Dispatch(message[2]);
                }
                else
                {
#if UNITY_EDITOR
                    throw new Exception($"Routed message has insufficient parameters, expected 3, got {message.Length}");
#else
					Debug.LogError($"Routed message has insufficient parameters, expected 3, got {message.Length}, ignoring");
					return;
#endif
                }
            }

            // SceneManager callbacks
            else if (message[0].Contains("SceneManager"))
            {
                if (message.Length == 3)
                {
                    if (bool.TryParse(message[2], out var state))
                    {
                        SceneManager.instance.SendMessage(message[1], state);
                    }
                    else
                    {
                        Debug.LogWarning("Received message or variable commands not implemented: " + message[1]);
                    }
                }
                else if (message.Length == 2)
                {
                    SceneManager.instance.SendMessage(message[1]);
                }
            }

            // Localizer callbacks
            else if (message[0] == nameof(LegacyLocalization.Localizer))
            {
                Debug.Log("In Localizer");

                LegacyLocalization.Localizer.instance.SetLanguage(message[1]);
            }

            // Synced Buttons
            else if (message[0] == nameof(SyncedButton))
            {
                if (message.Length == 2)
                {
                    SyncedButton.ClickButtonById(message[1]);

                    // Special case close detail view: Close tooltips
                    if (message[1] == closeDetailViewMessage)
                    {
                        Tooltip.CloseFocusedTooltipWithoutNotifyNetwork();
                    }
                }
                else
                {
                    Debug.LogWarning("Received message or variable commands not implemented: " + message[0]);
                }
            }

            // Synced Toggles
            else if (message[0] == nameof(SyncedToggle))
            {
                if (message.Length == 3)
                {
                    if (bool.TryParse(message[2], out var state))
                    {
                        SyncedToggle.SwitchToggleById(message[1], state);
                    }
                }
                else
                {
                    Debug.LogWarning("Received message or variable commands not implemented: " + message[0]);
                }
            }

            // Synced Sliders
            else if (message[0] == nameof(SyncedSlider))
            {
                if (message.Length == 3)
                {
                    if (float.TryParse(message[2], out var value))
                    {
                        SyncedSlider.SetSliderValueById(message[1], value);
                    }
                }
                else
                {
                    Debug.LogWarning("Received message or variable commands not implemented: " + message[0]);
                }
            }

            // Synced Tooltips (only used for zoom)
            else if (message[0] == nameof(Tooltip))
            {
                if (message.Length == 3)
                {
                    if (message[2] == "Focus")
                    {
                        Tooltip tooltip = Tooltip.GetTooltipByName(message[1]);
                        tooltip.ZoomToDetail();
                    }
                }
                else
                {
                    Debug.LogWarning("Received message or variable commands not implemented: " + message[0]);
                }
            }

            // Sync LB11
            else if (message[0] == nameof(Synced_LB11))
            {
                if (message.Length == 4)
                {
                    if (bool.TryParse(message[2], out var value))
                    {
                        LB11Sync.SwitchOnMessage(message, value);
                    }
                }
                else
                {
                    Debug.LogWarning("Received message or variable commands not implemented: " + message[0]);
                }
            }
        }
    }
}