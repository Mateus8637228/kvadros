using System;
using System.Drawing;
using System.Text.RegularExpressions;
using System.Windows.Forms;

namespace Kvadros
{
  public partial class Form1 : Form
  {
    private TextBox? txtEquation;
    private Label? lblResult;
    private Button? btnCalc, btnClear;

    public Form1()
    {
      this.Text = "Решение квадратных уравнений";
      this.Size = new Size(800, 700);
      this.StartPosition = FormStartPosition.CenterScreen;
      this.BackColor = Color.WhiteSmoke;

      InitializeCustomComponents();
    }

    private void InitializeCustomComponents()
    {
      Panel topPanel = new Panel();
      topPanel.Dock = DockStyle.Top;
      topPanel.Height = 160;
      topPanel.BackColor = Color.White;
      topPanel.Padding = new Padding(20);
      this.Controls.Add(topPanel);

      Label lblTitle = new Label();
      lblTitle.Text = "Введите квадратное уравнение (например: 2x^2 - 5x + 3 = 0):";
      lblTitle.Font = new Font("Arial", 12, FontStyle.Bold);
      lblTitle.Location = new Point(20, 20);
      lblTitle.AutoSize = true;
      topPanel.Controls.Add(lblTitle);

      txtEquation = new TextBox();
      txtEquation.Location = new Point(20, 50);
      txtEquation.Width = 500;
      txtEquation.Font = new Font("Arial", 16);
      txtEquation.Text = "x^2 - 5x + 6 = 0";
      topPanel.Controls.Add(txtEquation);

      btnCalc = new Button();
      btnCalc.Text = "Решить";
      btnCalc.Location = new Point(540, 48);
      btnCalc.Size = new Size(200, 35);
      btnCalc.Font = new Font("Arial", 12, FontStyle.Bold);
      btnCalc.BackColor = Color.LightBlue;
      btnCalc.FlatStyle = FlatStyle.Flat;
      btnCalc.Click += BtnCalc_Click;
      topPanel.Controls.Add(btnCalc);

      btnClear = new Button();
      btnClear.Text = "Очистить";
      btnClear.Location = new Point(20, 100);
      btnClear.Size = new Size(150, 30);
      btnClear.Font = new Font("Arial", 10);
      btnClear.Click += BtnClear_Click;
      topPanel.Controls.Add(btnClear);

      lblResult = new Label();
      lblResult.Dock = DockStyle.Fill;
      lblResult.Font = new Font("Consolas", 14);
      lblResult.BackColor = Color.AliceBlue;
      lblResult.BorderStyle = BorderStyle.FixedSingle;
      lblResult.Padding = new Padding(30);
      lblResult.Text = "Здесь будет подробное решение...";
      lblResult.TextAlign = ContentAlignment.TopLeft;
      this.Controls.Add(lblResult);
      lblResult.BringToFront();
    }

    private bool ParseEquation(string input, out double a, out double b, out double c)
    {
      a = 0; b = 0; c = 0;
      string s = input.ToLower().Replace(" ", "");

      if (s.Contains("=")) s = s.Split('=')[0];

      s = s.Replace("x*x", "x^2").Replace("x2", "x^2");

      try
      {
        Match matchA = Regex.Match(s, @"([+-]?\d*\.?\d*)x\^2");
        if (matchA.Success)
        {
          string val = matchA.Groups[1].Value;
          if (val == "" || val == "+") a = 1;
          else if (val == "-") a = -1;
          else a = double.Parse(val.Replace('.', ','));
          s = s.Replace(matchA.Value, "");
        }
        else if (s.Contains("x^2")) { a = 1; s = s.Replace("x^2", ""); }

        Match matchB = Regex.Match(s, @"([+-]?\d*\.?\d*)x");
        if (matchB.Success)
        {
          string val = matchB.Groups[1].Value;
          if (val == "" || val == "+") b = 1;
          else if (val == "-") b = -1;
          else b = double.Parse(val.Replace('.', ','));
          s = s.Replace(matchB.Value, "");
        }

        if (!string.IsNullOrEmpty(s))
        {
          s = s.Trim('+', '-');
          if (double.TryParse(s.Replace('.', ','), out double parsedC)) c = parsedC;
        }

        return true;
      }
      catch { return false; }
    }

    private void BtnCalc_Click(object? sender, EventArgs e)
    {
      if (txtEquation == null || lblResult == null) return;

      string input = txtEquation.Text.Trim();
      if (string.IsNullOrEmpty(input)) return;

      if (!ParseEquation(input, out double a, out double b, out double c))
      {
        MessageBox.Show("Не удалось разобрать уравнение. Попробуйте формат: 2x^2 - 5x + 3 = 0", "Ошибка", MessageBoxButtons.OK, MessageBoxIcon.Error);
        return;
      }

      if (a == 0)
      {
        MessageBox.Show("Коэффициент 'a' не может быть равен нулю.", "Ошибка", MessageBoxButtons.OK, MessageBoxIcon.Warning);
        return;
      }

      string result = $"РАЗБОР УРАВНЕНИЯ\n";
      result += $"Вы ввели: {input}\n";
      result += $"Коэффициенты: a = {a}, b = {b}, c = {c}\n";
      result += new string('=', 60) + "\n\n";

      result += "1. МЕТОД ДИСКРИМИНАНТА:\n";
      double d = b * b - 4 * a * c;
      result += $"   D = b² - 4ac = ({b})² - 4 * {a} * {c} = {d}\n";

      double? x1 = null, x2 = null;

      if (d > 0)
      {
        x1 = (-b + Math.Sqrt(d)) / (2 * a);
        x2 = (-b - Math.Sqrt(d)) / (2 * a);
        result += "   D > 0, уравнение имеет два различных корня:\n";
        result += $"   x1 = (-b + √D) / 2a = {Math.Round(x1.Value, 4)}\n";
        result += $"   x2 = (-b - √D) / 2a = {Math.Round(x2.Value, 4)}\n";
      }
      else if (Math.Abs(d) < 1e-9)
      {
        x1 = -b / (2 * a);
        result += "   D = 0, уравнение имеет один корень (кратный):\n";
        result += $"   x = -b / 2a = {Math.Round(x1.Value, 4)}\n";
      }
      else
      {
        result += "   D < 0, действительных корней нет.\n";
      }

      result += "\n" + new string('-', 60) + "\n\n";

      result += "2. ТЕОРЕМА ВИЕТА:\n";
      double sum = -b / a;
      double product = c / a;
      result += $"   Сумма корней (x1 + x2) = -b/a = {Math.Round(sum, 4)}\n";
      result += $"   Произведение (x1 * x2) = c/a = {Math.Round(product, 4)}\n\n";

      if (x1.HasValue && x2.HasValue)
      {
        double checkSum = x1.Value + x2.Value;
        double checkProd = x1.Value * x2.Value;
        result += "   ПРОВЕРКА найденных корней:\n";
        result += $"   x1 + x2 = {Math.Round(checkSum, 4)} (ожидалось {Math.Round(sum, 4)})\n";
        result += $"   x1 * x2 = {Math.Round(checkProd, 4)} (ожидалось {Math.Round(product, 4)})\n";

        if (Math.Abs(checkSum - sum) < 1e-4 && Math.Abs(checkProd - product) < 1e-4)
          result += "\n   ✅ Теорема Виета выполняется! Корни найдены верно.\n";
        else
          result += "\n   ❌ Ошибка в вычислениях.\n";
      }

      lblResult.Text = result;
    }

    private void BtnClear_Click(object? sender, EventArgs e)
    {
      if (txtEquation == null || lblResult == null) return;
      txtEquation.Text = "x^2 - 5x + 6 = 0";
      lblResult.Text = "Здесь будет подробное решение...";
    }
  }
}