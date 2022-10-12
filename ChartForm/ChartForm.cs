using InteractiveCenterLux.Models;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace ChartForm
{
    public partial class ChartForm : Form
    {
        private Dashboard model;
        private Button currentButton;

        public ChartForm()
        {
            InitializeComponent();

            //Default - Last 7 days
            dtpStartDate.Value = DateTime.Today.AddDays(-7);
            dtpEndDate.Value = DateTime.Now;
            btnLast7Days.Select();

            SetDateMenuButtonsUI(btnLast7Days);

            model = new Dashboard();
            LoadData();
        }

        private void LoadData()
        {
            var refreshData = model.LoadData(dtpStartDate.Value, dtpEndDate.Value);

            if (refreshData == true)
            {
                lblNumOrders.Text = model.NumOrders.ToString();
                lblTotalRevenue.Text = model.TotalRevenue.ToString();
                lblTotalProfit.Text = model.TotalProfit.ToString();

                chartGrossRevenue.DataSource = model.GrossRevenueList;
                chartGrossRevenue.Series[0].XValueMember = "Date";
                chartGrossRevenue.Series[0].YValueMembers = "TotalAmount";
                chartGrossRevenue.DataBind();

                chartTopProducts.DataSource = model.TopProductsList;
                chartTopProducts.Series[0].XValueMember = "Key";
                chartTopProducts.Series[0].YValueMembers = "Value";
                chartTopProducts.DataBind();

                dgvUnderstock.DataSource = model.UnderstockList;

                Console.WriteLine("Loaded view :)");
            }
            else Console.WriteLine("View not loaded, same query");
        }
        private void SetDateMenuButtonsUI(object button)
        {
            var btn = (Button)button;

            btn.BackColor = btnLastMonth.FlatAppearance.BorderColor;
            btn.ForeColor = Color.White;

            if (currentButton != null && currentButton != btn)
            {
                currentButton.BackColor = this.BackColor;
                currentButton.ForeColor = Color.FromName("ActiveCaption");
            }
            currentButton = btn;           
        }
        //Event methods
        private void btnToday_Click(object sender, EventArgs e)
        {
            dtpStartDate.Value = DateTime.Today;
            dtpEndDate.Value = DateTime.Now;
            LoadData();
            SetDateMenuButtonsUI(sender);
        }
        private void btnLast7Days_Click(object sender, EventArgs e)
        {
            dtpStartDate.Value = DateTime.Today.AddDays(-7);
            dtpEndDate.Value = DateTime.Now;
            LoadData();
            SetDateMenuButtonsUI(sender);
        }
        private void btnLast30Days_Click(object sender, EventArgs e)
        {
            dtpStartDate.Value = DateTime.Today.AddDays(-30);
            dtpEndDate.Value = DateTime.Now;
            LoadData();
            SetDateMenuButtonsUI(sender);
        }
        private void btnThisMonth_Click(object sender, EventArgs e)
        {
            dtpStartDate.Value = new DateTime(DateTime.Today.Year, DateTime.Today.Month, 1);
            dtpEndDate.Value = DateTime.Now;
            LoadData();
            SetDateMenuButtonsUI(sender);
        }
        private void btnCustomDate_Click(object sender, EventArgs e)
        {
            dtpStartDate.Enabled = true;
            dtpEndDate.Enabled = true;
            btnOkCustomDate.Visible = true;
            SetDateMenuButtonsUI(sender);

        }
        private void btnOkCustomDate_Click(object sender, EventArgs e)
        {
            dtpStartDate.Enabled = true;
            dtpEndDate.Enabled = true;
            btnOkCustomDate.Visible = true;
            SetDateMenuButtonsUI(sender);
            LoadData();
        }

        private void lblStartDate_Click(object sender, EventArgs e)
        {
            if (currentButton == btnOkCustomDate)
            {
                dtpStartDate.Select();
                SendKeys.Send("%{DOWN}");
            }
        }

        private void lblEndDate_Click(object sender, EventArgs e)
        {
            if (currentButton == btnOkCustomDate)
            {
                dtpEndDate.Select();
                SendKeys.Send("%{DOWN}");
            }
        }

        private void dtpStartDate_ValueChanged(object sender, EventArgs e)
        {
            lblStartDate.Text = dtpStartDate.Text;
        }

        private void dtpEndDate_ValueChanged(object sender, EventArgs e)
        {
            lblEndDate.Text = dtpEndDate.Text;
        }

        private void ChartForm_Load(object sender, EventArgs e)
        {
            lblStartDate.Text = dtpStartDate.Text;
            lblEndDate.Text = dtpEndDate.Text;
        }
                
    }
}