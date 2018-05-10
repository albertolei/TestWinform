using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using TestWinForm.Utils;

namespace TestWinForm
{
    public partial class StirrupForm : Form
    {
        private float PADDING = 40;
        private float STIRRUPMUTIPLE = 10;

        private float longitudinal_rebar_diameter = 40;    //纵筋直径
        private float protective_layer_thinckness = 20;   //保护层厚度
        private float bending_length = 75;               //弯折长度
        private float stirrup_diameter = 10;              //箍筋直径

        private string type = "1";                      //默认类型为2
        private float b = 1000, h = 1000, d = 1000;     //参数宽、高、直径
        private object[] m_values = {3, 4}, n_values = {3, 4};
        private int m = 4, n = 4;                       //默认m为4，n为4
        private float lap_length = 300;                  //圆形箍筋搭接长度

        private PointF center_point = new PointF();      //画布中心点
        private Bitmap bmp = null;                      //二维像素画布
        float zoom, zoomB, zoomH;                       //绘图比例

        private PointF start_point, end_point, left_down, left_up, right_up, right_down;            //箍筋围绕的四根纵筋的圆心;
        private Pen concrete_pen, rebar_pen;       //混凝土线, 钢筋线
        
        public StirrupForm()
        {
            InitializeComponent();
        }
        private void StirrupForm_Load(object sender, EventArgs e)
        {
            init_parameters1_controls();
            redraw1();
        }

        private void init_parameters1_controls()    //第1种箍筋参数
        {
            parambox.Text = "配筋参数1";

            parambox.Controls.Clear();

            Label label1, label2, label3, label4;
            label1 = ControlUtil.create_label("b：", new Size(23, 12), new Point(10, 25));
            label2 = ControlUtil.create_label("h：", new Size(23, 12), new Point(143, 25));
            label3 = ControlUtil.create_label("m：", new Size(23, 12), new Point(10, 55));
            label4 = ControlUtil.create_label("n：", new Size(23, 12), new Point(143, 55));
            TextBox textbox_b, textbox_h;
            textbox_b = ControlUtil.create_textbox(Convert.ToString(b), new Size(100, 21), new Point(33, 20), new KeyPressEventHandler(textboxb_KeyPress), new EventHandler(textboxb_TextChanged));
            textbox_h = ControlUtil.create_textbox(Convert.ToString(h), new Size(100, 21), new Point(166, 20), new KeyPressEventHandler(textboxh_KeyPress), new EventHandler(textboxh_TextChanged));
            ComboBox combobox_m, combobox_n;
            combobox_m = ControlUtil.create_combobox(m_values, m, new Size(100, 20), new Point(33, 51), new EventHandler(comboboxm_SelectedIndexChanged));
            combobox_n = ControlUtil.create_combobox(n_values, n, new Size(100, 20), new Point(166, 51), new EventHandler(comboboxn_SelectedIndexChanged));

            parambox.Controls.Add(label1);
            parambox.Controls.Add(textbox_b);
            parambox.Controls.Add(label2);
            parambox.Controls.Add(textbox_h);
            parambox.Controls.Add(label3);
            parambox.Controls.Add(combobox_m);
            parambox.Controls.Add(label4);
            parambox.Controls.Add(combobox_n);
        }
        private void init_parameters2_controls()    //第2种箍筋参数
        {
            parambox.Text = "配筋参数2";

            parambox.Controls.Clear();

            Label label1, label2;
            label1 = ControlUtil.create_label("b：", new Size(23, 12), new Point(10, 25));
            label2 = ControlUtil.create_label("h：", new Size(23, 12), new Point(143, 25));
            TextBox textbox_b, textbox_h;
            textbox_b = ControlUtil.create_textbox(Convert.ToString(b), new Size(100, 21), new Point(33, 20), new KeyPressEventHandler(textboxb_KeyPress), new EventHandler(textboxb_TextChanged));
            textbox_h = ControlUtil.create_textbox(Convert.ToString(h), new Size(100, 21), new Point(166, 20), new KeyPressEventHandler(textboxh_KeyPress), new EventHandler(textboxh_TextChanged));

            parambox.Controls.Add(label1);
            parambox.Controls.Add(textbox_b);
            parambox.Controls.Add(label2);
            parambox.Controls.Add(textbox_h);
        }
        private bool init_parameters6_controls()    //第6种箍筋参数
        {
            parambox.Text = "配筋参数6";

            parambox.Controls.Clear();

            Label label5 = ControlUtil.create_label("D：", new Size(23, 12), new Point(10, 25));
            TextBox textbox_d = ControlUtil.create_textbox(Convert.ToString(d), new Size(100, 21), new Point(33, 20), new KeyPressEventHandler(textboxd_KeyPress), new EventHandler(textboxd_TextChanged));

            parambox.Controls.Add(label5);
            parambox.Controls.Add(textbox_d);

            return check_textbox_d(textbox_d);
        }
        private bool init_parameters7_controls()    //第7种箍筋参数
        {
            parambox.Text = "配筋参数7";

            parambox.Controls.Clear();

            Label label5 = ControlUtil.create_label("D：", new Size(23, 12), new Point(10, 25));
            TextBox textbox_d = ControlUtil.create_textbox(Convert.ToString(d), new Size(100, 21), new Point(33, 20), new KeyPressEventHandler(textboxd_KeyPress), new EventHandler(textboxd_TextChanged));

            parambox.Controls.Add(label5);
            parambox.Controls.Add(textbox_d);

            return check_textbox_d(textbox_d);
        }
        private void redraw1()      //画第1种箍筋
        {
            try
            {
                //放大缩小比例
                zoomB = (picturebox.Width - PADDING - PADDING) / b;
                zoomH = (picturebox.Height - PADDING - PADDING) / h;
                zoom = Math.Min(zoomB, zoomH);

                bmp = new Bitmap(picturebox.Width, picturebox.Height);
                Graphics graphics = Graphics.FromImage(bmp);

                center_point.X = picturebox.Width / 2;  //中心点X向坐标
                center_point.Y = picturebox.Height / 2; //中心点Y向坐标

                concrete_pen = new Pen(Color.FromArgb(155, 112, 199), 3);       //混凝土线
                rebar_pen = new Pen(Color.Red, 2);                              //钢筋线
                //混凝土边框
                PointF[] points = new PointF[5];
                points[0] = new PointF(center_point.X + b / 2 * zoom, center_point.Y - h / 2 * zoom);
                points[1] = new PointF(center_point.X + b / 2 * zoom, center_point.Y + h / 2 * zoom);
                points[2] = new PointF(center_point.X - b / 2 * zoom, center_point.Y + h / 2 * zoom);
                points[3] = new PointF(center_point.X - b / 2 * zoom, center_point.Y - h / 2 * zoom);
                points[4] = new PointF(center_point.X + b / 2 * zoom, center_point.Y - h / 2 * zoom);
                graphics.DrawLines(concrete_pen, points);

                float rebarB = b - protective_layer_thinckness * 2 - longitudinal_rebar_diameter;    //X向直钢筋的长度
                float rebarH = h - protective_layer_thinckness * 2 - longitudinal_rebar_diameter;    //Y向直钢筋的长度

                //右上角第一段弯折(包括弧和弯折长度)
                right_up = new PointF(center_point.X + rebarB / 2 * zoom, center_point.Y - rebarH / 2 * zoom);  //右上角纵筋的圆心
                bending_length = stirrup_diameter * STIRRUPMUTIPLE > 75 ? stirrup_diameter * STIRRUPMUTIPLE : 75;   //箍筋平直段长度  

                graphics.FillEllipse(Brushes.Red, right_up.X - longitudinal_rebar_diameter / 2 * zoom, right_up.Y - longitudinal_rebar_diameter / 2 * zoom, longitudinal_rebar_diameter * zoom, longitudinal_rebar_diameter * zoom);

                end_point = new PointF(right_up.X - longitudinal_rebar_diameter / 2 / (float)Math.Sqrt(2.0) * zoom, right_up.Y - longitudinal_rebar_diameter / 2 / (float)Math.Sqrt(2.0) * zoom);
                start_point = new PointF(right_up.X - longitudinal_rebar_diameter / 2 / (float)Math.Sqrt(2.0) * zoom - bending_length / (float)Math.Sqrt(2.0) * zoom, right_up.Y - longitudinal_rebar_diameter / 2 / (float)Math.Sqrt(2.0) * zoom + bending_length / (float)Math.Sqrt(2.0) * zoom);
                graphics.DrawLine(rebar_pen, start_point, end_point);
                graphics.DrawArc(rebar_pen, right_up.X - longitudinal_rebar_diameter / 2 * zoom, right_up.Y - longitudinal_rebar_diameter / 2 * zoom, longitudinal_rebar_diameter * zoom, longitudinal_rebar_diameter * zoom, 225, 135);

                //右侧线
                PointF[] line1 = new PointF[2];
                line1[0] = new PointF(center_point.X + rebarB / 2 * zoom + longitudinal_rebar_diameter / 2 * zoom, center_point.Y - rebarH / 2 * zoom);
                line1[1] = new PointF(center_point.X + rebarB / 2 * zoom + longitudinal_rebar_diameter / 2 * zoom, center_point.Y + rebarH / 2 * zoom);
                graphics.DrawLines(rebar_pen, line1);

                //右下角的弧
                graphics.DrawArc(rebar_pen, center_point.X + rebarB / 2 * zoom - longitudinal_rebar_diameter / 2 * zoom, center_point.Y + rebarH / 2 * zoom - longitudinal_rebar_diameter / 2 * zoom, longitudinal_rebar_diameter * zoom, longitudinal_rebar_diameter * zoom, 0, 90);
                graphics.FillEllipse(Brushes.Red, center_point.X + rebarB / 2 * zoom - longitudinal_rebar_diameter / 2 * zoom, center_point.Y + rebarH / 2 * zoom - longitudinal_rebar_diameter / 2 * zoom, longitudinal_rebar_diameter * zoom, longitudinal_rebar_diameter * zoom);

                //下方线
                PointF[] line2 = new PointF[2];
                line2[0] = new PointF(center_point.X + rebarB / 2 * zoom, center_point.Y + rebarH / 2 * zoom + longitudinal_rebar_diameter / 2 * zoom);
                line2[1] = new PointF(center_point.X - rebarB / 2 * zoom, center_point.Y + rebarH / 2 * zoom + longitudinal_rebar_diameter / 2 * zoom);
                graphics.DrawLines(rebar_pen, line2);

                //左下角的弧
                graphics.DrawArc(rebar_pen, center_point.X - rebarB / 2 * zoom - longitudinal_rebar_diameter / 2 * zoom, center_point.Y + rebarH / 2 * zoom - longitudinal_rebar_diameter / 2 * zoom, longitudinal_rebar_diameter * zoom, longitudinal_rebar_diameter * zoom, 90, 90);
                graphics.FillEllipse(Brushes.Red, center_point.X - rebarB / 2 * zoom - longitudinal_rebar_diameter / 2 * zoom, center_point.Y + rebarH / 2 * zoom - longitudinal_rebar_diameter / 2 * zoom, longitudinal_rebar_diameter * zoom, longitudinal_rebar_diameter * zoom);

                //左侧线
                PointF[] line3 = new PointF[2];
                line3[0] = new PointF(center_point.X - rebarB / 2 * zoom - longitudinal_rebar_diameter / 2 * zoom, center_point.Y + rebarH / 2 * zoom);
                line3[1] = new PointF(center_point.X - rebarB / 2 * zoom - longitudinal_rebar_diameter / 2 * zoom, center_point.Y - rebarH / 2 * zoom);
                graphics.DrawLines(rebar_pen, line3);

                //左上角的弧
                graphics.DrawArc(rebar_pen, center_point.X - rebarB / 2 * zoom - longitudinal_rebar_diameter / 2 * zoom, center_point.Y - rebarH / 2 * zoom - longitudinal_rebar_diameter / 2 * zoom, longitudinal_rebar_diameter * zoom, longitudinal_rebar_diameter * zoom, 180, 90);
                graphics.FillEllipse(Brushes.Red, center_point.X - rebarB / 2 * zoom - longitudinal_rebar_diameter / 2 * zoom, center_point.Y - rebarH / 2 * zoom - longitudinal_rebar_diameter / 2 * zoom, longitudinal_rebar_diameter * zoom, longitudinal_rebar_diameter * zoom);

                //上方线
                PointF[] line4 = new PointF[2];
                line4[0] = new PointF(center_point.X - rebarB / 2 * zoom, center_point.Y - rebarH / 2 * zoom - longitudinal_rebar_diameter / 2 * zoom);
                line4[1] = new PointF(center_point.X + rebarB / 2 * zoom, center_point.Y - rebarH / 2 * zoom - longitudinal_rebar_diameter / 2 * zoom);
                graphics.DrawLines(rebar_pen, line4);

                //右上角第二段弯折(包括弧和弯折长度)
                start_point = new PointF(right_up.X + longitudinal_rebar_diameter / 2 / (float)Math.Sqrt(2.0) * zoom, right_up.Y + longitudinal_rebar_diameter / 2 / (float)Math.Sqrt(2.0) * zoom);
                end_point = new PointF(right_up.X + longitudinal_rebar_diameter / 2 / (float)Math.Sqrt(2.0) * zoom - bending_length / (float)Math.Sqrt(2.0) * zoom, right_up.Y + longitudinal_rebar_diameter / 2 / (float)Math.Sqrt(2.0) * zoom + bending_length / (float)Math.Sqrt(2.0) * zoom);
                graphics.DrawLine(rebar_pen, start_point, end_point);
                graphics.DrawArc(rebar_pen, right_up.X - longitudinal_rebar_diameter / 2 * zoom, right_up.Y - longitudinal_rebar_diameter / 2 * zoom, longitudinal_rebar_diameter * zoom, longitudinal_rebar_diameter * zoom, 270, 135);
                graphics.FillEllipse(Brushes.Red, right_up.X - longitudinal_rebar_diameter / 2 * zoom, right_up.Y - longitudinal_rebar_diameter / 2 * zoom, longitudinal_rebar_diameter * zoom, longitudinal_rebar_diameter * zoom);

                //X向箍筋
                switch (m)
                {
                    case 3:
                        rebarH = h - protective_layer_thinckness * 2 - longitudinal_rebar_diameter;    //Y向直钢筋的长度

                        //上方弯折部分
                        start_point = new PointF(center_point.X - longitudinal_rebar_diameter / 2 / (float)Math.Sqrt(2.0) * zoom - bending_length / (float)Math.Sqrt(2.0) * zoom, center_point.Y - rebarH / 2 * zoom - longitudinal_rebar_diameter / 2 / (float)Math.Sqrt(2.0) * zoom + bending_length / (float)Math.Sqrt(2.0) * zoom);
                        end_point = new PointF(center_point.X - longitudinal_rebar_diameter / 2 / (float)Math.Sqrt(2.0) * zoom, center_point.Y - rebarH / 2 * zoom - longitudinal_rebar_diameter / 2 / (float)Math.Sqrt(2.0) * zoom);
                        graphics.DrawLine(rebar_pen, start_point, end_point);
                        //上方弧
                        graphics.DrawArc(rebar_pen, center_point.X - longitudinal_rebar_diameter / 2 * zoom, center_point.Y - (rebarH / 2 + longitudinal_rebar_diameter / 2) * zoom, longitudinal_rebar_diameter * zoom, longitudinal_rebar_diameter * zoom, 225, 135);
                        graphics.FillEllipse(Brushes.Red, center_point.X - longitudinal_rebar_diameter / 2 * zoom, center_point.Y - (rebarH / 2 + longitudinal_rebar_diameter / 2) * zoom, longitudinal_rebar_diameter * zoom, longitudinal_rebar_diameter * zoom);
                        //竖向直线
                        start_point = new PointF(center_point.X + longitudinal_rebar_diameter / 2 * zoom, center_point.Y - rebarH / 2 * zoom);
                        end_point = new PointF(center_point.X + longitudinal_rebar_diameter / 2 * zoom, center_point.Y + rebarH / 2 * zoom);
                        graphics.DrawLine(rebar_pen, start_point, end_point);
                        //下方弧
                        graphics.DrawArc(rebar_pen, center_point.X - longitudinal_rebar_diameter / 2 * zoom, center_point.Y + (rebarH / 2 - longitudinal_rebar_diameter / 2) * zoom, longitudinal_rebar_diameter * zoom, longitudinal_rebar_diameter * zoom, 0, 135);
                        graphics.FillEllipse(Brushes.Red, center_point.X - longitudinal_rebar_diameter / 2 * zoom, center_point.Y + (rebarH / 2 - longitudinal_rebar_diameter / 2) * zoom, longitudinal_rebar_diameter * zoom, longitudinal_rebar_diameter * zoom);
                        //下方弯折部分
                        start_point = new PointF(center_point.X - longitudinal_rebar_diameter / 2 / (float)Math.Sqrt(2.0) * zoom, center_point.Y + rebarH / 2 * zoom + longitudinal_rebar_diameter / 2 / (float)Math.Sqrt(2.0) * zoom);
                        end_point = new PointF(center_point.X - longitudinal_rebar_diameter / 2 / (float)Math.Sqrt(2.0) * zoom - bending_length / (float)Math.Sqrt(2.0) * zoom, center_point.Y + rebarH / 2 * zoom + longitudinal_rebar_diameter / 2 / (float)Math.Sqrt(2.0) * zoom - bending_length / (float)Math.Sqrt(2.0) * zoom);
                        graphics.DrawLine(rebar_pen, start_point, end_point);
                        break;
                    case 4:
                        rebarB = (b - protective_layer_thinckness * 2 - longitudinal_rebar_diameter) / 3;    //X向直钢筋的长度
                        rebarH = h - protective_layer_thinckness * 2 - longitudinal_rebar_diameter;          //Y向直钢筋的长度

                        //左上角第一段直段箍筋
                        start_point = new PointF(center_point.X - rebarB / 2 * zoom - longitudinal_rebar_diameter / 2 / (float)Math.Sqrt(2.0) * zoom + bending_length / (float)Math.Sqrt(2.0) * zoom, center_point.Y - rebarH / 2 * zoom + longitudinal_rebar_diameter / 2 / (float)Math.Sqrt(2.0) * zoom + bending_length / (float)Math.Sqrt(2.0) * zoom);
                        end_point = new PointF(center_point.X - rebarB / 2 * zoom - longitudinal_rebar_diameter / 2 / (float)Math.Sqrt(2.0) * zoom, center_point.Y - rebarH / 2 * zoom + longitudinal_rebar_diameter / 2 / (float)Math.Sqrt(2.0) * zoom);
                        graphics.DrawLine(rebar_pen, start_point, end_point);
                        //左上角第一段弧
                        graphics.DrawArc(rebar_pen, center_point.X - rebarB / 2 * zoom - longitudinal_rebar_diameter / 2 * zoom, center_point.Y - rebarH / 2 * zoom - longitudinal_rebar_diameter / 2 * zoom, longitudinal_rebar_diameter * zoom, longitudinal_rebar_diameter * zoom, 135, 135);
                        graphics.FillEllipse(Brushes.Red, center_point.X - rebarB / 2 * zoom - longitudinal_rebar_diameter / 2 * zoom, center_point.Y - rebarH / 2 * zoom - longitudinal_rebar_diameter / 2 * zoom, longitudinal_rebar_diameter * zoom, longitudinal_rebar_diameter * zoom);
                        //上方直线
                        start_point = new PointF(center_point.X - rebarB / 2 * zoom, center_point.Y - rebarH / 2 * zoom - longitudinal_rebar_diameter / 2 * zoom);
                        end_point = new PointF(center_point.X + rebarB / 2 * zoom, center_point.Y - rebarH / 2 * zoom - longitudinal_rebar_diameter / 2 * zoom);
                        graphics.DrawLine(rebar_pen, start_point, end_point);
                        //右上角弧
                        graphics.DrawArc(rebar_pen, center_point.X + rebarB / 2 * zoom - longitudinal_rebar_diameter / 2 * zoom, center_point.Y - rebarH / 2 * zoom - longitudinal_rebar_diameter / 2 * zoom, longitudinal_rebar_diameter * zoom, longitudinal_rebar_diameter * zoom, 270, 90);
                        graphics.FillEllipse(Brushes.Red, center_point.X + rebarB / 2 * zoom - longitudinal_rebar_diameter / 2 * zoom, center_point.Y - rebarH / 2 * zoom - longitudinal_rebar_diameter / 2 * zoom, longitudinal_rebar_diameter * zoom, longitudinal_rebar_diameter * zoom);
                        //右侧直线
                        start_point = new PointF(center_point.X + rebarB / 2 * zoom + longitudinal_rebar_diameter / 2 * zoom, center_point.Y - rebarH / 2 * zoom);
                        end_point = new PointF(center_point.X + rebarB / 2 * zoom + longitudinal_rebar_diameter / 2 * zoom, center_point.Y + rebarH / 2 * zoom);
                        graphics.DrawLine(rebar_pen, start_point, end_point);
                        //右下角弧
                        graphics.DrawArc(rebar_pen, center_point.X + rebarB / 2 * zoom - longitudinal_rebar_diameter / 2 * zoom, center_point.Y + rebarH / 2 * zoom - longitudinal_rebar_diameter / 2 * zoom, longitudinal_rebar_diameter * zoom, longitudinal_rebar_diameter * zoom, 0, 90);
                        graphics.FillEllipse(Brushes.Red, center_point.X + rebarB / 2 * zoom - longitudinal_rebar_diameter / 2 * zoom, center_point.Y + rebarH / 2 * zoom - longitudinal_rebar_diameter / 2 * zoom, longitudinal_rebar_diameter * zoom, longitudinal_rebar_diameter * zoom);
                        //下方直线
                        start_point = new PointF(center_point.X + rebarB / 2 * zoom, center_point.Y + rebarH / 2 * zoom + longitudinal_rebar_diameter / 2 * zoom);
                        end_point = new PointF(center_point.X - rebarB / 2 * zoom, center_point.Y + rebarH / 2 * zoom + longitudinal_rebar_diameter / 2 * zoom);
                        graphics.DrawLine(rebar_pen, start_point, end_point);
                        //左下角弧
                        graphics.DrawArc(rebar_pen, center_point.X - rebarB / 2 * zoom - longitudinal_rebar_diameter / 2 * zoom, center_point.Y + rebarH / 2 * zoom - longitudinal_rebar_diameter / 2 * zoom, longitudinal_rebar_diameter * zoom, longitudinal_rebar_diameter * zoom, 90, 90);
                        graphics.FillEllipse(Brushes.Red, center_point.X - rebarB / 2 * zoom - longitudinal_rebar_diameter / 2 * zoom, center_point.Y + rebarH / 2 * zoom - longitudinal_rebar_diameter / 2 * zoom, longitudinal_rebar_diameter * zoom, longitudinal_rebar_diameter * zoom);
                        //左侧直线
                        start_point = new PointF(center_point.X - rebarB / 2 * zoom - longitudinal_rebar_diameter / 2 * zoom, center_point.Y + rebarH / 2 * zoom);
                        end_point = new PointF(center_point.X - rebarB / 2 * zoom - longitudinal_rebar_diameter / 2 * zoom, center_point.Y - rebarH / 2 * zoom);
                        graphics.DrawLine(rebar_pen, start_point, end_point);
                        //左上角第二段弧
                        graphics.DrawArc(rebar_pen, center_point.X - rebarB / 2 * zoom - longitudinal_rebar_diameter / 2 * zoom, center_point.Y - rebarH / 2 * zoom - longitudinal_rebar_diameter / 2 * zoom, longitudinal_rebar_diameter * zoom, longitudinal_rebar_diameter * zoom, 180, 135);
                        graphics.FillEllipse(Brushes.Red, center_point.X - rebarB / 2 * zoom - longitudinal_rebar_diameter / 2 * zoom, center_point.Y - rebarH / 2 * zoom - longitudinal_rebar_diameter / 2 * zoom, longitudinal_rebar_diameter * zoom, longitudinal_rebar_diameter * zoom);
                        //左上角第二段直段箍筋
                        start_point = new PointF(center_point.X - rebarB / 2 * zoom + longitudinal_rebar_diameter / 2 / (float)Math.Sqrt(2.0) * zoom, center_point.Y - rebarH / 2 * zoom - longitudinal_rebar_diameter / 2 / (float)Math.Sqrt(2.0) * zoom);
                        end_point = new PointF(center_point.X - rebarB / 2 * zoom + longitudinal_rebar_diameter / 2 / (float)Math.Sqrt(2.0) * zoom + bending_length / (float)Math.Sqrt(2.0) * zoom, center_point.Y - rebarH / 2 * zoom - longitudinal_rebar_diameter / 2 / (float)Math.Sqrt(2.0) * zoom + bending_length / (float)Math.Sqrt(2.0) * zoom);
                        graphics.DrawLine(rebar_pen, start_point, end_point);
                        break;
                }
                // Y向箍筋
                switch (n)
                {
                    case 3:
                        rebarB = b - protective_layer_thinckness * 2 - longitudinal_rebar_diameter;    //X向直钢筋的长度

                        //左侧弯折部分
                        start_point = new PointF(center_point.X - rebarB / 2 * zoom - longitudinal_rebar_diameter / 2 / (float)Math.Sqrt(2.0) * zoom + bending_length / (float)Math.Sqrt(2.0) * zoom, center_point.Y + longitudinal_rebar_diameter / 2 / (float)Math.Sqrt(2.0) * zoom + bending_length / (float)Math.Sqrt(2.0) * zoom);
                        end_point = new PointF(center_point.X - rebarB / 2 * zoom - longitudinal_rebar_diameter / 2 / (float)Math.Sqrt(2.0) * zoom, center_point.Y + longitudinal_rebar_diameter / 2 / (float)Math.Sqrt(2.0) * zoom);
                        graphics.DrawLine(rebar_pen, start_point, end_point);
                        //上方弧
                        graphics.DrawArc(rebar_pen, center_point.X - rebarB / 2 * zoom - longitudinal_rebar_diameter / 2 * zoom, center_point.Y - longitudinal_rebar_diameter / 2 * zoom, longitudinal_rebar_diameter * zoom, longitudinal_rebar_diameter * zoom, 135, 135);
                        graphics.FillEllipse(Brushes.Red, center_point.X - rebarB / 2 * zoom - longitudinal_rebar_diameter / 2 * zoom, center_point.Y - longitudinal_rebar_diameter / 2 * zoom, longitudinal_rebar_diameter * zoom, longitudinal_rebar_diameter * zoom);
                        //横向直线
                        start_point = new PointF(center_point.X - rebarB / 2 * zoom, center_point.Y - longitudinal_rebar_diameter / 2 * zoom);
                        end_point = new PointF(center_point.X + rebarB / 2 * zoom, center_point.Y - longitudinal_rebar_diameter / 2 * zoom);
                        graphics.DrawLine(rebar_pen, start_point, end_point);
                        //右侧弧
                        graphics.DrawArc(rebar_pen, center_point.X + rebarB / 2 * zoom - longitudinal_rebar_diameter / 2 * zoom, center_point.Y - longitudinal_rebar_diameter / 2 * zoom, longitudinal_rebar_diameter * zoom, longitudinal_rebar_diameter * zoom, 270, 135);
                        graphics.FillEllipse(Brushes.Red, center_point.X + rebarB / 2 * zoom - longitudinal_rebar_diameter / 2 * zoom, center_point.Y - longitudinal_rebar_diameter / 2 * zoom, longitudinal_rebar_diameter * zoom, longitudinal_rebar_diameter * zoom);
                        //右侧弯折部分
                        start_point = new PointF(center_point.X + rebarB / 2 * zoom + longitudinal_rebar_diameter / 2 / (float)Math.Sqrt(2.0) * zoom, center_point.Y + longitudinal_rebar_diameter / 2 / (float)Math.Sqrt(2.0) * zoom);
                        end_point = new PointF(center_point.X + rebarB / 2 * zoom + longitudinal_rebar_diameter / 2 / (float)Math.Sqrt(2.0) * zoom - bending_length / (float)Math.Sqrt(2.0) * zoom, center_point.Y + longitudinal_rebar_diameter / 2 / (float)Math.Sqrt(2.0) * zoom + bending_length / (float)Math.Sqrt(2.0) * zoom);
                        graphics.DrawLine(rebar_pen, start_point, end_point);
                        break;
                    case 4:
                        rebarB = b - protective_layer_thinckness * 2 - longitudinal_rebar_diameter;    //X向直钢筋的长度
                        rebarH = (h - protective_layer_thinckness * 2 - longitudinal_rebar_diameter) / 3;          //Y向直钢筋的长度

                        //左下角第一段弯折箍筋
                        start_point = new PointF(center_point.X - rebarB / 2 * zoom + longitudinal_rebar_diameter / 2 / (float)Math.Sqrt(2.0) * zoom + bending_length / (float)Math.Sqrt(2.0) * zoom, center_point.Y + rebarH / 2 * zoom + longitudinal_rebar_diameter / 2 / (float)Math.Sqrt(2.0) * zoom - bending_length / (float)Math.Sqrt(2.0) * zoom);
                        end_point = new PointF(center_point.X - rebarB / 2 * zoom + longitudinal_rebar_diameter / 2 / (float)Math.Sqrt(2.0) * zoom, center_point.Y + rebarH / 2 * zoom + longitudinal_rebar_diameter / 2 / (float)Math.Sqrt(2.0) * zoom);
                        graphics.DrawLine(rebar_pen, start_point, end_point);
                        //左下角第一段弧
                        graphics.DrawArc(rebar_pen, center_point.X - rebarB / 2 * zoom - longitudinal_rebar_diameter / 2 * zoom, center_point.Y + rebarH / 2 * zoom - longitudinal_rebar_diameter / 2 * zoom, longitudinal_rebar_diameter * zoom, longitudinal_rebar_diameter * zoom, 45, 135);
                        graphics.FillEllipse(Brushes.Red, center_point.X - rebarB / 2 * zoom - longitudinal_rebar_diameter / 2 * zoom, center_point.Y + rebarH / 2 * zoom - longitudinal_rebar_diameter / 2 * zoom, longitudinal_rebar_diameter * zoom, longitudinal_rebar_diameter * zoom);
                        //左侧直线
                        start_point = new PointF(center_point.X - rebarB / 2 * zoom - longitudinal_rebar_diameter / 2 * zoom, center_point.Y + rebarH / 2 * zoom);
                        end_point = new PointF(center_point.X - rebarB / 2 * zoom - longitudinal_rebar_diameter / 2 * zoom, center_point.Y - rebarH / 2 * zoom);
                        graphics.DrawLine(rebar_pen, start_point, end_point);
                        //左上角弧
                        graphics.DrawArc(rebar_pen, center_point.X - rebarB / 2 * zoom - longitudinal_rebar_diameter / 2 * zoom, center_point.Y - rebarH / 2 * zoom - longitudinal_rebar_diameter / 2 * zoom, longitudinal_rebar_diameter * zoom, longitudinal_rebar_diameter * zoom, 180, 90);
                        graphics.FillEllipse(Brushes.Red, center_point.X - rebarB / 2 * zoom - longitudinal_rebar_diameter / 2 * zoom, center_point.Y - rebarH / 2 * zoom - longitudinal_rebar_diameter / 2 * zoom, longitudinal_rebar_diameter * zoom, longitudinal_rebar_diameter * zoom);
                        //上方直线
                        start_point = new PointF(center_point.X - rebarB / 2 * zoom, center_point.Y - rebarH / 2 * zoom - longitudinal_rebar_diameter / 2 * zoom);
                        end_point = new PointF(center_point.X + rebarB / 2 * zoom, center_point.Y - rebarH / 2 * zoom - longitudinal_rebar_diameter / 2 * zoom);
                        graphics.DrawLine(rebar_pen, start_point, end_point);
                        //右上角弧
                        graphics.DrawArc(rebar_pen, center_point.X + rebarB / 2 * zoom - longitudinal_rebar_diameter / 2 * zoom, center_point.Y - rebarH / 2 * zoom - longitudinal_rebar_diameter / 2 * zoom, longitudinal_rebar_diameter * zoom, longitudinal_rebar_diameter * zoom, 270, 90);
                        graphics.FillEllipse(Brushes.Red, center_point.X + rebarB / 2 * zoom - longitudinal_rebar_diameter / 2 * zoom, center_point.Y - rebarH / 2 * zoom - longitudinal_rebar_diameter / 2 * zoom, longitudinal_rebar_diameter * zoom, longitudinal_rebar_diameter * zoom);
                        //右侧直线
                        start_point = new PointF(center_point.X + rebarB / 2 * zoom + longitudinal_rebar_diameter / 2 * zoom, center_point.Y - rebarH / 2 * zoom);
                        end_point = new PointF(center_point.X + rebarB / 2 * zoom + longitudinal_rebar_diameter / 2 * zoom, center_point.Y + rebarH / 2 * zoom);
                        graphics.DrawLine(rebar_pen, start_point, end_point);
                        //右下角弧
                        graphics.DrawArc(rebar_pen, center_point.X + rebarB / 2 * zoom - longitudinal_rebar_diameter / 2 * zoom, center_point.Y + rebarH / 2 * zoom - longitudinal_rebar_diameter / 2 * zoom, longitudinal_rebar_diameter * zoom, longitudinal_rebar_diameter * zoom, 0, 90);
                        graphics.FillEllipse(Brushes.Red, center_point.X + rebarB / 2 * zoom - longitudinal_rebar_diameter / 2 * zoom, center_point.Y + rebarH / 2 * zoom - longitudinal_rebar_diameter / 2 * zoom, longitudinal_rebar_diameter * zoom, longitudinal_rebar_diameter * zoom);
                        //下方直线
                        start_point = new PointF(center_point.X + rebarB / 2 * zoom, center_point.Y + rebarH / 2 * zoom + longitudinal_rebar_diameter / 2 * zoom);
                        end_point = new PointF(center_point.X - rebarB / 2 * zoom, center_point.Y + rebarH / 2 * zoom + longitudinal_rebar_diameter / 2 * zoom);
                        graphics.DrawLine(rebar_pen, start_point, end_point);
                        //左下角第二段弧
                        graphics.DrawArc(rebar_pen, center_point.X - rebarB / 2 * zoom - longitudinal_rebar_diameter / 2 * zoom, center_point.Y + rebarH / 2 * zoom - longitudinal_rebar_diameter / 2 * zoom, longitudinal_rebar_diameter * zoom, longitudinal_rebar_diameter * zoom, 90, 135);
                        graphics.FillEllipse(Brushes.Red, center_point.X - rebarB / 2 * zoom - longitudinal_rebar_diameter / 2 * zoom, center_point.Y + rebarH / 2 * zoom - longitudinal_rebar_diameter / 2 * zoom, longitudinal_rebar_diameter * zoom, longitudinal_rebar_diameter * zoom);
                        //左下角第二段弯折箍筋
                        start_point = new PointF(center_point.X - rebarB / 2 * zoom - longitudinal_rebar_diameter / 2 / (float)Math.Sqrt(2.0) * zoom, center_point.Y + rebarH / 2 * zoom - longitudinal_rebar_diameter / 2 / (float)Math.Sqrt(2.0) * zoom);
                        end_point = new PointF(center_point.X - rebarB / 2 * zoom - longitudinal_rebar_diameter / 2 / (float)Math.Sqrt(2.0) * zoom + bending_length / (float)Math.Sqrt(2.0) * zoom, center_point.Y + rebarH / 2 * zoom - longitudinal_rebar_diameter / 2 / (float)Math.Sqrt(2.0) * zoom - bending_length / (float)Math.Sqrt(2.0) * zoom);
                        graphics.DrawLine(rebar_pen, start_point, end_point);
                        break;
                }

                AnnotationUtil.drawLengthAnnotation(graphics, new PointF(center_point.X - b / 2 * zoom, center_point.Y + h / 2 * zoom + 2 * AnnotationUtil.ANNOTATIONEND), new PointF(center_point.X + b / 2 * zoom, center_point.Y + h / 2 * zoom + 2 * AnnotationUtil.ANNOTATIONEND), Convert.ToString(b));
                AnnotationUtil.drawLengthAnnotation(graphics, new PointF(center_point.X + b / 2 * zoom + 2 * AnnotationUtil.ANNOTATIONEND, center_point.Y + h / 2 * zoom), new PointF(center_point.X + b / 2 * zoom + 2 * AnnotationUtil.ANNOTATIONEND, center_point.Y - h / 2 * zoom), Convert.ToString(h));

                //释放资源
                rebar_pen.Dispose();
                concrete_pen.Dispose();
                //触发重绘事件
                picturebox.Invalidate();
            }
            catch (OutOfMemoryException ome)
            {
                Console.WriteLine(ome.Message);
                tip.Text = "参数无效";
            }
        }
        private void redraw2()      //画第2种箍筋
        {
            try
            {
                //放大缩小比例
                zoomB = (picturebox.Width - PADDING - PADDING) / b;
                zoomH = (picturebox.Height - PADDING - PADDING) / h;
                zoom = Math.Min(zoomB, zoomH);

                bmp = new Bitmap(picturebox.Width, picturebox.Height);
                Graphics graphics = Graphics.FromImage(bmp);

                concrete_pen = new Pen(Color.FromArgb(155, 112, 199), 3);    //混凝土线
                rebar_pen = new Pen(Color.Red, 2);                           //钢筋线

                center_point.X = picturebox.Width / 2;  //中心点X向坐标
                center_point.Y = picturebox.Height / 2; //中心点Y向坐标

                //混凝土边框
                PointF[] points = new PointF[5];
                points[0] = new PointF(center_point.X + b / 2 * zoom, center_point.Y - h / 2 * zoom);
                points[1] = new PointF(center_point.X + b / 2 * zoom, center_point.Y + h / 2 * zoom);
                points[2] = new PointF(center_point.X - b / 2 * zoom, center_point.Y + h / 2 * zoom);
                points[3] = new PointF(center_point.X - b / 2 * zoom, center_point.Y - h / 2 * zoom);
                points[4] = new PointF(center_point.X + b / 2 * zoom, center_point.Y - h / 2 * zoom);
                graphics.DrawLines(concrete_pen, points);

                float rebar_b = b - protective_layer_thinckness * 2 - longitudinal_rebar_diameter;    //X向直钢筋的长度
                float rebar_h = h - protective_layer_thinckness * 2 - longitudinal_rebar_diameter;    //Y向直钢筋的长度

                //右上角的弧(包括弧和弯折长度)
                PointF right_up = new PointF(center_point.X + rebar_b / 2 * zoom, center_point.Y - rebar_h / 2 * zoom);  //右上角纵筋的圆心
                bending_length = stirrup_diameter * STIRRUPMUTIPLE > 75 ? stirrup_diameter * STIRRUPMUTIPLE : 75;   //箍筋平直段长度  

                graphics.FillEllipse(Brushes.Red, right_up.X - longitudinal_rebar_diameter / 2 * zoom, right_up.Y - longitudinal_rebar_diameter / 2 * zoom, longitudinal_rebar_diameter * zoom, longitudinal_rebar_diameter * zoom);

                end_point = new PointF(right_up.X - longitudinal_rebar_diameter / 2 / (float)Math.Sqrt(2.0) * zoom, right_up.Y - longitudinal_rebar_diameter / 2 / (float)Math.Sqrt(2.0) * zoom);
                start_point = new PointF(right_up.X - longitudinal_rebar_diameter / 2 / (float)Math.Sqrt(2.0) * zoom - bending_length / (float)Math.Sqrt(2.0) * zoom, right_up.Y - longitudinal_rebar_diameter / 2 / (float)Math.Sqrt(2.0) * zoom + bending_length / (float)Math.Sqrt(2.0) * zoom);
                graphics.DrawLine(rebar_pen, start_point, end_point);
                graphics.DrawArc(rebar_pen, right_up.X - longitudinal_rebar_diameter / 2 * zoom, right_up.Y - longitudinal_rebar_diameter / 2 * zoom, longitudinal_rebar_diameter * zoom, longitudinal_rebar_diameter * zoom, 225, 135);

                //右侧线
                PointF[] line1 = new PointF[2];
                line1[0] = new PointF(center_point.X + rebar_b / 2 * zoom + longitudinal_rebar_diameter / 2 * zoom, center_point.Y - rebar_h / 2 * zoom);
                line1[1] = new PointF(center_point.X + rebar_b / 2 * zoom + longitudinal_rebar_diameter / 2 * zoom, center_point.Y + rebar_h / 2 * zoom);
                graphics.DrawLines(rebar_pen, line1);

                //右下角的弧
                graphics.DrawArc(rebar_pen, center_point.X + rebar_b / 2 * zoom - longitudinal_rebar_diameter / 2 * zoom, center_point.Y + rebar_h / 2 * zoom - longitudinal_rebar_diameter / 2 * zoom, longitudinal_rebar_diameter * zoom, longitudinal_rebar_diameter * zoom, 0, 90);
                graphics.FillEllipse(Brushes.Red, center_point.X + rebar_b / 2 * zoom - longitudinal_rebar_diameter / 2 * zoom, center_point.Y + rebar_h / 2 * zoom - longitudinal_rebar_diameter / 2 * zoom, longitudinal_rebar_diameter * zoom, longitudinal_rebar_diameter * zoom);
                //下方线
                PointF[] line2 = new PointF[2];
                line2[0] = new PointF(center_point.X + rebar_b / 2 * zoom, center_point.Y + rebar_h / 2 * zoom + longitudinal_rebar_diameter / 2 * zoom);
                line2[1] = new PointF(center_point.X - rebar_b / 2 * zoom, center_point.Y + rebar_h / 2 * zoom + longitudinal_rebar_diameter / 2 * zoom);
                graphics.DrawLines(rebar_pen, line2);
                //左下角的弧
                graphics.DrawArc(rebar_pen, center_point.X - rebar_b / 2 * zoom - longitudinal_rebar_diameter / 2 * zoom, center_point.Y + rebar_h / 2 * zoom - longitudinal_rebar_diameter / 2 * zoom, longitudinal_rebar_diameter * zoom, longitudinal_rebar_diameter * zoom, 90, 90);
                graphics.FillEllipse(Brushes.Red, center_point.X - rebar_b / 2 * zoom - longitudinal_rebar_diameter / 2 * zoom, center_point.Y + rebar_h / 2 * zoom - longitudinal_rebar_diameter / 2 * zoom, longitudinal_rebar_diameter * zoom, longitudinal_rebar_diameter * zoom);
                //左侧线
                PointF[] line3 = new PointF[2];
                line3[0] = new PointF(center_point.X - rebar_b / 2 * zoom - longitudinal_rebar_diameter / 2 * zoom, center_point.Y + rebar_h / 2 * zoom);
                line3[1] = new PointF(center_point.X - rebar_b / 2 * zoom - longitudinal_rebar_diameter / 2 * zoom, center_point.Y - rebar_h / 2 * zoom);
                graphics.DrawLines(rebar_pen, line3);
                //左上角的弧
                graphics.DrawArc(rebar_pen, center_point.X - rebar_b / 2 * zoom - longitudinal_rebar_diameter / 2 * zoom, center_point.Y - rebar_h / 2 * zoom - longitudinal_rebar_diameter / 2 * zoom, longitudinal_rebar_diameter * zoom, longitudinal_rebar_diameter * zoom, 180, 90);
                graphics.FillEllipse(Brushes.Red, center_point.X - rebar_b / 2 * zoom - longitudinal_rebar_diameter / 2 * zoom, center_point.Y - rebar_h / 2 * zoom - longitudinal_rebar_diameter / 2 * zoom, longitudinal_rebar_diameter * zoom, longitudinal_rebar_diameter * zoom);
                //上方线
                PointF[] line4 = new PointF[2];
                line4[0] = new PointF(center_point.X - rebar_b / 2 * zoom, center_point.Y - rebar_h / 2 * zoom - longitudinal_rebar_diameter / 2 * zoom);
                line4[1] = new PointF(center_point.X + rebar_b / 2 * zoom, center_point.Y - rebar_h / 2 * zoom - longitudinal_rebar_diameter / 2 * zoom);
                graphics.DrawLines(rebar_pen, line4);

                //右上角第二段弯折(包括弧和弯折长度)
                start_point = new PointF(right_up.X + longitudinal_rebar_diameter / 2 / (float)Math.Sqrt(2.0) * zoom, right_up.Y + longitudinal_rebar_diameter / 2 / (float)Math.Sqrt(2.0) * zoom);
                end_point = new PointF(right_up.X + longitudinal_rebar_diameter / 2 / (float)Math.Sqrt(2.0) * zoom - bending_length / (float)Math.Sqrt(2.0) * zoom, right_up.Y + longitudinal_rebar_diameter / 2 / (float)Math.Sqrt(2.0) * zoom + bending_length / (float)Math.Sqrt(2.0) * zoom);
                graphics.DrawLine(rebar_pen, start_point, end_point);
                graphics.DrawArc(rebar_pen, right_up.X - longitudinal_rebar_diameter / 2 * zoom, right_up.Y - longitudinal_rebar_diameter / 2 * zoom, longitudinal_rebar_diameter * zoom, longitudinal_rebar_diameter * zoom, 270, 135);
                graphics.FillEllipse(Brushes.Red, right_up.X - longitudinal_rebar_diameter / 2 * zoom, right_up.Y - longitudinal_rebar_diameter / 2 * zoom, longitudinal_rebar_diameter * zoom, longitudinal_rebar_diameter * zoom);

                AnnotationUtil.drawLengthAnnotation(graphics, new PointF(center_point.X - b / 2 * zoom, center_point.Y + h / 2 * zoom + 2 * AnnotationUtil.ANNOTATIONEND), new PointF(center_point.X + b / 2 * zoom, center_point.Y + h / 2 * zoom + 2 * AnnotationUtil.ANNOTATIONEND), Convert.ToString(b));
                AnnotationUtil.drawLengthAnnotation(graphics, new PointF(center_point.X + b / 2 * zoom + 2 * AnnotationUtil.ANNOTATIONEND, center_point.Y + h / 2 * zoom), new PointF(center_point.X + b / 2 * zoom + 2 * AnnotationUtil.ANNOTATIONEND, center_point.Y - h / 2 * zoom), Convert.ToString(h));

                //释放资源
                concrete_pen.Dispose();
                rebar_pen.Dispose();
                //触发重绘事件
                picturebox.Invalidate();
            }
            catch (OutOfMemoryException ome)
            {
                Console.WriteLine(ome.Message);
                tip.Text = "参数无效";
            }
        }
        private void redraw6()      //画第6种箍筋
        {
            try
            {
                zoom = Math.Min((picturebox.Width - PADDING - PADDING) / d, (picturebox.Height - PADDING - PADDING) / d); //放大缩小比例

                bmp = new Bitmap(picturebox.Width, picturebox.Height);
                Graphics graphics = Graphics.FromImage(bmp);

                center_point.X = picturebox.Width / 2;  //中心点X向坐标
                center_point.Y = picturebox.Height / 2; //中心点Y向坐标

                concrete_pen = new Pen(Color.FromArgb(155, 112, 199), 3);    //混凝土线
                rebar_pen = new Pen(Color.Red, 2);                           //钢筋线

                //混凝土边框
                float concrete_r = d / 2;
                graphics.DrawArc(concrete_pen, center_point.X - concrete_r * zoom, center_point.Y - concrete_r * zoom, d * zoom, d * zoom, 0, 360);

                //箍筋
                float rebar_r = d / 2 - protective_layer_thinckness; //箍筋半径 = 柱子半径 - 保护层厚度
                float angle = lap_length / (2 * (float)Math.PI * rebar_r) * 360;

                //右侧纵筋圆心
                right_up = new PointF(center_point.X + (rebar_r - longitudinal_rebar_diameter / 2) * (float)Math.Sin(angle / 2 / 180 * Math.PI) * zoom, center_point.Y + (rebar_r - longitudinal_rebar_diameter / 2) * (float)Math.Cos(angle / 2 / 180 * Math.PI) * zoom);
                //右侧弯折直段箍筋
                start_point = new PointF(right_up.X + longitudinal_rebar_diameter / 2 * (float)Math.Sin((45 - angle / 2) / 180 * Math.PI) * zoom, right_up.Y - longitudinal_rebar_diameter / 2 * (float)Math.Cos((45 - angle / 2) / 180 * Math.PI) * zoom);
                end_point = new PointF(right_up.X + longitudinal_rebar_diameter / 2 * (float)Math.Sin((45 - angle / 2) / 180 * Math.PI) * zoom - bending_length * (float)Math.Cos((45 - angle / 2) / 180 * Math.PI) * zoom, right_up.Y - longitudinal_rebar_diameter / 2 * (float)Math.Cos((45 - angle / 2) / 180 * Math.PI) * zoom - bending_length * (float)Math.Sin((45 - angle / 2) / 180 * Math.PI) * zoom);
                graphics.DrawLine(rebar_pen, start_point, end_point);
                //右侧弯折弧
                graphics.DrawArc(rebar_pen, right_up.X - longitudinal_rebar_diameter / 2 * zoom, right_up.Y - longitudinal_rebar_diameter / 2 * zoom, longitudinal_rebar_diameter * zoom, longitudinal_rebar_diameter * zoom, 315 - angle / 2, 135);
                graphics.FillEllipse(Brushes.Red, right_up.X - longitudinal_rebar_diameter / 2 * zoom, right_up.Y - longitudinal_rebar_diameter / 2 * zoom, longitudinal_rebar_diameter * zoom, longitudinal_rebar_diameter * zoom);
                //第一次搭接箍筋
                graphics.DrawArc(rebar_pen, center_point.X - rebar_r * zoom, center_point.Y - rebar_r * zoom, rebar_r * 2 * zoom, rebar_r * 2 * zoom, 90 - angle / 2, angle);
                //非搭接区
                graphics.DrawArc(rebar_pen, center_point.X - rebar_r * zoom, center_point.Y - rebar_r * zoom, rebar_r * 2 * zoom, rebar_r * 2 * zoom, 90 + angle / 2, 360 - angle);
                //第二次搭接箍筋
                graphics.DrawArc(rebar_pen, center_point.X - rebar_r * zoom, center_point.Y - rebar_r * zoom, rebar_r * 2 * zoom, rebar_r * 2 * zoom, 90 - angle / 2, angle);
                //左侧纵筋圆心
                PointF left_p = new PointF(center_point.X - (rebar_r - longitudinal_rebar_diameter / 2) * (float)Math.Sin(angle / 2 / 180 * Math.PI) * zoom, center_point.Y + (rebar_r - longitudinal_rebar_diameter / 2) * (float)Math.Cos(angle / 2 / 180 * Math.PI) * zoom);
                //左侧弯折弧
                graphics.DrawArc(rebar_pen, left_p.X - longitudinal_rebar_diameter / 2 * zoom, left_p.Y - longitudinal_rebar_diameter / 2 * zoom, longitudinal_rebar_diameter * zoom, longitudinal_rebar_diameter * zoom, 90 + angle / 2, 135);
                graphics.FillEllipse(Brushes.Red, left_p.X - longitudinal_rebar_diameter / 2 * zoom, left_p.Y - longitudinal_rebar_diameter / 2 * zoom, longitudinal_rebar_diameter * zoom, longitudinal_rebar_diameter * zoom);
                //左侧弯折直段箍筋
                start_point = new PointF(left_p.X - longitudinal_rebar_diameter / 2 * (float)Math.Sin((45 - angle / 2) / 180 * Math.PI) * zoom, left_p.Y - longitudinal_rebar_diameter / 2 * (float)Math.Cos((45 - angle / 2) / 180 * Math.PI) * zoom);
                end_point = new PointF(left_p.X - longitudinal_rebar_diameter / 2 * (float)Math.Sin((45 - angle / 2) / 180 * Math.PI) * zoom + bending_length * (float)Math.Cos((45 - angle / 2) / 180 * Math.PI) * zoom, left_p.Y - longitudinal_rebar_diameter / 2 * (float)Math.Cos((45 - angle / 2) / 180 * Math.PI) * zoom - bending_length * (float)Math.Sin((45 - angle / 2) / 180 * Math.PI) * zoom);
                graphics.DrawLine(rebar_pen, start_point, end_point);

                //释放资源
                concrete_pen.Dispose();
                rebar_pen.Dispose();
                //触发重绘事件
                picturebox.Invalidate();
            }
            catch (OutOfMemoryException ome)
            {
                Console.WriteLine(ome.Message);
                tip.Text = "D值无效";
            }
        }
        private void redraw7()      //画第7种箍筋
        {
            try
            {
                zoom = Math.Min((picturebox.Width - PADDING - PADDING) / d, (picturebox.Height - PADDING - PADDING) / d); //放大缩小比例

                bmp = new Bitmap(picturebox.Width, picturebox.Height);
                Graphics graphics = Graphics.FromImage(bmp);

                concrete_pen = new Pen(Color.FromArgb(155, 112, 199), 3);    //混凝土线
                rebar_pen = new Pen(Color.Red, 2);                           //钢筋线

                //混凝土边框
                float concrete_r = d / 2;
                graphics.DrawArc(concrete_pen, center_point.X - concrete_r * zoom, center_point.Y - concrete_r * zoom, d * zoom, d * zoom, 0, 360);

                //箍筋
                float core_column_diameter = Math.Max(d / 3, 250);         //芯柱长度, 包含了芯柱纵筋直径
                float rebar_r = d / 2 - protective_layer_thinckness;       //圆环箍筋半径

                //下方角的一半(弧度表示)
                float angle = (float)Math.Acos((core_column_diameter / 2 - longitudinal_rebar_diameter / 2) / (rebar_r - longitudinal_rebar_diameter / 2));

                //圆环箍筋
                //左下角纵筋圆心
                left_down = new PointF(center_point.X - (rebar_r - longitudinal_rebar_diameter / 2) * (float)Math.Sin(angle) * zoom, center_point.Y + (rebar_r - longitudinal_rebar_diameter / 2) * (float)Math.Cos(angle) * zoom);
                //左下第一段弯折平直段箍筋
                start_point = new PointF(left_down.X + longitudinal_rebar_diameter / 2 * (float)Math.Cos(angle) * zoom + bending_length * (float)Math.Sin(angle) * zoom, left_down.Y + longitudinal_rebar_diameter / 2 * (float)Math.Sin(angle) * zoom - bending_length * (float)Math.Cos(angle) * zoom);
                end_point = new PointF(left_down.X + longitudinal_rebar_diameter / 2 * (float)Math.Cos(angle) * zoom, left_down.Y + longitudinal_rebar_diameter / 2 * (float)Math.Sin(angle) * zoom);
                graphics.DrawLine(rebar_pen, start_point, end_point);
                //第一段弧
                graphics.DrawArc(rebar_pen, left_down.X - longitudinal_rebar_diameter / 2 * zoom, left_down.Y - longitudinal_rebar_diameter / 2 * zoom, longitudinal_rebar_diameter * zoom, longitudinal_rebar_diameter * zoom, angle / (float)Math.PI * 180, 90);
                graphics.FillEllipse(Brushes.Red, left_down.X - longitudinal_rebar_diameter / 2 * zoom, left_down.Y - longitudinal_rebar_diameter / 2 * zoom, longitudinal_rebar_diameter * zoom, longitudinal_rebar_diameter * zoom);
                //圆环部分
                graphics.DrawArc(rebar_pen, center_point.X - rebar_r * zoom, center_point.Y - rebar_r * zoom, rebar_r * 2 * zoom, rebar_r * 2 * zoom, 90 + angle / (float)Math.PI * 180, 360);
                //第二段弧
                graphics.DrawArc(rebar_pen, left_down.X - longitudinal_rebar_diameter / 2 * zoom, left_down.Y - longitudinal_rebar_diameter / 2 * zoom, longitudinal_rebar_diameter * zoom, longitudinal_rebar_diameter * zoom, angle / (float)Math.PI * 180 + 90, 90);
                graphics.FillEllipse(Brushes.Red, left_down.X - longitudinal_rebar_diameter / 2 * zoom, left_down.Y - longitudinal_rebar_diameter / 2 * zoom, longitudinal_rebar_diameter * zoom, longitudinal_rebar_diameter * zoom);
                //左下第二段弯折平直段箍筋
                start_point = new PointF(left_down.X - longitudinal_rebar_diameter / 2 * (float)Math.Cos(angle) * zoom, left_down.Y - longitudinal_rebar_diameter / 2 * (float)Math.Sin(angle) * zoom);
                end_point = new PointF(left_down.X - longitudinal_rebar_diameter / 2 * (float)Math.Cos(angle) * zoom + bending_length * (float)Math.Sin(angle) * zoom, left_down.Y - longitudinal_rebar_diameter / 2 * (float)Math.Sin(angle) * zoom - bending_length * (float)Math.Cos(angle) * zoom);
                graphics.DrawLine(rebar_pen, start_point, end_point);

                //X向封闭箍筋
                //四角纵筋圆心
                left_down = new PointF(center_point.X - (rebar_r - longitudinal_rebar_diameter / 2) * (float)Math.Sin(angle) * zoom, center_point.Y + (rebar_r - longitudinal_rebar_diameter / 2) * (float)Math.Cos(angle) * zoom);
                left_up = new PointF(center_point.X - (rebar_r - longitudinal_rebar_diameter / 2) * (float)Math.Sin(angle) * zoom, center_point.Y - (rebar_r - longitudinal_rebar_diameter / 2) * (float)Math.Cos(angle) * zoom);
                right_up = new PointF(center_point.X + (rebar_r - longitudinal_rebar_diameter / 2) * (float)Math.Sin(angle) * zoom, center_point.Y - (rebar_r - longitudinal_rebar_diameter / 2) * (float)Math.Cos(angle) * zoom);
                right_down = new PointF(center_point.X + (rebar_r - longitudinal_rebar_diameter / 2) * (float)Math.Sin(angle) * zoom, center_point.Y + (rebar_r - longitudinal_rebar_diameter / 2) * (float)Math.Cos(angle) * zoom);
                //左下第一段弯折平直段箍筋
                start_point = new PointF(left_down.X + longitudinal_rebar_diameter / 2 * (float)Math.Cos(angle) * zoom + bending_length * (float)Math.Sin(angle) * zoom, left_down.Y + longitudinal_rebar_diameter / 2 * (float)Math.Sin(angle) * zoom - bending_length * (float)Math.Cos(angle) * zoom);
                end_point = new PointF(left_down.X + longitudinal_rebar_diameter / 2 * (float)Math.Cos(angle) * zoom, left_down.Y + longitudinal_rebar_diameter / 2 * (float)Math.Sin(angle) * zoom);
                graphics.DrawLine(rebar_pen, start_point, end_point);
                //左下纵筋弧
                graphics.DrawArc(rebar_pen, left_down.X - longitudinal_rebar_diameter / 2 * zoom, left_down.Y - longitudinal_rebar_diameter / 2 * zoom, longitudinal_rebar_diameter * zoom, longitudinal_rebar_diameter * zoom, angle / (float)Math.PI * 180, 90);
                graphics.FillEllipse(Brushes.Red, left_down.X - longitudinal_rebar_diameter / 2 * zoom, left_down.Y - longitudinal_rebar_diameter / 2 * zoom, longitudinal_rebar_diameter * zoom, longitudinal_rebar_diameter * zoom);
                //左侧大弧
                graphics.DrawArc(rebar_pen, center_point.X - rebar_r * zoom, center_point.Y - rebar_r * zoom, rebar_r * 2 * zoom, rebar_r * 2 * zoom, 90 + angle / (float)Math.PI * 180, (90 - angle / (float)Math.PI * 180) * 2);
                //左上纵筋弧
                graphics.DrawArc(rebar_pen, left_up.X - longitudinal_rebar_diameter / 2 * zoom, left_up.Y - longitudinal_rebar_diameter / 2 * zoom, longitudinal_rebar_diameter * zoom, longitudinal_rebar_diameter * zoom, 270 - angle / (float)Math.PI * 180, angle / (float)Math.PI * 180);
                graphics.FillEllipse(Brushes.Red, left_up.X - longitudinal_rebar_diameter / 2 * zoom, left_up.Y - longitudinal_rebar_diameter / 2 * zoom, longitudinal_rebar_diameter * zoom, longitudinal_rebar_diameter * zoom);
                //上部直段箍筋
                start_point = new PointF(left_up.X, left_up.Y - longitudinal_rebar_diameter / 2 * zoom);
                end_point = new PointF(right_up.X, right_up.Y - longitudinal_rebar_diameter / 2 * zoom);
                graphics.DrawLine(rebar_pen, start_point, end_point);
                //右上纵筋弧
                graphics.DrawArc(rebar_pen, right_up.X - longitudinal_rebar_diameter / 2 * zoom, right_up.Y - longitudinal_rebar_diameter / 2 * zoom, longitudinal_rebar_diameter * zoom, longitudinal_rebar_diameter * zoom, 270, angle / (float)Math.PI * 180);
                graphics.FillEllipse(Brushes.Red, right_up.X - longitudinal_rebar_diameter / 2 * zoom, right_up.Y - longitudinal_rebar_diameter / 2 * zoom, longitudinal_rebar_diameter * zoom, longitudinal_rebar_diameter * zoom);
                //右侧大弧
                graphics.DrawArc(rebar_pen, center_point.X - rebar_r * zoom, center_point.Y - rebar_r * zoom, rebar_r * 2 * zoom, rebar_r * 2 * zoom, 270 + angle / (float)Math.PI * 180, (90 - angle / (float)Math.PI * 180) * 2);
                //右下纵筋弧
                graphics.DrawArc(rebar_pen, right_down.X - longitudinal_rebar_diameter / 2 * zoom, right_down.Y - longitudinal_rebar_diameter / 2 * zoom, longitudinal_rebar_diameter * zoom, longitudinal_rebar_diameter * zoom, 90 - angle / (float)Math.PI * 180, angle / (float)Math.PI * 180);
                graphics.FillEllipse(Brushes.Red, right_down.X - longitudinal_rebar_diameter / 2 * zoom, right_down.Y - longitudinal_rebar_diameter / 2 * zoom, longitudinal_rebar_diameter * zoom, longitudinal_rebar_diameter * zoom);
                //下方直段箍筋
                start_point = new PointF(right_down.X, right_down.Y + longitudinal_rebar_diameter / 2 * zoom);
                end_point = new PointF(left_down.X, left_down.Y + longitudinal_rebar_diameter / 2 * zoom);
                graphics.DrawLine(rebar_pen, start_point, end_point);
                //左下纵筋弧
                graphics.DrawArc(rebar_pen, left_down.X - longitudinal_rebar_diameter / 2 * zoom, left_down.Y - longitudinal_rebar_diameter / 2 * zoom, longitudinal_rebar_diameter * zoom, longitudinal_rebar_diameter * zoom, 90 + angle / (float)Math.PI * 180, 90);
                graphics.FillEllipse(Brushes.Red, left_down.X - longitudinal_rebar_diameter / 2 * zoom, left_down.Y - longitudinal_rebar_diameter / 2 * zoom, longitudinal_rebar_diameter * zoom, longitudinal_rebar_diameter * zoom);
                //左下第二段弯折平直段箍筋
                start_point = new PointF(left_down.X - longitudinal_rebar_diameter / 2 * (float)Math.Cos(angle) * zoom, left_down.Y - longitudinal_rebar_diameter / 2 * (float)Math.Sin(angle) * zoom);
                end_point = new PointF(left_down.X - longitudinal_rebar_diameter / 2 * (float)Math.Cos(angle) * zoom + bending_length * (float)Math.Sin(angle) * zoom, left_down.Y - longitudinal_rebar_diameter / 2 * (float)Math.Sin(angle) * zoom - bending_length * (float)Math.Cos(angle) * zoom);
                graphics.DrawLine(rebar_pen, start_point, end_point);

                left_down = new PointF(center_point.X - (rebar_r - longitudinal_rebar_diameter / 2) * (float)Math.Cos(angle) * zoom, center_point.Y + (rebar_r - longitudinal_rebar_diameter / 2) * (float)Math.Sin(angle) * zoom);
                left_up = new PointF(center_point.X - (rebar_r - longitudinal_rebar_diameter / 2) * (float)Math.Cos(angle) * zoom, center_point.Y - (rebar_r - longitudinal_rebar_diameter / 2) * (float)Math.Sin(angle) * zoom);
                right_up = new PointF(center_point.X + (rebar_r - longitudinal_rebar_diameter / 2) * (float)Math.Cos(angle) * zoom, center_point.Y - (rebar_r - longitudinal_rebar_diameter / 2) * (float)Math.Sin(angle) * zoom);
                right_down = new PointF(center_point.X + (rebar_r - longitudinal_rebar_diameter / 2) * (float)Math.Cos(angle) * zoom, center_point.Y + (rebar_r - longitudinal_rebar_diameter / 2) * (float)Math.Sin(angle) * zoom);

                //Y向封闭箍筋
                //右下第一段弯折平直段箍筋
                start_point = new PointF(right_down.X + longitudinal_rebar_diameter / 2 * (float)Math.Sin(angle) * zoom - bending_length * (float)Math.Cos(angle) * zoom, right_down.Y - longitudinal_rebar_diameter / 2 * (float)Math.Cos(angle) * zoom - bending_length * (float)Math.Sin(angle) * zoom);
                end_point = new PointF(right_down.X + longitudinal_rebar_diameter / 2 * (float)Math.Sin(angle) * zoom, right_down.Y - longitudinal_rebar_diameter / 2 * (float)Math.Cos(angle) * zoom);
                graphics.DrawLine(rebar_pen, start_point, end_point);
                //右下纵筋弧
                graphics.DrawArc(rebar_pen, right_down.X - longitudinal_rebar_diameter / 2 * zoom, right_down.Y - longitudinal_rebar_diameter / 2 * zoom, longitudinal_rebar_diameter * zoom, longitudinal_rebar_diameter * zoom, 270 + angle / (float)Math.PI * 180, 90);
                graphics.FillEllipse(Brushes.Red, right_down.X - longitudinal_rebar_diameter / 2 * zoom, right_down.Y - longitudinal_rebar_diameter / 2 * zoom, longitudinal_rebar_diameter * zoom, longitudinal_rebar_diameter * zoom);
                //下方大弧
                graphics.DrawArc(rebar_pen, center_point.X - rebar_r * zoom, center_point.Y - rebar_r * zoom, rebar_r * 2 * zoom, rebar_r * 2 * zoom, angle / (float)Math.PI * 180, (90 - angle / (float)Math.PI * 180) * 2);
                //左下纵筋弧
                graphics.DrawArc(rebar_pen, left_down.X - longitudinal_rebar_diameter / 2 * zoom, left_down.Y - longitudinal_rebar_diameter / 2 * zoom, longitudinal_rebar_diameter * zoom, longitudinal_rebar_diameter * zoom, 180 - angle / (float)Math.PI * 180, angle / (float)Math.PI * 180);
                graphics.FillEllipse(Brushes.Red, left_down.X - longitudinal_rebar_diameter / 2 * zoom, left_down.Y - longitudinal_rebar_diameter / 2 * zoom, longitudinal_rebar_diameter * zoom, longitudinal_rebar_diameter * zoom);
                //左侧直段箍筋
                start_point = new PointF(left_down.X - longitudinal_rebar_diameter / 2 * zoom, left_down.Y);
                end_point = new PointF(left_up.X - longitudinal_rebar_diameter / 2 * zoom, left_up.Y);
                graphics.DrawLine(rebar_pen, start_point, end_point);
                //左上纵筋弧
                graphics.DrawArc(rebar_pen, left_up.X - longitudinal_rebar_diameter / 2 * zoom, left_up.Y - longitudinal_rebar_diameter / 2 * zoom, longitudinal_rebar_diameter * zoom, longitudinal_rebar_diameter * zoom, 180, angle / (float)Math.PI * 180);
                graphics.FillEllipse(Brushes.Red, left_up.X - longitudinal_rebar_diameter / 2 * zoom, left_up.Y - longitudinal_rebar_diameter / 2 * zoom, longitudinal_rebar_diameter * zoom, longitudinal_rebar_diameter * zoom);
                //上方大弧
                graphics.DrawArc(rebar_pen, center_point.X - rebar_r * zoom, center_point.Y - rebar_r * zoom, rebar_r * 2 * zoom, rebar_r * 2 * zoom, 180 + angle / (float)Math.PI * 180, (90 - angle / (float)Math.PI * 180) * 2);
                //右上纵筋弧
                graphics.DrawArc(rebar_pen, right_up.X - longitudinal_rebar_diameter / 2 * zoom, right_up.Y - longitudinal_rebar_diameter / 2 * zoom, longitudinal_rebar_diameter * zoom, longitudinal_rebar_diameter * zoom, 360 - angle / (float)Math.PI * 180, angle / (float)Math.PI * 180);
                graphics.FillEllipse(Brushes.Red, right_up.X - longitudinal_rebar_diameter / 2 * zoom, right_up.Y - longitudinal_rebar_diameter / 2 * zoom, longitudinal_rebar_diameter * zoom, longitudinal_rebar_diameter * zoom);
                //左侧直段箍筋
                start_point = new PointF(right_up.X + longitudinal_rebar_diameter / 2 * zoom, right_up.Y);
                end_point = new PointF(right_down.X + longitudinal_rebar_diameter / 2 * zoom, right_down.Y);
                graphics.DrawLine(rebar_pen, start_point, end_point);
                //右下纵筋弧
                graphics.DrawArc(rebar_pen, right_down.X - longitudinal_rebar_diameter / 2 * zoom, right_down.Y - longitudinal_rebar_diameter / 2 * zoom, longitudinal_rebar_diameter * zoom, longitudinal_rebar_diameter * zoom, 0, 90 + angle / (float)Math.PI * 180);
                graphics.FillEllipse(Brushes.Red, right_down.X - longitudinal_rebar_diameter / 2 * zoom, right_down.Y - longitudinal_rebar_diameter / 2 * zoom, longitudinal_rebar_diameter * zoom, longitudinal_rebar_diameter * zoom);
                //右下第二段弯折平直段箍筋
                start_point = new PointF(right_down.X - longitudinal_rebar_diameter / 2 * (float)Math.Sin(angle) * zoom, right_down.Y + longitudinal_rebar_diameter / 2 * (float)Math.Cos(angle) * zoom);
                end_point = new PointF(right_down.X - longitudinal_rebar_diameter / 2 * (float)Math.Sin(angle) * zoom - bending_length * (float)Math.Cos(angle) * zoom, right_down.Y + longitudinal_rebar_diameter / 2 * (float)Math.Cos(angle) * zoom - bending_length * (float)Math.Sin(angle) * zoom);
                graphics.DrawLine(rebar_pen, start_point, end_point);

                //芯柱箍筋
                left_down = new PointF(center_point.X - core_column_diameter / 2 * zoom + longitudinal_rebar_diameter / 2 * zoom, center_point.Y + core_column_diameter / 2 * zoom - longitudinal_rebar_diameter / 2 * zoom);
                left_up = new PointF(center_point.X - core_column_diameter / 2 * zoom + longitudinal_rebar_diameter / 2 * zoom, center_point.Y - core_column_diameter / 2 * zoom + longitudinal_rebar_diameter / 2 * zoom);
                right_up = new PointF(center_point.X + core_column_diameter / 2 * zoom - longitudinal_rebar_diameter / 2 * zoom, center_point.Y - core_column_diameter / 2 * zoom + longitudinal_rebar_diameter / 2 * zoom);
                right_down = new PointF(center_point.X + core_column_diameter / 2 * zoom - longitudinal_rebar_diameter / 2 * zoom, center_point.Y + core_column_diameter / 2 * zoom - longitudinal_rebar_diameter / 2 * zoom);
                //右上角第一段弯折平直段箍筋
                start_point = new PointF(right_up.X - longitudinal_rebar_diameter / 2 / (float)Math.Sqrt(2.0) * zoom - bending_length / (float)Math.Sqrt(2.0) * zoom, right_up.Y - longitudinal_rebar_diameter / 2 / (float)Math.Sqrt(2.0) * zoom + bending_length / (float)Math.Sqrt(2.0) * zoom);
                end_point = new PointF(right_up.X - longitudinal_rebar_diameter / 2 / (float)Math.Sqrt(2.0) * zoom, right_up.Y - longitudinal_rebar_diameter / 2 / (float)Math.Sqrt(2.0) * zoom);
                graphics.DrawLine(rebar_pen, start_point, end_point);
                //右上角第一段纵筋弧
                graphics.DrawArc(rebar_pen, right_up.X - longitudinal_rebar_diameter / 2 * zoom, right_up.Y - longitudinal_rebar_diameter / 2 * zoom, longitudinal_rebar_diameter * zoom, longitudinal_rebar_diameter * zoom, 225, 135);
                graphics.FillEllipse(Brushes.Red, right_up.X - longitudinal_rebar_diameter / 2 * zoom, right_up.Y - longitudinal_rebar_diameter / 2 * zoom, longitudinal_rebar_diameter * zoom, longitudinal_rebar_diameter * zoom);
                //右侧箍筋
                start_point = new PointF(right_up.X + longitudinal_rebar_diameter / 2 * zoom, right_up.Y);
                end_point = new PointF(right_down.X + longitudinal_rebar_diameter / 2 * zoom, right_down.Y);
                graphics.DrawLine(rebar_pen, start_point, end_point);
                //右下角纵筋弧
                graphics.DrawArc(rebar_pen, right_down.X - longitudinal_rebar_diameter / 2 * zoom, right_down.Y - longitudinal_rebar_diameter / 2 * zoom, longitudinal_rebar_diameter * zoom, longitudinal_rebar_diameter * zoom, 0, 90);
                graphics.FillEllipse(Brushes.Red, right_down.X - longitudinal_rebar_diameter / 2 * zoom, right_down.Y - longitudinal_rebar_diameter / 2 * zoom, longitudinal_rebar_diameter * zoom, longitudinal_rebar_diameter * zoom);
                //下方箍筋
                start_point = new PointF(right_down.X, right_down.Y + longitudinal_rebar_diameter / 2 * zoom);
                end_point = new PointF(left_down.X, left_down.Y + longitudinal_rebar_diameter / 2 * zoom);
                graphics.DrawLine(rebar_pen, start_point, end_point);
                //左下角纵筋弧
                graphics.DrawArc(rebar_pen, left_down.X - longitudinal_rebar_diameter / 2 * zoom, left_down.Y - longitudinal_rebar_diameter / 2 * zoom, longitudinal_rebar_diameter * zoom, longitudinal_rebar_diameter * zoom, 90, 90);
                graphics.FillEllipse(Brushes.Red, left_down.X - longitudinal_rebar_diameter / 2 * zoom, left_down.Y - longitudinal_rebar_diameter / 2 * zoom, longitudinal_rebar_diameter * zoom, longitudinal_rebar_diameter * zoom);
                //左侧箍筋
                start_point = new PointF(left_down.X - longitudinal_rebar_diameter / 2 * zoom, left_down.Y);
                end_point = new PointF(left_up.X - longitudinal_rebar_diameter / 2 * zoom, left_up.Y);
                graphics.DrawLine(rebar_pen, start_point, end_point);
                //左上角纵筋弧
                graphics.DrawArc(rebar_pen, left_up.X - longitudinal_rebar_diameter / 2 * zoom, left_up.Y - longitudinal_rebar_diameter / 2 * zoom, longitudinal_rebar_diameter * zoom, longitudinal_rebar_diameter * zoom, 180, 90);
                graphics.FillEllipse(Brushes.Red, left_up.X - longitudinal_rebar_diameter / 2 * zoom, left_up.Y - longitudinal_rebar_diameter / 2 * zoom, longitudinal_rebar_diameter * zoom, longitudinal_rebar_diameter * zoom);
                //上方箍筋
                start_point = new PointF(left_up.X, left_up.Y - longitudinal_rebar_diameter / 2 * zoom);
                end_point = new PointF(right_up.X, left_up.Y - longitudinal_rebar_diameter / 2 * zoom);
                graphics.DrawLine(rebar_pen, start_point, end_point);
                //右上角第二段纵筋弧
                graphics.DrawArc(rebar_pen, right_up.X - longitudinal_rebar_diameter / 2 * zoom, right_up.Y - longitudinal_rebar_diameter / 2 * zoom, longitudinal_rebar_diameter * zoom, longitudinal_rebar_diameter * zoom, 270, 135);
                graphics.FillEllipse(Brushes.Red, right_up.X - longitudinal_rebar_diameter / 2 * zoom, right_up.Y - longitudinal_rebar_diameter / 2 * zoom, longitudinal_rebar_diameter * zoom, longitudinal_rebar_diameter * zoom);
                //右上角第二段弯折平直段箍筋
                start_point = new PointF(right_up.X + longitudinal_rebar_diameter / 2 / (float)Math.Sqrt(2.0) * zoom, right_up.Y + longitudinal_rebar_diameter / 2 / (float)Math.Sqrt(2.0) * zoom);
                end_point = new PointF(right_up.X + longitudinal_rebar_diameter / 2 / (float)Math.Sqrt(2.0) * zoom - bending_length / (float)Math.Sqrt(2.0) * zoom, right_up.Y + longitudinal_rebar_diameter / 2 / (float)Math.Sqrt(2.0) * zoom + bending_length / (float)Math.Sqrt(2.0) * zoom);
                graphics.DrawLine(rebar_pen, start_point, end_point);

                //释放资源
                concrete_pen.Dispose();
                rebar_pen.Dispose();
                //触发重绘事件
                picturebox.Invalidate();
            }
            catch (OutOfMemoryException ome)
            {
                Console.WriteLine(ome.Message);
                tip.Text = "D值无效";
            }
        }

        private void picturebox1_Paint(object sender, PaintEventArgs e) //picture的重绘事件
        {
            Graphics g = e.Graphics;
            if (bmp == null)
            {
                g.Clear(Color.Black);
            }
            else
            {
                g.DrawImage(bmp, new PointF(0, 0));
            }
        }

        //选择配筋类型
        private void choose_stirruptype_Click(object sender, EventArgs e)
        {
            StirrupTypeForm stirrup_type_form = new StirrupTypeForm();
            if (stirrup_type_form.ShowDialog() == DialogResult.OK)
            {
                type = stirrup_type_form.type;
                switch (type)
                {
                    case "1":
                        init_parameters1_controls();
                        redraw1();
                        break;
                    case "2":
                        init_parameters2_controls();
                        redraw2();
                        break;
                    case "6":
                        if (init_parameters6_controls())
                        {
                            redraw6();
                        }
                        break;
                    case "7":
                        if (init_parameters7_controls())
                        {
                            redraw7();
                        }
                        break;
                }
            }
        }   
        private void textboxb_KeyPress(object sender, KeyPressEventArgs e)
        {
            if (e.KeyChar != '\b')  //这是允许输入退格键  
            {
                if ((e.KeyChar < '0') || (e.KeyChar > '9'))     //这是允许输入0-9数字  
                {
                    e.Handled = true;
                }
            }
        }
        private void textboxb_TextChanged(object sender, EventArgs e)
        {
            TextBox textbox_b = (TextBox)sender;
            if (textbox_b.Text.Trim().Equals(""))
            {
                tip.Text = "参数无效";
                return;
            }
            else if (Convert.ToInt32(textbox_b.Text.Trim()) <= 0)
            {
                tip.Text = "参数无效";
                return;
            }
            else 
            {
                tip.Text = "";
                float.TryParse(textbox_b.Text, out b);
            }
            switch (type)
            {
                case "1":
                    redraw1();
                    break;
                case "2":
                    redraw2();
                    break;
            }
        }
        private void textboxh_KeyPress(object sender, KeyPressEventArgs e)
        {
            if (e.KeyChar != '\b')  //这是允许输入退格键  
            {
                if ((e.KeyChar < '0') || (e.KeyChar > '9'))     //这是允许输入0-9数字  
                {
                    e.Handled = true;
                }
            }
        }
        private void textboxh_TextChanged(object sender, EventArgs e)
        {
            TextBox textbox_h = (TextBox)sender;
            if (textbox_h.Text.Trim().Equals(""))
            {
                tip.Text = "参数无效";
            }
            else if (Convert.ToInt32(textbox_h.Text.Trim()) <= 0)
            {
                tip.Text = "参数无效";
            }
            else
            {
                tip.Text = "";
                float.TryParse(textbox_h.Text, out h);
            }
            switch (type)
            {
                case "1":
                    redraw1();
                    break;
                case "2":
                    redraw2();
                    break;
            }
        }
        private void textboxd_KeyPress(object sender, KeyPressEventArgs e)
        {
            if (e.KeyChar != '\b')  //这是允许输入退格键  
            {
                if ((e.KeyChar < '0') || (e.KeyChar > '9'))     //这是允许输入0-9数字  
                {
                    e.Handled = true;
                }
            }
        }
        private void textboxd_TextChanged(object sender, EventArgs e)
        {
            TextBox textbox_d = (TextBox)sender;
            check_textbox_d(textbox_d);
        }
        
        private void comboboxm_SelectedIndexChanged(object sender, EventArgs e)
        {
            ComboBox combobox_m = (ComboBox)sender;
            int.TryParse(combobox_m.SelectedItem.ToString(), out m);
            if (type.Equals("1"))
            {
                redraw1();
            }
        }
        private void comboboxn_SelectedIndexChanged(object sender, EventArgs e)
        {
            ComboBox combobox_n = (ComboBox)sender;
            int.TryParse(combobox_n.SelectedItem.ToString(), out n);
            if (type.Equals("1"))
            {
                redraw1();
            }
        }

        private bool check_textbox_d(TextBox textbox_d)
        {
            bool result = false;
            switch (type)
            {
                case "6":
                    if (textbox_d.Text.Trim().Equals(""))
                    {
                        tip.Text = "D值无效";
                        result = false;
                    }
                    else if (Convert.ToDouble(textbox_d.Text.Trim()) <= lap_length / Math.PI || Convert.ToDouble(textbox_d.Text.Trim()) <= protective_layer_thinckness * 2 + longitudinal_rebar_diameter * 2)
                    {
                        tip.Text = "D值无效";
                        result = false;
                    }
                    else
                    {
                        tip.Text = "";
                        float.TryParse(textbox_d.Text, out d);
                        redraw6();
                        result = true;
                    }
                    break;
                case "7":
                    if (textbox_d.Text.Trim().Equals(""))
                    {
                        tip.Text = "D值无效";
                        result = false;
                    }
                    else if (Convert.ToInt32(textbox_d.Text.Trim()) <= 250 * Math.Sqrt(2.0) + longitudinal_rebar_diameter * 2 + protective_layer_thinckness * 2)
                    {
                        tip.Text = "D值无效";
                        result = false;
                    }
                    else
                    {
                        tip.Text = "";
                        float.TryParse(textbox_d.Text, out d);
                        redraw7();
                        result = true;
                    }
                    break;
            }
            return result;
        }
    }


}
