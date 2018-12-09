BaseTreeRoutine
======

BaseTreeRoutine is an extension of PoeHUD's BasePluginSettings designed to allow quick and easy implementation of a plugin written within a behavior tree. Behavior trees allow for code to be easier to maintain, modular, and as complicated or simple as necessary.

### Credits
* PoeHUD and all their great contributors. https://github.com/TehCheat/PoEHUD
* Apoc for writing TreeSharp. https://github.com/ApocDev/TreeSharp
* Zaafar for some help with PoeHUD and giving me a good place to get started
* AutoFlaskManager and all their great contributors for giving me a good place to get started. https://github.com/Xcesius/AutoFlaskManager

### Donation Addresses
* BTC: 1MTKA5qFknUcG68LGZXnmCSvDvvjMbMvRK
* BCH: 17Eu4YCs7MUnHrBuSeUggbCsnHNX7n5fNZ
* ETH: 0x38B6e014F3923B8F9aFbE4e0ff0e872beCdb5d72
* LTC: LMXsH6DyKiqLw3JZea9ZdHDV4oXZ2Uek3W

[![paypal](https://www.paypalobjects.com/en_US/i/btn/btn_donateCC_LG.gif)](https://www.paypal.com/cgi-bin/webscr?cmd=_s-xclick&hosted_button_id=72QZ2RSUMGJ8N)

### Examples of public BaseTreeRoutine Implementations

* [BasicFlaskRoutine](https://github.com/sychotixdev/BasicFlaskRoutine)
 
### Plugin Development
* Requires [PoeHUD.](https://github.com/TehCheat/PoEHUD) Either add PoeHUD.exe as a reference, or download the repository and reference the project
* Requires [TreeSharp.](https://github.com/ApocDev/TreeSharp) Add this project as a reference, or add TreeSharp.dll as a reference from a completed plugin.
* All routines must implement BaseTreeRoutinePlugin.
* Place your behavior tree in BaseTreeRoutinePlugin.Tree and it will be ticked once the plugin is enabled.