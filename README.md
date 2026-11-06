# WakeBoard

[![Deploy](https://github.com/tzer0m/WakeBoard/actions/workflows/deploy.yml/badge.svg)](https://github.com/tzer0m/WakeBoard/actions/workflows/deploy.yml)

A small ASP.NET Core Razor Pages site that shows which machines on the home network are online and can wake them with Wake-on-LAN. It installs as a PWA, so it can live on a phone's home screen.

## Features

- Live Online and Offline status for every machine, by ping or by a TCP port check where ping is blocked, refreshed on a configurable interval.
- A Wake button for machines with a MAC address, sent with `etherwake` as a raw Ethernet frame.
- Pocket ID (OIDC) login.
- Installable PWA with a favicon and app icons.

## Configuration

Non-secret settings live in `appsettings.json` under `WakeBoard`:

| Key | Purpose |
| --- | --- |
| `TimeoutMs` | How long a status check waits before calling a machine offline. |
| `RefreshIntervalMs` | How often the page refreshes status. |
| `CommandTimeoutMs` | How long the wake command may run. |
| `SudoPath` | Path to `sudo`. |
| `EtherWakePath` | Path to `etherwake`. |

`Oidc:Authority` is also in `appsettings.json`. The machine list and the OIDC client go in user secrets, so addresses and credentials stay out of git:

```json
{
  "Oidc": {
    "ClientId": "<client id>",
    "ClientSecret": "<client secret>"
  },
  "WakeBoard": {
    "Machines": [
      { "Name": "Example", "Host": "192.0.2.10", "Port": 22 },
      { "Name": "Wakeable", "Host": "192.0.2.20", "Mac": "00:00:00:00:00:00", "Interface": "eth0" }
    ]
  }
}
```

| Machine Field | Purpose |
| --- | --- |
| `Name` | Display name. |
| `Host` | Hostname or IP address used for status checks. |
| `Port` | Optional TCP port to check instead of pinging. |
| `Mac` | MAC address for Wake-on-LAN. A machine without one has no Wake button. |
| `Interface` | Network interface on the host that is on the target's VLAN. |

## Wake on LAN

Waking runs `sudo -n etherwake -i <Interface> <Mac>` on the host, so it needs:

- `etherwake` installed, and passwordless sudo for it.
- A host interface on the same VLAN as the target, because the packet is a layer 2 frame.
- Wake-on-LAN enabled on the target, both in its BIOS and on its NIC (`ethtool <nic>` should show `Wake-on: g`).

## Power Off And Sleep

Power off and sleep run a Semaphore task, so no SSH keys live on the WakeBoard host. Create a Semaphore playbook template that takes an extra var `power_action` (`shutdown` or `sleep`) and runs against the `--limit` host, then add to user secrets:

```json
{
  "Semaphore": {
    "BaseUrl": "https://semaphore.example.com",
    "Token": "<api token>",
    "ProjectId": 1,
    "TemplateId": 1
  }
}
```

Give each machine a `SemaphoreHost` matching its inventory hostname to show the Sleep and Power Off buttons, and a `SemaphoreInventoryId` (the number in the inventory's Semaphore URL) so each machine uses its own inventory and credentials. The template needs the Inventory and Limit prompts enabled. Set `"CanSleep": true` on machines that support sleep, otherwise only Power Off is shown. Windows machines need to be in an inventory group called `windows`.

## Deployment

Pushes to `master` publish and deploy to a systemd service through `.github/workflows/deploy.yml`, behind a Cloudflare Tunnel. Secrets are passed as `Environment=` lines in the unit, generated from user secrets with SecretEnv.

## Credits

Wakeboard icons created by Nikita Golubev - [Flaticon](https://www.flaticon.com/free-icons/wakeboard).
