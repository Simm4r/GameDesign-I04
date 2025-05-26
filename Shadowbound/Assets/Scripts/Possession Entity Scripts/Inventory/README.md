# Sistema inventario

La cartella `Inventory` è organizzata come segue:

<pre>
Inventory/
├──Item/
|   ├──Items/
|   ├──InventoryItem.cs
|   ├──ItemData.cs
|   └──PickupItem.cs
├──Inventory.cs
└──README.md
</pre>

- La cartella `Item/` contiene tutte le informazioni per gli items che possono essere collezionati:
  - La cartella `Items/` contiene tutti asset di oggetti tipo `ItemData` (es. `Key_Rusty`).
  - `InventoryItem.cs` è la classe che rappresenta un item che viene collezionato in un inventario
  - `ItemData.cs` è uno script di tipo ScriptableObject che definisce quali sono le proprietà di un item (es. nome, icona, desc. ecc.). Serve a creare asset di items.
  - `PickupItem.cs` script da assegnare all'item raccoglibile
- `Inventory.cs` script da assegnare all'entity da possedere. Si occupa di gestire le funzionalità di un inventario (add item, remove item, trade, ecc.)
