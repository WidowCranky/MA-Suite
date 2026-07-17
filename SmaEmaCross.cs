using System;
using System.Drawing;
using TradingPlatform.BusinessLayer;

/// <summary>
/// SMA/EMA Cross Indicator
/// Plots one SMA and one EMA with configurable periods and colours.
/// </summary>
public class SmaEmaCross : Indicator
{
    [InputParameter("SMA Period", 0, 1, 999, 1, 0)]
    public int SmaPeriod = 7;

    [InputParameter("EMA Period", 1, 1, 999, 1, 0)]
    public int EmaPeriod = 5;

    [InputParameter("SMA Color", 2)]
    public Color SmaColor = Color.DodgerBlue;

    [InputParameter("EMA Color", 3)]
    public Color EmaColor = Color.OrangeRed;

    [InputParameter("Line Width", 4, 1, 5, 1, 0)]
    public int LineWidth = 2;

    private Indicator sma;
    private Indicator ema;

    public override string ShortName => $"SMA/EMA Cross ({SmaPeriod}/{EmaPeriod})";

    public SmaEmaCross()
        : base()
    {
        Name = "SMA/EMA Cross";
        Description = "Plots an SMA and an EMA. Useful for identifying crossover signals.";
        SeparateWindow = false;

        AddLineSeries("SMA", Color.DodgerBlue, 2, LineStyle.Solid);
        AddLineSeries("EMA", Color.OrangeRed, 2, LineStyle.Solid);
    }

    protected override void OnInit()
    {
        sma = Core.Indicators.BuiltIn.SMA(SmaPeriod, PriceType.Close);
        ema = Core.Indicators.BuiltIn.EMA(EmaPeriod, PriceType.Close);

        AddIndicator(sma);
        AddIndicator(ema);

        // Apply user colour choices
        LinesSeries[0].Color = SmaColor;
        LinesSeries[0].Width = LineWidth;
        LinesSeries[1].Color = EmaColor;
        LinesSeries[1].Width = LineWidth;
    }

    protected override void OnUpdate(UpdateArgs args)
    {
        if (Count < Math.Max(SmaPeriod, EmaPeriod))
            return;

        SetValue(sma.GetValue(), 0);
        SetValue(ema.GetValue(), 1);
    }
}
