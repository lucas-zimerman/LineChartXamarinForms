using Charts.Query;
using SkiaSharp;
using SkiaSharp.Views.Forms;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace Charts.CustomControls
{
    public class LineCharts : SKCanvasView
    {
        private bool _animate = true;
        private int _animationTotalFrames = 10;
        internal double _animationActualRange = 0.0;
        internal int _animationFrame = 0;
        private double _maxValue;
        private double _maxRange;

        public LineCharts()
        {
            this.PaintSurface += new EventHandler<SKPaintSurfaceEventArgs>(this.OnCanvasViewPaintSurface);
        }
        
        public static readonly BindableProperty ChartDataProperty = BindableProperty.Create(
            nameof(ChartData),
            typeof(ChartData),
            typeof(LineCharts), 
            null,
            BindingMode.OneWay,
            propertyChanged: ChartDataPropertyChanged);


        private static void ChartDataPropertyChanged(BindableObject bindable, object oldValue, object newValue)
        {
            LineCharts lineCharts = (LineCharts)bindable;
            if (!(newValue is ChartData chartData))
                return;
            lineCharts.ChartData = chartData;
            if (chartData != null)
            {
                lineCharts.GetMaxRange();
                lineCharts.GetMaxValue();
            }
            lineCharts.Render();
        }

        public ChartData ChartData
        {
            get
            {
                return (ChartData)((BindableObject)this).GetValue(LineCharts.ChartDataProperty);
            }
            set
            {
                ((BindableObject)this).SetValue(LineCharts.ChartDataProperty, (object)value);
            }
        }


        private void GetMaxValue()
        {
            double num1 = this.ChartData.Data[0][0].Value;
            int index = 0;
            while (true)
            {
                int num2 = index;
                int? count = this.ChartData?.Data.Count;
                int valueOrDefault = count.GetValueOrDefault();
                if (num2 < valueOrDefault & count.HasValue)
                {
                    double num3 = this.ChartData.Data[index].Max<ChartEntry>((Func<ChartEntry, double>)(q => q.Value));
                    if (num3 > num1)
                        num1 = num3;
                    ++index;
                }
                else
                    break;
            }
            this._maxValue = num1 + num1 * 0.2;
        }

        private void GetMaxRange()
        {
            double num1 = this.ChartData.Data[0][0].LabelValue;
            int index = 0;
            while (true)
            {
                int num2 = index;
                int? count = this.ChartData?.Data.Count;
                int valueOrDefault = count.GetValueOrDefault();
                if (num2 < valueOrDefault & count.HasValue)
                {
                    double num3 = this.ChartData.Data[index].Max<ChartEntry>((Func<ChartEntry, double>)(q => q.LabelValue));
                    if (num3 > num1)
                        num1 = num3;
                    ++index;
                }
                else
                    break;
            }
            this._maxRange = num1;
        }
        
        private async void Render()
        {
            if (!this._animate)
            {
                this._animationActualRange = this._maxRange;
                this._animationFrame = this._animationTotalFrames;
                this.InvalidateSurface();
            }
            else
            {
                while (true)
                {
                    for (this._animationActualRange = 0.0; this._animationActualRange < this._maxRange; ++this._animationActualRange)
                    {
                        for (this._animationFrame = 1; this._animationFrame < this._animationTotalFrames; ++this._animationFrame)
                        {
                            this.InvalidateSurface();
                            await Task.Delay(30);
                        }
                    }
                    await Task.Delay(10000);
                }
            }
        }

        private void OnCanvasViewPaintSurface(object sender, SKPaintSurfaceEventArgs args)
        {
            SKImageInfo info = args.Info;
            SKCanvas canvas = args.Surface.Canvas;
            double num1 = (double)info.Height * 0.8;
            double num2 = (double)info.Width * 0.8;
            canvas.Clear();
            if (this.ChartData == null)
                return;
            float radius = 6f;
            int index1 = 0;
            for (int index2 = 0; index2 < 6; ++index2)
            {
                float y = (float)(num1 - (double)index2 * num1 / 6.0);
                canvas.DrawText(((int)(this._maxValue - this._maxValue * (double)(5 - index2) / 5.0)).ToString(), 0.0f, y, new SKPaint()
                {
                    TextSize = (float)num2 / 30f,
                    IsAntialias = true,
                    Style = SKPaintStyle.Fill,
                    Color = ((Color)Color.Black).ToSKColor()
                });
            }
            for (int index2 = 0; index2 < 6; ++index2)
            {
                float num3 = (float)(num1 - (double)index2 * num1 / 6.0);
                canvas.DrawLine((float)(num2 / this._maxRange), num3, (float)num2, num3, new SKPaint()
                {
                    Style = SKPaintStyle.Fill,
                    Color = ((Color)Color.Gray).ToSKColor()
                });
            }
            foreach (List<ChartEntry> chartEntryList in this.ChartData.Data)
            {
                for (int index2 = 0; index2 < chartEntryList.Count; ++index2)
                {
                    SKPaint paint = new SKPaint();
                    paint.Style = SKPaintStyle.Fill;
                    paint.Color = !chartEntryList[index2].Color.HasValue ? this.ChartData.Colors[index1].ToSKColor() : chartEntryList[index2].Color.Value.ToSKColor();
                    float cx = (float)(chartEntryList[index2].LabelValue * num2 / this._maxRange);
                    float cy = (float)(num1 - chartEntryList[index2].Value * num1 / this._maxValue);
                    canvas.DrawCircle(cx, cy, radius, paint);
                }
                ++index1;
            }
            int index3 = 0;
            foreach (List<ChartEntry> source in this.ChartData.Data)
            {
                for (int i = 0; (double)i < this._animationActualRange; i++)
                {
                    SKPaint paint = new SKPaint();
                    paint.Style = SKPaintStyle.Stroke;
                    paint.StrokeWidth = (float)num2 / 141f;
                    paint.Color = this.ChartData.Colors[index3].ToSKColor().WithAlpha((byte)128);
                    ChartEntry chartEntry1 = source.FirstOrDefault<ChartEntry>((Func<ChartEntry, bool>)(q => q.LabelValue == (double)(i + 1)));
                    ChartEntry chartEntry2 = source.FirstOrDefault<ChartEntry>((Func<ChartEntry, bool>)(q => q.LabelValue == (double)(i + 2)));
                    if (chartEntry1 == null)
                        chartEntry1 = new ChartEntry()
                        {
                            Value = 0.0,
                            LabelValue = (double)(i + 1)
                        };
                    if (chartEntry2 == null)
                        chartEntry2 = new ChartEntry()
                        {
                            Value = 0.0,
                            LabelValue = (double)(i + 2)
                        };
                    float x0 = (float)(chartEntry1.LabelValue * num2 / this._maxRange);
                    float y0 = (float)(num1 - chartEntry1.Value * num1 / this._maxValue);
                    if ((double)i < this._maxRange)
                    {
                        float x1 = (float)(chartEntry2.LabelValue * num2 / this._maxRange);
                        float y1 = (float)(num1 - chartEntry2.Value * num1 / this._maxValue);
                        if (this._animationActualRange == (double)(i + 1))
                        {
                            x1 = x0 + (float)this._animationFrame * (x1 - x0) / (float)this._animationTotalFrames;
                            y1 = y0 + (float)this._animationFrame * (y1 - y0) / (float)this._animationTotalFrames;
                        }
                        canvas.DrawLine(x0, y0, x1, y1, paint);
                    }
                }
                ++index3;
            }
        }

        private SKPoint GetNextLinePositionIncrement(
          float x0,
          float y0,
          float x1,
          float y1,
          int steps)
        {
            return new SKPoint((x1 - x0) / (float)steps, (y1 - y0) / (float)steps);
        }
    }
}