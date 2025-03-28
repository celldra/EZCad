# EZCadSync
A companion FiveM script that assists with syncing player states and permissions with the EZCad FOSS FiveM CAD.

## Editing

Edit it, open `ezcadsync.sln` in Visual Studio or Rider (if you're fancy 😎)

## Compilation

To build it, run `build.cmd`. The `dist` folder can be copied to your server resources.

## Installation

Grab the latest version from the releases page.

Afterwards, you can use `ensure ezcadsync` in your `server.cfg` or server console to start the resource. You should make sure that this is after the `ensure chat` and such is ran.

Then you'll need to append the following lines to the end of your `server.cfg` file:

```ini
add_ace resource.ezcadsync command.add_principal allow 
add_ace resource.ezcadsync command.remove_principal allow
```