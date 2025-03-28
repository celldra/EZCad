lua54 'yes'

fx_version 'bodacious'
game 'gta5'

files {
    'Cad/*.dll',
    'Cad/sync-config.json',
    'Commands/*.dll',
    'Commands/sync-config.json',
    'Hud/*.html',
    'Hud/dist/*'
}


ui_page 'Hud/index.html'

client_scripts {
    'Cad/EzCadSync.Client.net.dll',
    'Commands/GallagherCommands.Client.net.dll'
}

server_scripts {
    'Cad/EzCadSync.Server.net.dll',
    'Commands/GallagherCommands.Server.net.dll'
}

name 'cadsync'
author 'celldra'
version '1.1.0'
description 'CAD syncing utilities for the FOSS FiveM CAD software, EZCad. This is bundled with commands and the HUD'