using System;
using System.Drawing;
using System.Windows.Forms;

public enum AnimationType { FadeIn, ZoomIn }

public static class AnimationHelper
{
    public static void Animate(Control ctrl, AnimationType type, int durationMs = 500)
    {
        var timer = new Timer { Interval = 15 };
        int steps = durationMs / timer.Interval;
        int current = 0;

        float initOpacity = ctrl.BackColor.A / 255f;

        var initialState = (opacity: initOpacity, scale: 1.0f, initialSize: ctrl.Size, initialLocation: ctrl.Location);
        ctrl.Tag = initialState;
        ctrl.Visible = true;

        timer.Tick += (s, e) =>
        {
            current++;
            if (current > steps)
            {
                timer.Stop();
                timer.Dispose();
                return;
            }

            var t = current / (float)steps;
            switch (type)
            {
                case AnimationType.FadeIn:
                    float opacity = initOpacity * t;
                    ctrl.BackColor = Color.FromArgb((int)(255 * opacity), ctrl.BackColor);
                    break;
                case AnimationType.ZoomIn:
                    float scale = 0.5f + 0.5f * t;
                    ctrl.Size = new Size((int)(initialState.initialSize.Width * scale), (int)(initialState.initialSize.Height * scale));
                    ctrl.Location = new Point(
                        initialState.initialLocation.X + (initialState.initialSize.Width - ctrl.Size.Width) / 2,
                        initialState.initialLocation.Y + (initialState.initialSize.Height - ctrl.Size.Height) / 2
                    );
                    break;
            }
        };
        timer.Start();
    }
}