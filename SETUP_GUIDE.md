# Quantower Custom Indicators — Setup Guide

## Files included
| File | Indicator |
|---|---|
| SmaEmaCross.cs | SMA + EMA cross, settable periods & colours |
| DualHma.cs | Two HMAs, settable periods & colours |
| TripleSma.cs | Three SMAs, settable periods & colours |
| TripleEmaHlc.cs | Three EMAs (Close/High/Low) + optional smoothing line |

---

## Step 1 — Get the Quantower Indicator SDK

1. Open Quantower.
2. Go to **Control Center → Settings → About** and note your version.
3. Download the matching SDK from:
   https://github.com/Quantower/QuantowerAPI
   (Clone or download the ZIP — you need `TradingPlatform.BusinessLayer.dll`)

---

## Step 2 — Create a Visual Studio project (do this once)

1. Open **Visual Studio** → **Create a new project**.
2. Choose **Class Library (.NET Framework)** — target **.NET 6** or whichever matches your Quantower install.
3. Name the project e.g. `MyQuantowerIndicators`.
4. Delete the default `Class1.cs`.
5. Right-click **References** → **Add Reference** → **Browse**.
6. Navigate to your Quantower install folder, typically:
   `C:\Program Files\Quantower\` and find `TradingPlatform.BusinessLayer.dll`
   Add it.
7. Make sure **Copy Local = False** for that reference (so you don't ship the DLL).

---

## Step 3 — Add the indicator files

1. Copy all four `.cs` files into the Visual Studio project folder.
2. In Solution Explorer, right-click the project → **Add → Existing Item** → select all four files.
3. Each file is a standalone public class — no namespace needed (Quantower discovers them by reflection).

---

## Step 4 — Build & deploy

1. **Build → Build Solution** (Ctrl+Shift+B).
2. This produces `MyQuantowerIndicators.dll` in the `bin\Release` (or `Debug`) folder.
3. Copy that single DLL to Quantower's indicators folder:
   `C:\Users\<YourName>\Documents\Quantower\Settings\Scripts\Indicators\`
   (Create the folder if it doesn't exist.)
4. **Restart Quantower** (or reload scripts if prompted).

---

## Step 5 — Load on a chart

1. Open a chart, click **Indicators** (the `f(x)` button).
2. Search for the indicator name, e.g. "Triple SMA" or "SMA/EMA Cross".
3. Double-click to add. All settings (periods, colours, line width) appear in the indicator panel.

---

## Indicator notes

### SMA/EMA Cross
- Separate period settings for the SMA and EMA.
- Colour picker per line; line width applies to both.

### Dual HMA
- HMA is computed manually as `WMA(sqrt(n), 2*WMA(n/2) - WMA(n))` — the standard Hull formula.
- No crossover dot/symbol by design.

### Triple SMA
- Three fully independent SMAs; all applied to Close.
- Independent colour per line.

### Triple EMA HLC
- All three EMAs share one period setting.
- EMA Close = white, EMA High = green, EMA Low = red (defaults — all changeable).
- Smooth Line = EMA of the Close EMA, with its own period setting.
- Toggle smooth line on/off via **Show Smooth Line** checkbox.
- Smooth line renders as a dashed line to distinguish it visually.

---

## Troubleshooting

| Problem | Fix |
|---|---|
| Indicator doesn't appear in search | Check DLL is in the correct Scripts\Indicators folder; restart Quantower |
| Build error: `TradingPlatform not found` | Re-add the DLL reference; check .NET target framework matches |
| Flat/missing line on chart | Increase bars loaded — some indicators need more history to warm up |
| Colour picker not showing | Quantower needs to be version 1.135+ for full InputParameter colour support |
