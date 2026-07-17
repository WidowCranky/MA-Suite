using System;
using System.Drawing;
using TradingPlatform.BusinessLayer;

/// <summary>
/// Triple SMA Indicator
/// Plots three Simple Moving Averages with configurable periods and colours.
/// </summary>
public class TripleSma : Indicator
{
    [InputParameter("SMA 1 Period", 0, 1, 999, 1, 0)]
    public int Sma1Period = 14;

    [InputParameter("SMA 2 Period", 1, 1, 999, 1, 0)]
    public int Sma2Period = 21;

    [InputParameter("SMA 3 Period", 2, 1, 999, 1, 0)]
    public int Sma3Period = 35;

    [InputParameter("SMA 1 Color", 3)]
    public Color Sma1Color = Color.FromArgb(0xb8, 0x7e, 0x9e);

    [InputParameter("SMA 2 Color", 4)]
    public Color Sma2Color = Color.FromArgb(0x9b, 0x52, 0x77);

    [InputParameter("SMA 3 Color", 5)]
    public Color Sma3Color = Color.FromArgb(0x6b, 0x22, 0x46);

    [InputParameter("Line Width", 6, 1, 5, 1, 0)]
    public int LineWidth = 1;

    private Indicator sma1;
    private Indicator sma2;
    private Indicator sma3;

    public override string ShortName => $"Triple SMA ({Sma1Period}/{Sma2Period}/{Sma3Period})";

    public TripleSma()
        : base()
    {
        Name = "Triple SMA";
        Description = "Plots three Simple Moving Averages with configurable periods and colours.";
        SeparateWindow = false;

        AddLineSeries("SMA 1", Color.Yellow, 2, LineStyle.Solid);
        AddLineSeries("SMA 2", Color.Orange, 2, LineStyle.Solid);
        AddLineSeries("SMA 3", Color.Red, 2, LineStyle.Solid);
    }

    protected override void OnInit()
    {
        sma1 = Core.Indicators.BuiltIn.SMA(Sma1Period, PriceType.Close);
        sma2 = Core.Indicators.BuiltIn.SMA(Sma2Period, PriceType.Close);
        sma3 = Core.Indicators.BuiltIn.SMA(Sma3Period, PriceType.Close);

        AddIndicator(sma1);
        AddIndicator(sma2);
        AddIndicator(sma3);

        LinesSeries[0].Color = Sma1Color;
        LinesSeries[0].Width = LineWidth;
        LinesSeries[1].Color = Sma2Color;
        LinesSeries[1].Width = LineWidth;
        LinesSeries[2].Color = Sma3Color;
        LinesSeries[2].Width = LineWidth;
    }

    protected override void OnUpdate(UpdateArgs args)
    {
        int minBars = Math.Max(Sma1Period, Math.Max(Sma2Period, Sma3Period));
        if (Count < minBars)
            return;

        SetValue(sma1.GetValue(), 0);
        SetValue(sma2.GetValue(), 1);
        SetValue(sma3.GetValue(), 2);
    }
}
