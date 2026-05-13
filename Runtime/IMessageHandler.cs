using System;

public interface IMessageHandler
{
    event Action<string> UiClicked;
}