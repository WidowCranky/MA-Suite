using System;
using System.Drawing;
using TradingPlatform.BusinessLayer;

/// <summary>
/// Dual HMA (Hull Moving Average) Indicator
/// HMA is calculated manually: HMA(n) = WMA(2*WMA(n/2) - WMA(n), sqrt(n))
/// Plots two HMAs with configurable periods and colours. No crossover symbol.
/// </summary>
public class DualHma : Indicator
{
    [InputParameter("HMA 1 Period", 0, 2, 999, 1, 0)]
    public int Hma1Period = 12;

    [InputParameter("HMA 2 Period", 1, 2, 999, 1, 0)]
    public int Hma2Period = 20;

    [InputParameter("HMA 1 Color", 2)]
    public Color Hma1Color = Color.Gray;

    [InputParameter("HMA 2 Color", 3)]
    public Color Hma2Color = Color.White;

    [InputParameter("Line Width", 4, 1, 5, 1, 0)]
    public int LineWidth = 2;

    public override string ShortName => $"Dual HMA ({Hma1Period}/{Hma2Period})";

    public DualHma() : base()
    {
        Name = "Dual HMA";
        Description = "Plots two Hull Moving Averages with configurable periods and colours.";
        SeparateWindow = false;

        AddLineSeries("HMA 1", Color.Cyan,    2, LineStyle.Solid);
        AddLineSeries("HMA 2", Color.Magenta, 2, LineStyle.Solid);
    }

    protected override void OnInit()
    {
        LinesSeries[0].Color = Hma1Color;
        LinesSeries[0].Width = LineWidth;
        LinesSeries[1].Color = Hma2Color;
        LinesSeries[1].Width = LineWidth;
    }

    protected override void OnUpdate(UpdateArgs args)
    {
        int needed = Math.Max(Hma1Period, Hma2Period);
        if (Count < needed + (int)Math.Round(Math.Sqrt(needed)))
            return;

        SetValue(CalculateHma(Hma1Period), 0);
        SetValue(CalculateHma(Hma2Period), 1);
    }

    /// <summary>
    /// Calculates HMA value at the current bar for a given period.
    /// HMA(n) = WMA( 2*WMA(n/2) - WMA(n), floor(sqrt(n)) )
    /// </summary>
    private double CalculateHma(int period)
    {
        int halfPeriod = Math.Max(1, period / 2);
        int sqrtPeriod = Math.Max(1, (int)Math.Floor(Math.Sqrt(period)));

        // Build the intermediate series: 2*WMA(half) - WMA(full), going back sqrtPeriod bars
        double[] intermediate = new double[sqrtPeriod];
        for (int i = 0; i < sqrtPeriod; i++)
        {
            double wmaHalf = CalculateWma(halfPeriod, i);
            double wmaFull = CalculateWma(period, i);
            intermediate[i] = 2.0 * wmaHalf - wmaFull;
        }

        // WMA of the intermediate series
        return CalculateWmaFromArray(intermediate, sqrtPeriod);
    }

    /// <summary>
    /// Weighted Moving Average over 'period' bars, offset bars back from current bar.
    /// </summary>
    private double CalculateWma(int period, int offset)
    {
        double weightedSum = 0.0;
        double weightSum   = 0.0;

        for (int i = 0; i < period; i++)
        {
            int barIndex = offset + i;
            if (barIndex >= Count) return double.NaN;

            double price  = GetPrice(PriceType.Close, barIndex);
            double weight = period - i;   // most recent bar gets highest weight
            weightedSum += price * weight;
            weightSum   += weight;
        }

        return weightSum == 0 ? double.NaN : weightedSum / weightSum;
    }

    /// <summary>
    /// WMA over a pre-built array (used for the final smoothing pass).
    /// index 0 = most recent value.
    /// </summary>
    private double CalculateWmaFromArray(double[] values, int period)
    {
        double weightedSum = 0.0;
        double weightSum   = 0.0;

        for (int i = 0; i < period && i < values.Length; i++)
        {
            if (double.IsNaN(values[i])) return double.NaN;
            double weight = period - i;
            weightedSum += values[i] * weight;
            weightSum   += weight;
        }

        return weightSum == 0 ? double.NaN : weightedSum / weightSum;
    }
}
