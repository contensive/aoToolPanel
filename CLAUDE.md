# CLAUDE.md

This file provides guidance to Claude Code (claude.ai/code) when working with code in this repository.

## Project Overview

aoToolPanel is a **Contensive CMS addon collection** that renders a floating tool panel at the top of public-facing website pages. It provides login, account management, and content editing UI for the [Contensive5](https://github.com/contensive/Contensive5) platform — an enterprise addon execution framework with hardware abstraction and modular addon architecture.

The addon executes at **OnBodyEnd** (after all body HTML), wrapping page content in a tool panel container. It is role-aware: unauthenticated users see a Login tab, authenticated content managers see Edit/Account tabs.

## Build

```bash
scripts/build.cmd
```

This cleans, compiles the C# solution (MSBuild 2019, .NET Framework 4.7.2), copies UI files from `/ui/` into `/collections/aoToolPanel/`, zips the collection, and deploys to `C:\Deployments\aoToolPanel\Dev\{version}/`. Version format: `YY.MM.DD.revision`.

**Requirements:** MSBuild 2019 BuildTools, 7-Zip, NuGet packages at `C:\NuGetLocalPackages\`.

There are no tests.

## Architecture

### Server-Side (C#)

- **Entry point:** `source/aoToolPanel/Addons/ToolPanelClass.cs` — main addon class inheriting `CPAddonBaseClass`. Determines auth state, configures visible tabs, injects initial JS variable values (e.g. `isLockedAccountTab`, `tpLoginTabIsPinned`) via `cp.Doc.AddHeadJavascript()`, and sets edit tab position via `cp.Doc.AddHeadStyle()`.
- **Constants:** `source/aoToolPanel/constants.cs` — GUIDs and site/user property name constants.
- **Form addons:** `source/aoToolPanel/Addons/` — separate classes for account form, login form, registration form, and their handlers. Each is a remote method addon called via `cj.ajax.addonCallback()`.
- **Models:** `source/aoToolPanel/Models/Db/personModel.cs` — user data model with factory `create(cp, userId)`.

### Client-Side (UI)

All UI source files live in `/ui/` and are copied to `/collections/` during build:

- **`ui/toolpanel.html`** — main layout template with three tab groups: Account/Login tabs (right-aligned, `position: absolute`), Edit tab (center, `position: fixed`, draggable on x-axis).
- **`ui/toolPanel.js`** — jQuery-based event handlers for tab auto-hide/pin, form submission, draggable panel, and remote method calls.
- **`ui/toolPanel.css`** — positioning, z-index layering (9999+), gradient styling, opacity hover effects.

### Tab Auto-Hide Pattern

Account, Login, and Edit tabs share this pattern:
1. A 5px-tall invisible zone at the top of the page triggers `slideDown("slow")` on mouseenter
2. On mouseleave, a 2-second `setTimeout` calls the close function which runs `slideUp("slow")`
3. A lock icon toggles pinning — when pinned, auto-hide is disabled
4. Pin state persists server-side via `cj.ajax.addon('setUserProperty', 'n={property}&v={0|1}')` and is restored in `ToolPanelClass.cs` via `jsOnReady`

### Server-Client Initialization Flow

`ToolPanelClass.cs` builds a `jsOnReady` string that sets initial JS variable values and lock icon states, then injects it via `cp.Doc.AddHeadJavascript()`. The edit tab's horizontal position is set via `cp.Doc.AddHeadStyle()` with `display:block`. This means the server controls initial visibility — the CSS default is `display:none`.

## Key Contensive APIs Used

- `cp.User.GetBoolean/SetProperty` — per-user persistent properties (lock states)
- `cp.Visit.GetInteger` — per-visit properties (panel position)
- `cp.Site.GetBoolean` — site-wide settings (tab visibility defaults)
- `cp.Cache.GetText/Store` — layout HTML caching
- `cp.WwwFiles.Read` — reads files from the web root
- `cj.ajax.addon(addonName, qs)` — client-side remote method call (fire-and-forget)
- `cj.ajax.addonCallback(addonName, qs, callback)` — client-side remote method call with response callback

## Collection Manifest

`collections/aoToolPanel/aoToolPanel.xml` defines all addons, their GUIDs, execution contexts (RemoteMethod, OnBodyEnd, ProcessRunOnce), included resources, site properties, and settings forms. This file is the source of truth for what gets installed on a Contensive site.

## Directory Purposes

- `/ui/` — active UI source files (HTML, JS, CSS, images)
- `/source/` — C# solution and addon source code
- `/collections/` — build output: XML manifest + generated zip package
- `/etc/` — archived/legacy files, not used in current build
- `/help/` — markdown help docs, zipped into collection during build
- `/scripts/` — build and deployment batch files
