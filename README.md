# EZCad
Probably, one of my all-time favourite projects. Now immortalised and discontinued.

> This project was made in 2022, and uses outdated packages and an outdated .NET version. Don't use this in production unless you want to maintain or extend it.

EZCad is a CAD software (like Sonoran CAD or ImperialCAD) that I built for a server I used to administrate. It came paired with a FiveM script which would help bridge the gap between the FiveM server itself, and the CAD software.

> You can find the sync script for your FiveM server in the `EzCadSync` folder.

## vMenu Integration

This CAD was integrated with a fork of vMenu to allow for custom role syncing, unit tracking, and permission mapping.
The original plan was to extend it further and build a fully connected ecosystem — including economy logic and call routing — but the project was never fully completed.

Still, the integration logic exists and gives a great starting point for anyone wanting to connect UI-based CAD systems with in-game menus.

## Features
This CAD featured a bunch of goodies which I looking back at I'm super proud of:

- Server login activity
- Jobs
- Salaries
- Vehicles
- Identities
- Policing system
- Banking and salary system
- Rank system
- Very comprehensive administrative panel
- Customisable at runtime
- Speed-limits
- Syncs with the FiveM game server (FxServer)
- Discord integration
- Still, quite secure, even though it depends on an unsupported .NET version

## Deploying
> This CAD is super buggy since it has been half-baked to work with my own economy system I started to develop and later scrapped. It still somewhat works though after some minor adjustments I personally made to it.

You can deploy it by publishing it for whatever OS/Arch you wish to run it on, it was typically deployed on a Linux x64 system.

```sh
dotnet publish -c Release
```

Usually does the trick. It doesn't migrate the database automatically, so you'll need to migrate it using the API endpoints:

```
HTTP GET to /migrate-db
```

And then, create an account with the username `harry`, and then go and call

```
HTTP GET to /setup-roles
```

That'll create all the default roles available and give the `harry` account all of them. For the sync script, check the `README.md` file inside the `EzCadSync` folder.

## License
Licensed under MIT. See LICENSE
