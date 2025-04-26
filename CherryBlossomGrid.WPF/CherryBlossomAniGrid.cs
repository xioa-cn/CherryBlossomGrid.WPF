using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media.Animation;
using System.Windows.Media;
using System.Windows;
using System.Windows.Shapes;

namespace CherryBlossomGrid.WPF
{
    public class CherryBlossomAniGrid : Grid
    {
        private Random random = new Random();
        private Canvas AnimationCanvas;

        public CherryBlossomAniGrid()
        {
            // 初始化画布
            AnimationCanvas = new Canvas();
            Panel.SetZIndex(AnimationCanvas, 9999); // 设置最高层级
            this.Children.Add(AnimationCanvas);

            // 注册鼠标点击事件
            this.PreviewMouseLeftButtonDown += MouseAnimationGrid_MouseLeftButtonDown;

            // 设置背景为透明
            this.Background = Brushes.Transparent;
        }

        private void MouseAnimationGrid_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            try
            {
                // 获取鼠标点击位置
                Point position = e.GetPosition(AnimationCanvas);

                // 创建樱花形状
                Path sakura = CreateSakura();
                if (sakura == null) return;

                // 设置樱花位置
                Canvas.SetLeft(sakura, position.X);
                Canvas.SetTop(sakura, position.Y);

                // 添加到画布
                AnimationCanvas.Children.Add(sakura);

                // 创建随机X轴动画
                double randomX = random.Next(-100, 100);

                // 确保RenderTransform已经初始化
                if (sakura.RenderTransform == null)
                {
                    sakura.RenderTransform = new TransformGroup();
                }

                // 创建和设置TranslateTransform
                TranslateTransform translateTransform = new TranslateTransform();
                ((TransformGroup)sakura.RenderTransform).Children.Add(translateTransform);

                // 创建动画
                var storyboard = new Storyboard();

                // Y轴动画
                var yAnimation = new DoubleAnimation
                {
                    From = 0,
                    To = 500,
                    Duration = TimeSpan.FromSeconds(3)
                };
                Storyboard.SetTarget(yAnimation, sakura);
                Storyboard.SetTargetProperty(yAnimation,
                    new PropertyPath(
                        "(UIElement.RenderTransform).(TransformGroup.Children)[3].(TranslateTransform.Y)"));

                // 旋转动画
                var rotateAnimation = new DoubleAnimation
                {
                    From = 0,
                    To = 360,
                    Duration = TimeSpan.FromSeconds(3)
                };
                Storyboard.SetTarget(rotateAnimation, sakura);
                Storyboard.SetTargetProperty(rotateAnimation,
                    new PropertyPath(
                        "(UIElement.RenderTransform).(TransformGroup.Children)[2].(RotateTransform.Angle)"));

                // 透明度动画
                var opacityAnimation = new DoubleAnimation
                {
                    From = 1,
                    To = 0,
                    Duration = TimeSpan.FromSeconds(3)
                };
                Storyboard.SetTarget(opacityAnimation, sakura);
                Storyboard.SetTargetProperty(opacityAnimation, new PropertyPath(UIElement.OpacityProperty));

                // 添加所有动画到故事板
                storyboard.Children.Add(yAnimation);
                storyboard.Children.Add(rotateAnimation);
                storyboard.Children.Add(opacityAnimation);

                // X轴动画
                translateTransform.BeginAnimation(TranslateTransform.XProperty, new DoubleAnimation
                {
                    From = 0,
                    To = randomX,
                    Duration = TimeSpan.FromSeconds(3)
                });

                // 动画完成后移除樱花
                storyboard.Completed += (s, _) => AnimationCanvas.Children.Remove(sakura);
                storyboard.Begin();
            }
            catch (Exception ex)
            {
                MessageBox.Show($"发生错误：{ex.Message}");
            }
        }

        private Path CreateSakura()
        {
            // 创建随机颜色
            Color randomColor = Color.FromRgb(
                (byte)random.Next(200, 255), // 保持较高的R值使颜色偏粉
                (byte)random.Next(150, 200),
                (byte)random.Next(150, 200)
            );

            // 创建樱花形状
            Path sakura = new Path
            {
                Fill = new SolidColorBrush(randomColor),
                Width = 40,
                Height = 40,
                RenderTransformOrigin = new Point(0.5, 0.5),
                RenderTransform = new TransformGroup
                {
                    Children = new TransformCollection
                    {
                        new ScaleTransform(),
                        new SkewTransform(),
                        new RotateTransform(random.Next(0, 360)),
                        new TranslateTransform()
                    }
                }
            };

            // 使用你提供的SVG路径数据
            string pathData =
                "M401.931514 125.446707C424.768056 26.530096 478.85672 14.103476 553.183916 0c5.544803 23.949974 21.003179 38.858341 46.379598 44.716157 25.376419 5.857817 47.913362-0.27724 67.601887-18.400698 56.771633 46.835703 96.752349 81.785852 74.631266 177.594689-19.992594 86.606253-131.952908 210.478952-192.127441 237.129782-3.581764-23.24793-14.291284-36.931074-32.128559-41.049432-17.837275-4.113886-32.839546 3.630952-45.006812 23.238987-43.191336-48.561747-91.444541-207.482969-70.597869-297.782778z M836.581506 314.148891c101.130061-8.844856 129.667913 38.755493 166.044506 105.082969-21.06131 12.672559-30.460646 31.980996-28.193537 57.92531 2.271581 25.948786 15.069345 45.485275 38.388821 58.609468-26.999616 68.460437-47.882061 117.29048-145.837275 125.858096-88.546934 7.74931-240.953013-60.456245-284.895581-109.447267 20.998707-10.593258 30.702114-25.005275 29.110219-43.240524-1.596367-18.235249-13.602655-30.107389-36.009922-35.625362 32.844017-56.083004 169.07179-151.08248 261.392769-159.158219z M792.044213 787.29055c39.663231 93.447825 3.215092 135.297677-48.624349 190.387983-18.566148-16.115703-39.833153-19.084856-63.809957-8.907459-23.972332 10.172926-38.598987 28.381345-43.875493 54.616315-73.455231-4.516332-126.345502-9.292017-164.765624-99.806463-34.731039-81.821624-16.960838-247.839301 16.0531-304.776385 16.562865 16.701485 33.268821 21.47717 50.122341 14.327057 16.849048-7.154585 24.432908-22.237345 22.75158-45.252751 63.488 13.902253 195.937258 114.10669 232.148402 199.411703z M329.866955 891.005205c-76.616664 66.600245-127.682515 44.863721-196.098235 12.587598 9.596087-22.635319 5.848873-43.777118-11.241642-63.438812-17.086044-19.652751-38.920943-27.938655-65.504699-24.848768-18.400699-71.259668-30.201293-123.036507 44.009642-187.544035 67.083179-58.314341 230.467074-92.718952 294.813624-78.910603-10.763179 20.913747-10.141624 38.277031 1.869136 52.094323 12.01076 13.812821 28.69883 16.366114 50.073153 7.646463 6.39441 64.681921-47.980437 221.613275-117.920979 282.413834z M88.761907 481.959686C1.744266 429.677555 6.636213 374.394969 16.196528 299.352314c24.491039 2.128489 43.441747-7.968419 56.856593-30.295196 13.414847-22.322306 14.546166-45.650725 3.389485-69.976315 62.088384-39.52014 107.680978-66.747808 191.966463-16.097816 76.19186 45.77593 159.399686 190.535546 166.151825 256-23.212157-3.774044-39.538026 2.182148-48.964192 17.873048-9.426166 15.686428-6.694009 32.35214 8.192 49.98372-59.535092 26.06952-225.58407 22.849956-305.026795-24.880069z M486.087322 547.035109a31.761886 31.761886 0 1 1 0-63.523773 31.761886 31.761886 0 0 1 0 63.523773z";
            var pa = Geometry.Parse(pathData);
            // 创建路径几何图形
            PathGeometry geometry = PathGeometry.CreateFromGeometry(pa);

            // 缩放到合适的大小
            double scale = 0.04; // 调整这个值可以改变樱花大小
            TransformGroup transformGroup = new TransformGroup();
            transformGroup.Children.Add(new ScaleTransform(scale, scale));
            geometry.Transform = transformGroup;

            sakura.Data = geometry;
            return sakura;
        }
    }
}
