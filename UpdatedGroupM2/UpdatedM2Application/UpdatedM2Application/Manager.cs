using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.IO;


namespace UpdatedM2Application
{
    public partial class Manager : Form
    {
        public Manager()
        {
            InitializeComponent();
        }

        private string RemoveR(string amount)
        {
            string cleanString = amount.Substring(1, amount.Length - 1);
            return cleanString;
        }

        private double CostOfItem()
        {
            double sum = 0;
            int i;
            for (i = 0; i < dataSalesList.Rows.Count; i++)
            {
                sum += Convert.ToDouble(dataSalesList.Rows[i].Cells[3].Value);
            }
            return sum;
        }

        private void AddCost()
        {
            const double tax = 0.15;
            if (dataSalesList.Rows.Count > 0)
            {

                txtTax.Text = (CostOfItem() * tax).ToString();
                txtSubtotal.Text = (CostOfItem() - (CostOfItem() * tax)).ToString();
                txtTotal.Text = CostOfItem().ToString();
            }
        }

        private void Change()
        {
            double x = Convert.ToDouble(txtTotal.Text);
            double y = Convert.ToDouble(txtAmountRecieved.Text);
            if (y >= x)
            {
                double change = y - x;
                txtChange.Text = change.ToString();
            }
            else MessageBox.Show("Please provide a sufficient amount");
        }

        private void LoadIntoComboBox()
        {
            cmbMethodOfPayment.Items.Add("CASH");
            cmbMethodOfPayment.Items.Add("CREDIT CARD");
            cmbMethodOfPayment.Items.Add("DEBIT CARD");
            cmbMethodOfPayment.Items.Add("EFT");
            cmbMethodOfPayment.Items.Add("VOUCHER");
        }

        private void AddCost(double amount)
        {
            double total = Convert.ToDouble(txtTotal.Text) + amount;
            double tax = Convert.ToDouble(txtTax.Text) + (amount * 0.14);
            double subtotal = total - tax;

            txtTax.Text = tax.ToString();
            txtTotal.Text = total.ToString();
            txtSubtotal.Text = subtotal.ToString();
        }

        private void SubtractCost(double amount)
        {
            double total = Convert.ToDouble(txtTotal.Text) - amount;
            double tax = Convert.ToDouble(txtTax.Text) - (amount * 0.14);
            double subtotal = total - tax;

            txtSubtotal.Text = subtotal.ToString();
            txtTotal.Text = total.ToString();
            txtTax.Text = tax.ToString();
        }

        private void button10_Click(object sender, EventArgs e)
        {

        }

        private void button1_Click(object sender, EventArgs e)
        {

        }

        private void Manager_Load(object sender, EventArgs e)
        {
            try
            {
                // TODO: This line of code loads data into the 'group28DataSet.Employee' table. You can move, or remove it, as needed.
                this.employeeTableAdapter.Fill(this.group28DataSet.Employee);
                // TODO: This line of code loads data into the 'group28DataSet.Products' table. You can move, or remove it, as needed.
                this.productsTableAdapter.Fill(this.group28DataSet.Products);
                LoadIntoComboBox();
            }catch
            {
                if(MessageBox.Show("Your internet connection is slow or not available, would you like to reconnect?", "Message", MessageBoxButtons.YesNo, MessageBoxIcon.Question) == DialogResult.Yes)
                {
                    Manager_Load(sender, e);
                }else
                {
                    Application.Exit();
                }
            }

        }

        private void txtPOSSearch_TextChanged(object sender, EventArgs e)
        {
            try
            {
                this.productsTableAdapter.FillBySearch(this.group28DataSet.Products, txtPOSSearch.Text);
            }
            catch
            {
                if (MessageBox.Show("Internet connection is lagging, would you like to retry connecting the the database?", "Internet Connection Error", MessageBoxButtons.YesNo, MessageBoxIcon.Question) == DialogResult.Yes)
                {
                    txtPOSSearch_TextChanged(sender, e);
                }
                else
                {
                    return;
                }
            }
        }

        private void logoutButton_Click(object sender, EventArgs e)
        {
            LogIn f = new LogIn();
            this.Hide();
            f.Show();
        }

        private void button11_Click(object sender, EventArgs e)
        {
            Button b = (Button)sender;
            if (txtAmountRecieved.Text == "0")
            {
                txtAmountRecieved.Text = "";
                txtAmountRecieved.Text = b.Text;
            }
            else if (b.Text == ",")
            {
                if (!(txtAmountRecieved.Text.Contains(",")))
                    txtAmountRecieved.Text += b.Text;
            }
            else
                txtAmountRecieved.Text += b.Text;
        }

        private void button10_Click_1(object sender, EventArgs e)
        {
            cmbMethodOfPayment.Text = "CASH";
            txtAmountRecieved.Text = "0";
            txtChange.Text = "0";
        }

        private void dataProductsList_CellContentClick(object sender, DataGridViewCellEventArgs e)
        {
            DataGridViewSelectedRowCollection rows = dataProductsList.SelectedRows;
            DataGridViewSelectedCellCollection cells = dataProductsList.SelectedCells;
            if (rows.Count > 0)
            {
                MessageBox.Show("Select Cell");
                foreach (DataGridViewRow row in rows)
                {
                    string barcode = row.Cells[0].Value.ToString();
                    string name = row.Cells[1].Value.ToString();
                    double amount = Convert.ToDouble(row.Cells[3].Value.ToString());

                    foreach (DataGridViewRow ROW in dataSalesList.Rows)
                    {
                        if (ROW.Cells[0].Value.ToString() == barcode)
                        {
                            ROW.Cells[2].Value = (Convert.ToInt16(ROW.Cells[2].Value.ToString()) + 1);
                            ROW.Cells[3].Value = (Convert.ToDouble(ROW.Cells[3].Value.ToString()) + amount);
                            AddCost(amount);
                            return;
                        }
                    }
                    dataSalesList.Rows.Add(barcode, name, 1, amount);
                    AddCost(amount);
                    return;
                }
            }
            else if (cells.Count > 0)
            {
                //It clearly means that when we select items as cells from the dataProductsList
                foreach (DataGridViewCell cell in cells)
                {
                    Int64 code;
                    try
                    {
                        code = Convert.ToInt64(cell.Value.ToString());
                    }
                    catch (Exception ex)
                    {
                        MessageBox.Show(ex.Message);
                        return;
                    }
                    string name = productsTableAdapter.GetProductName(code);
                    double amount = Convert.ToDouble(productsTableAdapter.GetProductPrice(code));

                    //now you need to run an algorithm that checks if this item exists in the dataSalesList. If it does, then it should
                    //      be updated....If it isn't, then a new entry shall be made.

                    foreach (DataGridViewRow row in dataSalesList.Rows)
                    {
                        if (row.Cells[0].Value.ToString() == code.ToString())
                        {
                            row.Cells[2].Value = (Convert.ToInt16(row.Cells[2].Value.ToString()) + 1);
                            row.Cells[3].Value = (Convert.ToDouble(row.Cells[3].Value.ToString()) + amount);
                            AddCost(amount);
                            return;
                        }
                    }

                    //if the program get's here, it means that this item does not exist in our new transaction list. Add it
                    dataSalesList.Rows.Add(code, name, 1, amount);
                    AddCost(amount);
                    return;
                }
            }
            else
            {
                MessageBox.Show("No Rows Have Been Selected");
                return;
            }
        }

        private void btnChange_Click(object sender, EventArgs e)
        {
            Change();
        }

        private void btnRemoveItem_Click(object sender, EventArgs e)
        {
            DataGridViewSelectedRowCollection rows = dataSalesList.SelectedRows;
            if (rows.Count > 0)
            {
                foreach (DataGridViewRow row in rows)
                {
                    double amount = Convert.ToDouble(row.Cells[3].Value.ToString()) / Convert.ToInt16(row.Cells[2].Value.ToString());
                    SubtractCost(amount);
                    dataSalesList.Rows.RemoveAt(row.Index);
                    dataSalesList.Refresh();
                }
            }
            else
            {
                MessageBox.Show("No Rows Have Been Selected");
            }
        }

        private void btnReset_Click(object sender, EventArgs e)
        {
            dataSalesList.Rows.Clear();
            dataSalesList.Refresh();
            txtAmountRecieved.Text = "0";
            txtChange.Text = "0";
            txtSubtotal.Text = "0";
            txtTax.Text = "0";
            txtTotal.Text = "0";
            cmbMethodOfPayment.Text = "CASH";
        }

        private void txtEmployeeSearch_TextChanged(object sender, EventArgs e)
        {

        }

        private void txtEmployeeSearch_TextChanged_1(object sender, EventArgs e)
        {
            try
            {
                this.employeeTableAdapter.FillByEmployeeName(this.group28DataSet.Employee, txtEmployeeSearch.Text);
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }

        private void textSearch_TextChanged(object sender, EventArgs e)
        {
            try
            {
                this.productsTableAdapter.FillBySearch(this.group28DataSet.Products, textSearch.Text);
            }
            catch
            {
                if (MessageBox.Show("Internet Connection is Slow, Do you want to retry?", "Message", MessageBoxButtons.YesNo, MessageBoxIcon.Question) == DialogResult.Yes)
                {
                    textSearch_TextChanged(sender, e);
                }
                else
                    return;
            }
        }

        private void button13_Click(object sender, EventArgs e)
        {
            LogIn f = new LogIn();
            this.Hide();
            f.Show();
        }

        private void btnInsert_Click(object sender, EventArgs e)
        {
            try
            {
                this.productsBindingSource1.AddNew();
            }catch(Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }

        private void btnSave_Click(object sender, EventArgs e)
        {
            if (txtBarcode.Text == "" || txtProductName.Text == "" || txtQuantity.Text == "" || txtPrice.Text == "" || txtDescription.Text == "" || cmbCategory.Text == "")
            {
                MessageBox.Show("Please fill in missing values");
                return;
            }
            else
            {
                Int64 barcode;
                string prodName;
                int quantity;
                decimal price;
                string description;
                string category;
                try
                {
                    barcode = Convert.ToInt64(txtBarcode.Text);
                    prodName = txtProductName.Text;
                    quantity = Convert.ToInt16(txtQuantity.Text);
                    price = Convert.ToDecimal(txtPrice.Text);
                    description = txtDescription.Text;
                    category = cmbCategory.Text;
                }
                catch (Exception ex)
                {
                    MessageBox.Show(ex.Message);
                    return;
                }
                int count;
                try
                {
                    count = Convert.ToInt16(this.productsTableAdapter.doesBarcodeExist(barcode));
                }
                catch
                {
                    MessageBox.Show("Invalid Barcode");
                    return;
                }
                if (count > 0) MessageBox.Show("This barcode already exists, please enter a new one");
                else
                {
                    if(price < 1 || quantity < 1)
                    {
                        MessageBox.Show("Price and Quantity cannot be zero or negative");
                        return;
                    }
                    this.productsBindingSource1.EndEdit();
                    this.productsTableAdapter.Insert(barcode, prodName, quantity, price, description, category);
                    this.productsTableAdapter.Fill(group28DataSet.Products);
                }
            }
        }

        private void btnDelete_Click(object sender, EventArgs e)
        {
            if (MessageBox.Show("Are you sure you want to delete this product?", "Message", MessageBoxButtons.YesNo, MessageBoxIcon.Question) == DialogResult.Yes)
            {
                try
                {
                    Int64 barcode = Convert.ToInt64(txtBarcode.Text);
                    string productname = txtProductName.Text;
                    int prodQuantity = Convert.ToInt16(txtQuantity.Text);
                    decimal prodPrice = Convert.ToDecimal(txtPrice.Text);
                    string description = txtDescription.Text;
                    string category = cmbCategory.Text;
                    this.productsTableAdapter.Delete(barcode, productname, prodQuantity, prodPrice, description, category);
                    this.productsTableAdapter.Fill(this.group28DataSet.Products);
                }
                catch (Exception ex)
                {
                    MessageBox.Show(ex.Message);
                }
            }
        }

        private void btnUpdate_Click(object sender, EventArgs e)
        {
            Int64 barcode;
            string prodName;
            int quantity;
            decimal price;
            string description;
            string category;
            try
            {
                barcode = Convert.ToInt64(txtBarcode.Text);
                prodName = txtProductName.Text;
                quantity = Convert.ToInt16(txtQuantity.Text);
                price = Convert.ToDecimal(txtPrice.Text);
                description = txtDescription.Text;
                category = cmbCategory.Text;
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
                return;
            }
            try
            {
                this.productsTableAdapter.UpdateQuery(barcode, prodName, quantity, price, description, category, barcode);
                this.productsTableAdapter.Fill(this.group28DataSet.Products);
            }
            catch (Exception ex) { MessageBox.Show(ex.Message); }
        }

        private void btnReload_Click(object sender, EventArgs e)
        {
            try
            {
                this.productsTableAdapter.Fill(this.group28DataSet.Products);
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }

        private void button18_Click(object sender, EventArgs e)
        {
            try
            {
                this.employeeBindingSource.AddNew();
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }

        private void button15_Click(object sender, EventArgs e)
        {
            if (txtCode.Text == "" || txtName.Text == "" || txtEmail.Text == "" || txtPhone.Text == "" || txtPassword.Text == "")
            {
                MessageBox.Show("Fill in missing information", "Missing Data", MessageBoxButtons.OK, MessageBoxIcon.Information);
                return;
            }
            else
            {
                Int64 phone;
                try
                {
                    phone = Convert.ToInt64(txtPhone.Text);
                }
                catch (Exception ex)
                {
                    MessageBox.Show(ex.Message);
                    return;
                }
                if (txtPhone.Text.Length < 10 || txtPhone.Text.Length > 10)
                {
                    MessageBox.Show("Phone number is invalid");
                    return;
                }
                string name = txtName.Text;
                string address = txtAddress.Text;
                string email = txtEmail.Text;
                string password = txtPassword.Text;

                int count = Convert.ToInt16(this.employeeTableAdapter.doesEmailExist(email).ToString());
                if (count > 0 || count < 0)
                {
                    MessageBox.Show("This email already exists", "Email error", MessageBoxButtons.OKCancel, MessageBoxIcon.Warning);
                }
                else
                {
                    try
                    {
                        this.employeeTableAdapter.InsertQuery(name, phone, email, address, password);
                        this.employeeTableAdapter.Fill(this.group28DataSet.Employee);
                    }
                    catch (Exception ex)
                    {
                        MessageBox.Show(ex.Message);
                    }
                }

            }
        }

        private void button16_Click(object sender, EventArgs e)
        {
            foreach (DataGridViewRow row in dataEmployee.SelectedRows)
            {
                if (MessageBox.Show("Are you sure you want to delete this record?", "Message", MessageBoxButtons.YesNo, MessageBoxIcon.Question) == DialogResult.Yes)
                {
                    try
                    {
                        dataEmployee.Rows.RemoveAt(row.Index);
                    }
                    catch (Exception ex)
                    {
                        MessageBox.Show(ex.Message);
                    }
                }
            }
        }

        private void button17_Click(object sender, EventArgs e)
        {
            try
            {
                this.employeeBindingSource.EndEdit();
                this.employeeTableAdapter.Update(this.group28DataSet.Employee);
                this.employeeTableAdapter.Fill(this.group28DataSet.Employee);
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }

        private void button14_Click(object sender, EventArgs e)
        {
            try
            {
                this.employeeTableAdapter.Fill(this.group28DataSet.Employee);
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }

        private void button19_Click(object sender, EventArgs e)
        {
            LogIn f = new LogIn();
            this.Hide();
            f.Show();
        }

        private void btnPurchase_Click(object sender, EventArgs e)
        {
            if (dataSalesList.Rows.Count > 0)
            {
                int error = 0;
                foreach (DataGridViewRow row in dataSalesList.Rows)
                {
                    Int64 Barcode = Convert.ToInt64(row.Cells[0].Value.ToString());
                    int originalQTY = Convert.ToInt16(this.productsTableAdapter.GetQuantity(Barcode).Value.ToString());
                    string name = row.Cells[1].Value.ToString();
                    int saleQTY = Convert.ToInt16(row.Cells[2].Value.ToString());
                    if (originalQTY == 0)
                    {
                        MessageBox.Show(name + " is not avaialble, do you wish to continue buy other items?");
                        error = 1;
                        return;
                    }
                    if (originalQTY < saleQTY)
                    {
                        MessageBox.Show("The quantity of " + name + " that you want to buy is not available");
                        error = 1;
                        return;
                    }
                }
                if (error == 0)
                {
                    StreamWriter sw = new StreamWriter("F:\\Updated Group M2\\UpdatedM2Application\\UpdatedM2Application\\Sales.txt");
                    foreach (DataGridViewRow row in dataSalesList.Rows)
                    {
                        Int64 Barcode = Convert.ToInt64(row.Cells[0].Value.ToString());
                        int originalQTY = Convert.ToInt16(this.productsTableAdapter.GetQuantity(Barcode).Value.ToString());
                        string name = row.Cells[1].Value.ToString();
                        int saleQTY = Convert.ToInt16(row.Cells[2].Value.ToString());
                        if (originalQTY == 0)
                        {
                            MessageBox.Show(name + " is not avaialble, do you wish to continue buy other items?");
                            error = 1;
                            return;
                        }
                        if (originalQTY < saleQTY)
                        {
                            MessageBox.Show("The quantity of " + name + " that you want to buy is not available");
                            error = 1;
                            return;
                        }
                        DateTime currentDate = DateTime.Now;
                        double price = Convert.ToDouble(row.Cells[3].Value.ToString());
                        double amountReceived = Convert.ToDouble(txtAmountRecieved.Text);
                        double change = Convert.ToDouble(txtChange.Text);
                        try
                        {
                            //ordersTableAdapter1.InsertQuery(Cust_ID, currentDate, saleQTY, price, amountReceived, change);

                            string s = Barcode + "," + name + "," + currentDate + "," + price + "," + amountReceived + "," + change;
                            sw.WriteLine(s);


                        }
                        catch (Exception ex)
                        {
                            MessageBox.Show(ex.Message);
                            return;
                        }
                    }
                    MessageBox.Show("Your transaction is successful");
                    dataSalesList.Rows.Clear();
                    dataSalesList.Refresh();
                    sw.Close();
                    txtAmountRecieved.Text = "0";
                    txtChange.Text = "0";
                    cmbCategory.Text = "CASH";
                    txtSubtotal.Text = "0";
                    txtTax.Text = "0";
                    txtTotal.Text = "0";
                }
            }
            else
            {
                MessageBox.Show("There are no selected products");
                return;
            }
        }
    }
}
