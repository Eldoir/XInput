# X360

Ever wondered how to handle Xbox 360 controllers very easily? Well, there you go!

X360 gets rid of all the complicated code so that you can focus on the essentials: interaction!

See [Code Usage](#code-usage) for some examples.

## Getting Started

For a quick import into an existing project, just get the [UnityPackage](X360Package.unitypackage).

The X360 folder is an empty project with only the plugin imported and some examples! :)

## Prerequisites

There are absolutely no prerequisites to this plugin.

Everything comes into a few files (and most of them are used for demo).

## Code Usage

You can get information from the controllers in 2 ways :
- The first, simplest, is to call methods directly, in Update() for example.

```csharp
public class ButtonScript : MonoBehaviour
{
	void Update()
	{
		if (X360.IsButtonPressed(X360.Button.A)) // Player 1 just pressed A this frame
		{
			// Do stuff for player 1
		}

		if (X360.IsButtonHold(X360.Button.A, 1)) // Player 2 is holding the A button
		{
			// Do stuff for player 2 (playerIndex = 1)
		}
	}
}
```

- The second, hardly more complex and much more efficient and professional, consists of registering for events, and to perform actions only when these events occur.

```csharp
public class ButtonScript : MonoBehaviour
{
	void Start()
	{
		X360.onButtonPressed += OnButtonPressed; // We listen to the OnButtonPressed event
	}

	void OnButtonPressed(X360.Button button, int playerIndex)
	{
		if (button == X360.Button.A && playerIndex == 0) // Player 1 pressed the A button
		{
			// Do stuff
		}
	}

	void OnDestroy()
	{
		X360.onButtonPressed -= OnButtonPressed; // We don't need to listen to the event anymore
	}
}
```

## Screenshots

![Demo Scene](Screenshots/DemoScene.png)

## Notes

* Last tested with [Unity 2018.3.8f1](https://unity3d.com/unity/whats-new/2018.3.8).

## Authors

* **[Arthur Cousseau](https://www.linkedin.com/in/arthurcousseau/)**

## License

This project is licensed under the MIT License - see the [LICENSE.md](LICENSE.md) file for details.
