using System;
using System.Collections.Generic;
using System.Data;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Text.RegularExpressions;
using System.Windows.Forms;

namespace GoldenRatioSolver
{
    public partial class Form1 : Form
    {
        private MenuStrip menuStrip;
        private ToolStripMenuItem calculateMenuItem;
        private ToolStripMenuItem clearMenuItem;
        private ToolStripMenuItem exitMenuItem;
        private ToolStripMenuItem aboutMenuItem;

        private TextBox txtA;
        private TextBox txtB;
        private TextBox txtEpsilon;
        private TextBox txtFunction;
        private TextBox txtResult;

        private Label lblA;
        private Label lblB;
        private Label lblEpsilon;
        private Label lblFunction;
        private Label lblResult;
        private Label lblMode;

        private PictureBox graphPictureBox;
        private Panel inputPanel;
        private Panel resultPanel;
        private RadioButton radioMin;
        private RadioButton radioMax;
        private GroupBox groupBoxMode;

        private const double GOLDEN_RATIO = 0.61803398874989484820458683436564;

        // ИЗМЕНЕННАЯ ЦВЕТОВАЯ СХЕМА - теплые тона
        private Color primaryColor = Color.FromArgb(217, 85, 89);     // Теплый красный
        private Color secondaryColor = Color.FromArgb(237, 125, 49);   // Оранжевый
        private Color accentColor = Color.FromArgb(255, 184, 28);      // Золотисто-желтый
        private Color dangerColor = Color.FromArgb(192, 57, 43);       // Темно-красный
        private Color darkColor = Color.FromArgb(87, 61, 51);          // Теплый коричневый
        private Color lightColor = Color.FromArgb(253, 245, 230);      // Кремовый/бежевый
        private Color panelColor = Color.FromArgb(255, 250, 240);      // Светлый бежевый фон

        public Form1()
        {
            InitializeComponent();
            InitializeCustomComponents();
            ApplyColorScheme();
        }

        private void InitializeCustomComponents()
        {
            this.Text = "Метод золотого сечения";
            this.Size = new Size(1000, 700);
            this.StartPosition = FormStartPosition.CenterScreen;
            this.BackColor = lightColor;
            this.Font = new Font("Segoe UI", 9);

            menuStrip = new MenuStrip();
            menuStrip.BackColor = darkColor;
            menuStrip.ForeColor = Color.White;

            calculateMenuItem = new ToolStripMenuItem("Рассчитать");
            calculateMenuItem.Click += CalculateMenuItem_Click;

            clearMenuItem = new ToolStripMenuItem("Очистить");
            clearMenuItem.Click += ClearMenuItem_Click;

            aboutMenuItem = new ToolStripMenuItem("Справка");
            aboutMenuItem.Click += AboutMenuItem_Click;

            exitMenuItem = new ToolStripMenuItem("Выход");
            exitMenuItem.Click += (s, e) => Application.Exit();

            menuStrip.Items.AddRange(new ToolStripItem[] {
                calculateMenuItem,
                clearMenuItem,
                aboutMenuItem,
                exitMenuItem
            });

            this.Controls.Add(menuStrip);
            this.MainMenuStrip = menuStrip;

            // Основной контейнер для левой части
            Panel leftContainer = new Panel();
            leftContainer.Location = new Point(20, 50);
            leftContainer.Size = new Size(320, 600);
            leftContainer.BackColor = Color.Transparent;

            // Панель ввода данных
            inputPanel = new Panel();
            inputPanel.Location = new Point(0, 0);
            inputPanel.Size = new Size(320, 400);
            inputPanel.BackColor = Color.White;
            inputPanel.BorderStyle = BorderStyle.FixedSingle;
            inputPanel.Padding = new Padding(15);

            // Заголовок панели ввода
            Label inputHeader = new Label();
            inputHeader.Text = "Параметры оптимизации";
            inputHeader.Font = new Font("Segoe UI", 12, FontStyle.Bold);
            inputHeader.ForeColor = darkColor;
            inputHeader.Location = new Point(15, 15);
            inputHeader.Size = new Size(290, 25);
            inputPanel.Controls.Add(inputHeader);

            int yPos = 50;

            // Поле функции
            lblFunction = new Label();
            lblFunction.Text = "Функция f(x):";
            lblFunction.Location = new Point(15, yPos);
            lblFunction.Size = new Size(100, 25);
            lblFunction.Font = new Font("Segoe UI", 9);
            inputPanel.Controls.Add(lblFunction);

            txtFunction = new TextBox();
            txtFunction.Location = new Point(120, yPos);
            txtFunction.Size = new Size(175, 25);
            txtFunction.Font = new Font("Segoe UI", 9);
            txtFunction.Text = "";
            txtFunction.BorderStyle = BorderStyle.FixedSingle;
            inputPanel.Controls.Add(txtFunction);

            yPos += 45;

            // Режим поиска
            lblMode = new Label();
            lblMode.Text = "Режим поиска:";
            lblMode.Location = new Point(15, yPos);
            lblMode.Size = new Size(100, 25);
            lblMode.Font = new Font("Segoe UI", 9);
            inputPanel.Controls.Add(lblMode);

            groupBoxMode = new GroupBox();
            groupBoxMode.Location = new Point(120, yPos - 5);
            groupBoxMode.Size = new Size(175, 60);
            groupBoxMode.Text = "";
            groupBoxMode.BackColor = Color.White;

            radioMin = new RadioButton();
            radioMin.Text = "Минимум";
            radioMin.Location = new Point(10, 5);
            radioMin.Size = new Size(75, 25);
            radioMin.Font = new Font("Segoe UI", 9);
            radioMin.Checked = true;
            groupBoxMode.Controls.Add(radioMin);

            radioMax = new RadioButton();
            radioMax.Text = "Максимум";
            radioMax.Location = new Point(10, 30);
            radioMax.Size = new Size(85, 25);
            radioMax.Font = new Font("Segoe UI", 9);
            groupBoxMode.Controls.Add(radioMax);

            inputPanel.Controls.Add(groupBoxMode);

            yPos += 65;

            // Поля ввода интервала и точности
            string[] labels = { "Начало интервала (a):", "Конец интервала (b):", "Точность (ε):" };
            TextBox[] textBoxes = new TextBox[3];
            textBoxes[0] = txtA = new TextBox();
            textBoxes[1] = txtB = new TextBox();
            textBoxes[2] = txtEpsilon = new TextBox();

            string[] defaultValues = { "", "", "" };

            for (int i = 0; i < 3; i++)
            {
                Label label = new Label();
                label.Text = labels[i];
                label.Location = new Point(15, yPos);
                label.Size = new Size(100, 25);
                label.Font = new Font("Segoe UI", 9);
                inputPanel.Controls.Add(label);

                textBoxes[i].Location = new Point(120, yPos);
                textBoxes[i].Size = new Size(175, 25);
                textBoxes[i].Font = new Font("Segoe UI", 9);
                textBoxes[i].Text = defaultValues[i];
                textBoxes[i].BorderStyle = BorderStyle.FixedSingle;
                inputPanel.Controls.Add(textBoxes[i]);

                yPos += 45;
            }

            // Панель результатов
            resultPanel = new Panel();
            resultPanel.Location = new Point(0, 420);
            resultPanel.Size = new Size(320, 180);
            resultPanel.BackColor = Color.White;
            resultPanel.BorderStyle = BorderStyle.FixedSingle;
            resultPanel.Padding = new Padding(15);

            // Заголовок панели результатов
            Label resultHeader = new Label();
            resultHeader.Text = "Результаты расчета";
            resultHeader.Font = new Font("Segoe UI", 12, FontStyle.Bold);
            resultHeader.ForeColor = darkColor;
            resultHeader.Location = new Point(15, 15);
            resultHeader.Size = new Size(290, 25);
            resultPanel.Controls.Add(resultHeader);

            txtResult = new TextBox();
            txtResult.Location = new Point(15, 45);
            txtResult.Size = new Size(290, 120);
            txtResult.Multiline = true;
            txtResult.ScrollBars = ScrollBars.Vertical;
            txtResult.ReadOnly = true;
            txtResult.Font = new Font("Consolas", 9);
            txtResult.BackColor = Color.FromArgb(255, 250, 245);
            txtResult.BorderStyle = BorderStyle.FixedSingle;
            resultPanel.Controls.Add(txtResult);

            // График
            graphPictureBox = new PictureBox();
            graphPictureBox.Location = new Point(360, 50);
            graphPictureBox.Size = new Size(620, 600);
            graphPictureBox.BorderStyle = BorderStyle.None;
            graphPictureBox.BackColor = Color.White;
            graphPictureBox.Paint += (s, e) =>
            {
                if (graphPictureBox.Image == null)
                {
                    e.Graphics.DrawString("График появится после расчета",
                        new Font("Segoe UI", 11, FontStyle.Italic),
                        Brushes.Gray, graphPictureBox.Width / 2 - 120, graphPictureBox.Height / 2);
                }
            };

            // Кнопки действий
            Button btnCalculate = CreateButton("Рассчитать", primaryColor, new Point(20, 610), new Size(150, 40));
            btnCalculate.Click += CalculateMenuItem_Click;

            Button btnClear = CreateButton("Очистить", dangerColor, new Point(190, 610), new Size(150, 40));
            btnClear.Click += ClearMenuItem_Click;

            // Добавление элементов в контейнеры
            leftContainer.Controls.Add(inputPanel);
            leftContainer.Controls.Add(resultPanel);
            leftContainer.Controls.Add(btnCalculate);
            leftContainer.Controls.Add(btnClear);

            this.Controls.Add(leftContainer);
            this.Controls.Add(graphPictureBox);

            // Информационная панель внизу
            Panel infoPanel = new Panel();
            infoPanel.Location = new Point(0, 660);
            infoPanel.Size = new Size(1000, 40);
            infoPanel.BackColor = darkColor;

            Label infoLabel = new Label();
            infoLabel.Text = "Метод золотого сечения для поиска экстремумов функции";
            infoLabel.ForeColor = Color.White;
            infoLabel.Font = new Font("Segoe UI", 9);
            infoLabel.Location = new Point(20, 10);
            infoLabel.Size = new Size(500, 20);
            infoPanel.Controls.Add(infoLabel);

            Label goldenRatioLabel = new Label();
            goldenRatioLabel.Text = $"φ = {GOLDEN_RATIO:F6}";
            goldenRatioLabel.ForeColor = accentColor;
            goldenRatioLabel.Font = new Font("Segoe UI", 9, FontStyle.Bold);
            goldenRatioLabel.Location = new Point(850, 10);
            goldenRatioLabel.Size = new Size(130, 20);
            infoPanel.Controls.Add(goldenRatioLabel);

            this.Controls.Add(infoPanel);
        }

        private Button CreateButton(string text, Color color, Point location, Size size)
        {
            Button button = new Button();
            button.Text = text;
            button.Location = location;
            button.Size = size;
            button.BackColor = color;
            button.ForeColor = Color.White;
            button.Font = new Font("Segoe UI", 10, FontStyle.Bold);
            button.FlatStyle = FlatStyle.Flat;
            button.FlatAppearance.BorderSize = 0;
            button.Cursor = Cursors.Hand;
            button.MouseEnter += (s, e) => button.BackColor = ControlPaint.Light(color);
            button.MouseLeave += (s, e) => button.BackColor = color;

            return button;
        }

        private void ApplyColorScheme()
        {
            // Применение цветов к элементам
            foreach (Control control in this.Controls)
            {
                if (control is Panel panel)
                {
                    ApplyPanelColors(panel);
                }
                else if (control is Label label)
                {
                    if (label.Parent is Panel)
                        label.ForeColor = darkColor;
                }
            }
        }

        private void ApplyPanelColors(Panel panel)
        {
            foreach (Control control in panel.Controls)
            {
                if (control is Label label)
                {
                    label.ForeColor = darkColor;
                }
                else if (control is TextBox textBox)
                {
                    textBox.BackColor = Color.White;
                    textBox.ForeColor = darkColor;
                }
                else if (control is GroupBox groupBox)
                {
                    foreach (Control subControl in groupBox.Controls)
                    {
                        if (subControl is RadioButton radio)
                        {
                            radio.ForeColor = darkColor;
                        }
                    }
                }
            }
        }

        private double GoldenRatioMin(Func<double, double> f, double a, double b, double epsilon)
        {
            double x1 = b - GOLDEN_RATIO * (b - a);
            double x2 = a + GOLDEN_RATIO * (b - a);

            double f1 = f(x1);
            double f2 = f(x2);

            while (Math.Abs(b - a) > epsilon)
            {
                if (f1 < f2)
                {
                    b = x2;
                    x2 = x1;
                    f2 = f1;
                    x1 = b - GOLDEN_RATIO * (b - a);
                    f1 = f(x1);
                }
                else
                {
                    a = x1;
                    x1 = x2;
                    f1 = f2;
                    x2 = a + GOLDEN_RATIO * (b - a);
                    f2 = f(x2);
                }
            }

            return (a + b) / 2;
        }

        private double GoldenRatioMax(Func<double, double> f, double a, double b, double epsilon)
        {
            double x1 = b - GOLDEN_RATIO * (b - a);
            double x2 = a + GOLDEN_RATIO * (b - a);

            double f1 = f(x1);
            double f2 = f(x2);

            while (Math.Abs(b - a) > epsilon)
            {
                if (f1 > f2)
                {
                    b = x2;
                    x2 = x1;
                    f2 = f1;
                    x1 = b - GOLDEN_RATIO * (b - a);
                    f1 = f(x1);
                }
                else
                {
                    a = x1;
                    x1 = x2;
                    f1 = f2;
                    x2 = a + GOLDEN_RATIO * (b - a);
                    f2 = f(x2);
                }
            }

            return (a + b) / 2;
        }

        private string ConvertPowerToMultiplication(string expression)
        {
            string result = expression;

            string pattern = @"([a-zA-Z0-9\.\(\)]+)\^(\d+)";

            MatchCollection matches = Regex.Matches(result, pattern);

            List<Match> matchList = new List<Match>();
            foreach (Match match in matches)
            {
                matchList.Add(match);
            }

            for (int i = matchList.Count - 1; i >= 0; i--)
            {
                Match match = matchList[i];
                string baseExpr = match.Groups[1].Value;
                int exponent = int.Parse(match.Groups[2].Value);

                string multiplication = "";
                for (int j = 0; j < exponent; j++)
                {
                    if (j > 0) multiplication += "*";
                    multiplication += baseExpr;
                }

                if (exponent == 1)
                {
                    multiplication = baseExpr;
                }
                else if (exponent == 0)
                {
                    multiplication = "1";
                }

                result = result.Remove(match.Index, match.Length);
                result = result.Insert(match.Index, multiplication);
            }

            return result;
        }

        private string PrepareExpression(string expression, double xValue)
        {
            string result = expression;

            result = result.Replace(" ", "");

            result = ConvertPowerToMultiplication(result);

            result = result.Replace("x", xValue.ToString(System.Globalization.CultureInfo.InvariantCulture));

            result = Regex.Replace(result, @"(\d),(\d)", "$1.$2");

            return result;
        }

        private Func<double, double> ParseFunction(string functionString)
        {
            return x =>
            {
                try
                {
                    string expression = PrepareExpression(functionString, x);

                    object result = new DataTable().Compute(expression, null);

                    if (result is double)
                        return (double)result;
                    else if (result is decimal)
                        return (double)(decimal)result;
                    else if (result is int)
                        return (double)(int)result;
                    else
                        return Convert.ToDouble(result);
                }
                catch
                {
                    throw new ArgumentException("Некорректное выражение функции");
                }
            };
        }

        private void DrawGraph(Func<double, double> f, double a, double b, double extremumX, bool isMinimum)
        {
            Bitmap bitmap = new Bitmap(graphPictureBox.Width, graphPictureBox.Height);
            using (Graphics g = Graphics.FromImage(bitmap))
            {
                g.Clear(Color.White);
                g.SmoothingMode = System.Drawing.Drawing2D.SmoothingMode.AntiAlias;

                // Градиентный фон
                using (LinearGradientBrush brush = new LinearGradientBrush(
                    new Point(0, 0),
                    new Point(0, graphPictureBox.Height),
                    Color.FromArgb(255, 245, 235),
                    Color.FromArgb(255, 240, 225)))
                {
                    g.FillRectangle(brush, 0, 0, graphPictureBox.Width, graphPictureBox.Height);
                }

                double step = (b - a) / 200;
                double minY = double.MaxValue;
                double maxY = double.MinValue;

                List<double> xValues = new List<double>();
                List<double> yValues = new List<double>();

                for (double x = a; x <= b; x += step)
                {
                    try
                    {
                        double y = f(x);
                        xValues.Add(x);
                        yValues.Add(y);

                        if (!double.IsNaN(y) && !double.IsInfinity(y))
                        {
                            if (y < minY) minY = y;
                            if (y > maxY) maxY = y;
                        }
                    }
                    catch
                    {
                    }
                }

                if (double.IsInfinity(minY) || double.IsNaN(minY) ||
                    double.IsInfinity(maxY) || double.IsNaN(maxY) ||
                    Math.Abs(maxY - minY) < 1e-10)
                {
                    minY = -10;
                    maxY = 10;
                }

                double yRange = maxY - minY;
                minY -= yRange * 0.1;
                maxY += yRange * 0.1;
                yRange = maxY - minY;

                float scaleX = graphPictureBox.Width / (float)(b - a);
                float scaleY = graphPictureBox.Height / (float)yRange;

                // Оси координат
                Pen axisPen = new Pen(Color.FromArgb(180, 150, 130), 2);
                Pen gridPen = new Pen(Color.FromArgb(240, 230, 220), 1);

                float yZero = graphPictureBox.Height - (float)(-minY) * scaleY;

                // Сетка по X
                for (double x = a; x <= b; x += (b - a) / 10)
                {
                    float screenX = (float)(x - a) * scaleX;
                    g.DrawLine(gridPen, screenX, 0, screenX, graphPictureBox.Height);
                }

                // Сетка по Y
                for (double y = minY; y <= maxY; y += yRange / 10)
                {
                    float screenY = graphPictureBox.Height - (float)(y - minY) * scaleY;
                    g.DrawLine(gridPen, 0, screenY, graphPictureBox.Width, screenY);
                }

                // Ось X
                if (yZero > 0 && yZero < graphPictureBox.Height)
                {
                    g.DrawLine(new Pen(darkColor, 2), 0, yZero, graphPictureBox.Width, yZero);

                    for (double x = a; x <= b; x += (b - a) / 10)
                    {
                        float screenX = (float)(x - a) * scaleX;
                        g.DrawLine(axisPen, screenX, yZero - 4, screenX, yZero + 4);
                        g.DrawString(x.ToString("F1"), new Font("Segoe UI", 8),
                            Brushes.Brown, screenX - 10, yZero + 8);
                    }
                }

                // Ось Y
                float xZero = (float)(-a) * scaleX;
                if (xZero > 0 && xZero < graphPictureBox.Width)
                {
                    g.DrawLine(new Pen(darkColor, 2), xZero, 0, xZero, graphPictureBox.Height);

                    for (double y = minY; y <= maxY; y += yRange / 10)
                    {
                        float screenY = graphPictureBox.Height - (float)(y - minY) * scaleY;
                        g.DrawLine(axisPen, xZero - 4, screenY, xZero + 4, screenY);
                        g.DrawString(y.ToString("F1"), new Font("Segoe UI", 8),
                            Brushes.Brown, xZero + 8, screenY - 10);
                    }
                }

                // График функции
                Pen graphPen = new Pen(primaryColor, 3);
                PointF? prevPoint = null;

                for (int i = 0; i < xValues.Count; i++)
                {
                    try
                    {
                        double x = xValues[i];
                        double y = yValues[i];

                        if (double.IsNaN(y) || double.IsInfinity(y))
                        {
                            prevPoint = null;
                            continue;
                        }

                        float screenX = (float)(x - a) * scaleX;
                        float screenY = graphPictureBox.Height - (float)(y - minY) * scaleY;

                        PointF currentPoint = new PointF(screenX, screenY);

                        if (prevPoint.HasValue)
                        {
                            g.DrawLine(graphPen, prevPoint.Value, currentPoint);
                        }

                        prevPoint = currentPoint;
                    }
                    catch
                    {
                        prevPoint = null;
                    }
                }

                // Экстремум
                if (extremumX >= a && extremumX <= b)
                {
                    try
                    {
                        double extremumY = f(extremumX);
                        float screenExtremumX = (float)(extremumX - a) * scaleX;
                        float screenExtremumY = graphPictureBox.Height - (float)(extremumY - minY) * scaleY;

                        Color pointColor = isMinimum ? dangerColor : accentColor;
                        Brush pointBrush = new SolidBrush(pointColor);

                        // Тень точки
                        g.FillEllipse(Brushes.Brown, screenExtremumX - 6, screenExtremumY - 6, 12, 12);

                        // Основная точка
                        g.FillEllipse(pointBrush, screenExtremumX - 5, screenExtremumY - 5, 10, 10);
                        g.DrawEllipse(new Pen(Color.White, 2), screenExtremumX - 5, screenExtremumY - 5, 10, 10);

                        // Подпись
                        string label = isMinimum
                            ? $"Минимум: ({extremumX:F3}, {extremumY:F3})"
                            : $"Максимум: ({extremumX:F3}, {extremumY:F3})";

                        using (Font labelFont = new Font("Segoe UI", 9, FontStyle.Bold))
                        {
                            SizeF textSize = g.MeasureString(label, labelFont);
                            RectangleF textRect = new RectangleF(
                                screenExtremumX + 15,
                                screenExtremumY - textSize.Height / 2,
                                textSize.Width + 10,
                                textSize.Height + 4);

                            g.FillRectangle(new SolidBrush(Color.FromArgb(220, Color.White)), textRect);
                            g.DrawRectangle(new Pen(Color.LightGray, 1),
                                textRect.X, textRect.Y, textRect.Width, textRect.Height);
                            g.DrawString(label, labelFont,
                                new SolidBrush(pointColor), screenExtremumX + 20, screenExtremumY - textSize.Height / 2);
                        }
                    }
                    catch { }
                }

                // Информационная панель на графике
                using (Font titleFont = new Font("Segoe UI", 12, FontStyle.Bold))
                using (Font infoFont = new Font("Segoe UI", 9))
                {
                    string modeText = isMinimum ? "Поиск минимума" : "Поиск максимума";
                    Color modeColor = isMinimum ? dangerColor : accentColor;

                    g.DrawString($"f(x) = {txtFunction.Text}", titleFont, Brushes.Brown, 20, 20);
                    g.DrawString(modeText, infoFont, new SolidBrush(modeColor), 20, 50);
                    g.DrawString($"Интервал: [{a:F2}, {b:F2}]", infoFont, Brushes.SaddleBrown, 20, 75);
                    g.DrawString($"Точность: ε = {txtEpsilon.Text}", infoFont, Brushes.SaddleBrown, 20, 95);
                    g.DrawString($"Метод: Золотое сечение", infoFont, Brushes.SaddleBrown, 20, 115);
                }
            }

            graphPictureBox.Image = bitmap;
        }

        private void CalculateMenuItem_Click(object sender, EventArgs e)
        {
            try
            {
                if (string.IsNullOrWhiteSpace(txtFunction.Text))
                {
                    MessageBox.Show("Введите функцию f(x)", "Ошибка",
                        MessageBoxButtons.OK, MessageBoxIcon.Error);
                    return;
                }

                if (!double.TryParse(txtA.Text.Replace(",", "."),
                    System.Globalization.NumberStyles.Any,
                    System.Globalization.CultureInfo.InvariantCulture, out double a))
                {
                    MessageBox.Show("Некорректное значение a", "Ошибка",
                        MessageBoxButtons.OK, MessageBoxIcon.Error);
                    return;
                }

                if (!double.TryParse(txtB.Text.Replace(",", "."),
                    System.Globalization.NumberStyles.Any,
                    System.Globalization.CultureInfo.InvariantCulture, out double b))
                {
                    MessageBox.Show("Некорректное значение b", "Ошибка",
                        MessageBoxButtons.OK, MessageBoxIcon.Error);
                    return;
                }

                if (!double.TryParse(txtEpsilon.Text.Replace(",", "."),
                    System.Globalization.NumberStyles.Any,
                    System.Globalization.CultureInfo.InvariantCulture, out double epsilon))
                {
                    MessageBox.Show("Некорректное значение точности ε", "Ошибка",
                        MessageBoxButtons.OK, MessageBoxIcon.Error);
                    return;
                }

                if (epsilon <= 0)
                {
                    MessageBox.Show("Точность должна быть положительным числом", "Ошибка",
                        MessageBoxButtons.OK, MessageBoxIcon.Error);
                    return;
                }

                if (a >= b)
                {
                    MessageBox.Show("a должно быть меньше b", "Ошибка",
                        MessageBoxButtons.OK, MessageBoxIcon.Error);
                    return;
                }

                bool findMinimum = radioMin.Checked;

                Func<double, double> function = ParseFunction(txtFunction.Text);

                try
                {
                    double test1 = function(a);
                    double test2 = function(b);
                    double test3 = function((a + b) / 2);

                    if (double.IsNaN(test1) || double.IsInfinity(test1) ||
                        double.IsNaN(test2) || double.IsInfinity(test2) ||
                        double.IsNaN(test3) || double.IsInfinity(test3))
                    {
                        throw new Exception("Функция возвращает нечисловое значение");
                    }
                }
                catch (Exception ex)
                {
                    MessageBox.Show($"Ошибка в функции: {ex.Message}", "Ошибка",
                        MessageBoxButtons.OK, MessageBoxIcon.Error);
                    return;
                }

                double extremumX;
                double extremumY;

                if (findMinimum)
                {
                    extremumX = GoldenRatioMin(function, a, b, epsilon);
                }
                else
                {
                    extremumX = GoldenRatioMax(function, a, b, epsilon);
                }

                extremumY = function(extremumX);

                string extremumType = findMinimum ? "минимума" : "максимума";
                txtResult.Text = $"Найденный {extremumType}:" + Environment.NewLine +
                               $"x = {extremumX:F6}" + Environment.NewLine +
                               $"f(x) = {extremumY:F6}" + Environment.NewLine +
                               $"Точность: ε = {epsilon}" + Environment.NewLine +
                               $"Интервал: [{a}, {b}]" + Environment.NewLine +
                               $"Метод: Золотое сечение (φ = {GOLDEN_RATIO:F4})";

                DrawGraph(function, a, b, extremumX, findMinimum);
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Ошибка: {ex.Message}", "Ошибка",
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void ClearMenuItem_Click(object sender, EventArgs e)
        {
            txtFunction.Text = "x^2 - 4*x + 4";
            radioMin.Checked = true;
            txtA.Text = "0";
            txtB.Text = "5";
            txtEpsilon.Text = "0.001";
            txtResult.Clear();

            if (graphPictureBox.Image != null)
            {
                graphPictureBox.Image.Dispose();
                graphPictureBox.Image = null;
            }

            graphPictureBox.Invalidate();
        }

        private void AboutMenuItem_Click(object sender, EventArgs e)
        {
            string message = "Метод золотого сечения для поиска экстремумов функции\n\n" +
                           "Поддерживаемые операции:\n" +
                           "• +, -, *, / - арифметические операции\n" +
                           "• ^ - возведение в степень (только целые степени)\n" +
                           "• ( ) - скобки для группировки\n\n" +
                           "Примеры функций:\n" +
                           "• x^2 - 4*x + 4\n" +
                           "• sin(x) + cos(x)\n" +
                           "• x^3 - 6*x^2 + 9*x + 2\n\n" +
                           "Золотое сечение: φ = (√5 - 1)/2 ≈ 0.618";

            MessageBox.Show(message, "Справка",
                MessageBoxButtons.OK, MessageBoxIcon.Information);
        }
    }
}