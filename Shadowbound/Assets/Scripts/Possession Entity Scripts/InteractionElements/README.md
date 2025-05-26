# Interactions Element

La cartella `InteractionElements/` è organizzata come segue:

<pre>
InteractionElements/
├──Door/
|   ├──DoorOpener.cs
|   └──LockedDoorOpener.cs
├──Lever/
|   ├──LeverSwitchController.cs
|   └──PullLeverHandler.cs
├──Portcullis/
|   └──PullLeverHandler.cs
└──README.md
</pre>

- La cartella `Door/` contiene gli script per interagire con le porte:
  - `DoorOpener.cs` è lo script che permette di aprire una porta.
  - `LockedDoorOpener.cs` è lo script che permette di aprire una porta chiusa a chiave.
- La cartella `Lever/` contiene gli script per interagire con le leve:
  - `PullLeverHandler.cs` è lo script che gestisce le interazioni della leva.
  - `LeverSwitchController.cs` è lo script che permette di gestire l'animazione leva.
- La cartella `Portcullis/` contiene gli script per interagire con le grate:
  - `PullLeverHandler.cs` è lo script che gestisce le interazioni del portone.

> `PullLeverHandler.cs` e `LeverSwitchController.cs` sono legati allo script `PullLeverHandler.cs`
