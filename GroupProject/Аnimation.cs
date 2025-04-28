        private void AnimateMove(UIElement figure, Point from, Point to)
        {
            DoubleAnimation moveX = new DoubleAnimation
            {
                From = from.X,
                To = to.X,
                Duration = TimeSpan.FromSeconds(0.5),
                EasingFunction = new CubicEase { EasingMode = EasingMode.EaseInOut }
            };

            DoubleAnimation moveY = new DoubleAnimation
            {
                From = from.Y,
                To = to.Y,
                Duration = TimeSpan.FromSeconds(0.5),
                EasingFunction = new CubicEase { EasingMode = EasingMode.EaseInOut }
            };

            figure.BeginAnimation(Canvas.LeftProperty, moveX);
            figure.BeginAnimation(Canvas.TopProperty, moveY);
        }
