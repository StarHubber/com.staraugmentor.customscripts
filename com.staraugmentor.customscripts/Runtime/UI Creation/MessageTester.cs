using NaughtyAttributes;
using StarCooperation;
using StarCooperation.Export;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;

public class MessageTester : MonoBehaviour
{
    public string TestGuid = "f4662aebfa709c341b065bb8956f9851";
    // Start is called before the first frame update
    void Start()
    {

    }


    // Update is called once per frame

    [Button]
    private void TestSendMessage()
    {
        FindObjectOfType<MessageReceiver>().OnUIItemClicked(TestGuid);
    }
    [Button]
    public void SendExtension()
    {
        FindObjectOfType<MessageReceiver>().OnUIExtensionClicked(TestGuid);
    }

    [Button]
    public void SendBackButtonMessage()
    {

        GameObject.Find("ButtonBack").GetComponent<Button>().onClick.Invoke();
    }
}
