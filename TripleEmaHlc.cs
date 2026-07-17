using System;
using System.Drawing;
using TradingPlatform.BusinessLayer;

/// <summary>
/// Triple EMA — Close / High / Low with optional Smoothing Line (Wilder's SMMA)
/// All three EMAs share the same period; applied to Close, High, and Low respectively.
/// The smoothing line is a true Wilder's SMMA applied directly to Close price,
/// seeded with a simple average of the first SmoothPeriod bars.
/// </summary>
public class TripleEmaHlc : Indicator
{
    [InputParameter("EMA Period", 0, 1, 999, 1, 0)]
    public int EmaPeriod = 21;

    [InputParameter("Smoothing Period", 1, 1, 999, 1, 0)]
    public int SmoothPeriod = 21;

    [InputParameter("EMA Close Color", 2)]
    public Color EmaCloseColor = Color.DarkOrange;

    [InputParameter("EMA High Color", 3)]
    public Color EmaHighColor = Color.LightGray;

    [InputParameter("EMA Low Color", 4)]
    public Color EmaLowColor = Color.LightGray;

    [InputParameter("Smooth Line Color", 5)]
    public Color SmoothColor = Color.DodgerBlue;

    [InputParameter("Show Smooth Line", 6)]
    public bool ShowSmooth = true;

    [InputParameter("Line Width", 7, 1, 5, 1, 0)]
    public int LineWidth = 1;

    private Indicator emaClose;
    private Indicator emaHigh;
    private Indicator emaLow;

    private double smoothValue = double.NaN;
    private bool smoothSeeded = false;
    private int lastBarIndex = -1;

    public override string ShortName => $"Triple EMA HLC ({EmaPeriod}, S:{SmoothPeriod})";

    public TripleEmaHlc() : base()
    {
        Name = "Triple EMA HLC";
        Description = "Three EMAs on Close, High and Low plus Wilder's SMMA smoothing line.";
        SeparateWindow = false;

        AddLineSeries("EMA Close", Color.DarkOrange, 2, LineStyle.Solid);
        AddLineSeries("EMA High",  Color.LightGray,  2, LineStyle.Solid);
        AddLineSeries("EMA Low",   Color.LightGray,  2, LineStyle.Solid);
        AddLineSeries("Smooth",    Color.DodgerBlue, 2, LineStyle.Solid);
    }

    protected override void OnInit()
    {
        emaClose = Core.Indicators.BuiltIn.EMA(EmaPeriod, PriceType.Close);
        emaHigh  = Core.Indicators.BuiltIn.EMA(EmaPeriod, PriceType.High);
        emaLow   = Core.Indicators.BuiltIn.EMA(EmaPeriod, PriceType.Low);

        AddIndicator(emaClose);
        AddIndicator(emaHigh);
        AddIndicator(emaLow);

        smoothValue  = double.NaN;
        smoothSeeded = false;
        lastBarIndex = -1;

        LinesSeries[0].Color = EmaCloseColor;
        LinesSeries[0].Width = LineWidth;
        LinesSeries[1].Color = EmaHighColor;
        LinesSeries[1].Width = LineWidth;
        LinesSeries[2].Color = EmaLowColor;
        LinesSeries[2].Width = LineWidth;
        LinesSeries[3].Color = SmoothColor;
        LinesSeries[3].Width = LineWidth;
        LinesSeries[3].Visible = ShowSmooth;
    }

    protected override void OnUpdate(UpdateArgs args)
    {
        if (Count < EmaPeriod)
            return;

        SetValue(emaClose.GetValue(), 0);
        SetValue(emaHigh.GetValue(),  1);
        SetValue(emaLow.GetValue(),   2);

        if (!ShowSmooth) return;

        // Only advance the SMMA calculation once per bar, not on every tick
        bool isNewBar = (Count != lastBarIndex);

        if (!smoothSeeded)
        {
            if (Count < SmoothPeriod)
                return;

            // Seed with simple average of the first SmoothPeriod bars
            double sum = 0;
            for (int i = 0; i < SmoothPeriod; i++)
                sum += GetPrice(PriceType.Close, i);

            smoothValue  = sum / SmoothPeriod;
            smoothSeeded = true;
            lastBarIndex = Count;
        }
        else if (isNewBar)
        {
            // New bar — advance Wilder's SMMA using the previous bar's close
            double prevClose = GetPrice(PriceType.Close, 1);
            smoothValue  = (smoothValue * (SmoothPeriod - 1) + prevClose) / SmoothPeriod;
            lastBarIndex = Count;
        }

        SetValue(smoothValue, 3);
    }
}
