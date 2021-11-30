using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;


using System.Configuration;
using System.Data;
using System.Data.SqlClient;

namespace currency_converter
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        SqlConnection con = new SqlConnection();
        SqlCommand cmd = new SqlCommand();
        SqlDataAdapter da = new SqlDataAdapter();

        //field
        private int CurrencyId = 0;
        private double FromAmount = 0;
        private double ToAmount = 0;


        public MainWindow()
        {
            InitializeComponent();
            BindCurrencyTable();
            GetData();
            ClearControl();
        }

        //connection method
        public void mycon()
        {
            String Con = ConfigurationManager.ConnectionStrings["CurrencyConnectionString"].ConnectionString;
            con = new SqlConnection(Con);
            con.Open();
        }

        //bind method- datatable
        private void BindCurrencyTable()
        {
            mycon();
            DataTable dt = new DataTable();
            //spec command
            cmd = new SqlCommand("SELECT Id, Amount, CurrencyName FROM CurrencyTable", con);
            cmd.CommandType = CommandType.Text;
            da = new SqlDataAdapter(cmd);
            da.Fill(dt);
            //create row
            DataRow newRow = dt.NewRow();
            newRow["Id"] = 0;
            newRow["CurrencyName"] = "--SELECTED--";
            dt.Rows.InsertAt(newRow, 0);
            if (dt != null && dt.Rows.Count > 0)
            {
                cmbFromCurrency.ItemsSource = dt.DefaultView;
                cmbToCurrency.ItemsSource = dt.DefaultView;
            }
            con.Close();

            cmbFromCurrency.DisplayMemberPath = "CurrencyName";
            cmbFromCurrency.SelectedValuePath = "Id";
            cmbFromCurrency.SelectedValue = 0;

            cmbToCurrency.DisplayMemberPath = "CurrencyName";
            cmbToCurrency.SelectedValuePath = "Id";
            cmbToCurrency.SelectedValue = 0;
        }

        //convert button
        private void btnConvert_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                double ConvertedValue;

                if (txtCurrencyAmount.Text == null || txtCurrencyAmount.Text.Trim() == "")
                {
                    MessageBox.Show("Please Enter Currency", "Information", MessageBoxButton.OK, MessageBoxImage.Information);
                    txtCurrencyAmount.Focus();
                    return;
                }
                else if (cmbFromCurrency.SelectedValue == null || cmbFromCurrency.SelectedIndex == 0)
                {
                    MessageBox.Show("Please Select Currency From", "Information", MessageBoxButton.OK, MessageBoxImage.Information);
                    cmbFromCurrency.Focus();
                    return;
                }

                else if (cmbToCurrency.SelectedValue == null || cmbToCurrency.SelectedIndex == 0)
                {
                    MessageBox.Show("Please Select Currency To", "Information", MessageBoxButton.OK, MessageBoxImage.Information);
                    cmbToCurrency.Focus();
                    return;
                }

                if (cmbFromCurrency.SelectedValue == cmbToCurrency.SelectedValue)
                {
                    ConvertedValue = double.Parse(txtCurrencyAmount.Text);
                    lblConvertedCurrency.Content = cmbToCurrency.Text + ConvertedValue.ToString("N3");
                }
                else
                {
                    if (FromAmount != null && FromAmount != 0 && ToAmount != null && ToAmount != 0)
                    {
                        ConvertedValue = FromAmount * double.Parse(txtCurrencyAmount.Text) / ToAmount;
                        lblConvertedCurrency.Content = cmbToCurrency.Text + ConvertedValue.ToString("N3");
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        
        }

        //clear button
        private void btnClear_Click(object sender, RoutedEventArgs e)
        {
            ClearControl();
        }
        //method clearcontorl
        private void ClearControl()
        {
            try
            {
                txtCurrencyAmount.Text = string.Empty;
                if (cmbFromCurrency.Items.Count > 0)
                    cmbFromCurrency.SelectedIndex = 0;
                if (cmbToCurrency.Items.Count > 0)
                    cmbToCurrency.SelectedIndex = 0;
                lblConvertedCurrency.Content = "Find out up to date market rates!";
                txtCurrencyAmount.Focus();
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            };
        }
        //numbervalidation
        private void NumberValidationTextBox(object sender, TextCompositionEventArgs e)
        {
            Regex regex = new Regex("[^0-9]+,");
            e.Handled = regex.IsMatch(e.Text);
        }
        //reverse button
        private void btnReverse_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                if (cmbFromCurrency.SelectedValue == null || cmbFromCurrency.SelectedIndex == 0)
                {
                    //tada pokazi poruku
                    MessageBox.Show("Please Select Currency From", "Information", MessageBoxButton.OK, MessageBoxImage.Information);

                    //postavi fokus na From
                    cmbFromCurrency.Focus();
                    return;
                }
                else if (cmbToCurrency.SelectedValue == null || cmbToCurrency.SelectedIndex == 0)
                {
                    //tada pokazi poruku
                    MessageBox.Show("Please Select Currency To", "Information", MessageBoxButton.OK, MessageBoxImage.Information);

                    //i postavi fokus na To
                    cmbToCurrency.Focus();
                    return;
                }

                else if (cmbFromCurrency.SelectedValue != null && cmbToCurrency.SelectedValue != null)
                {
                    string tempCurrency = cmbFromCurrency.Text;
                    cmbFromCurrency.Text = cmbToCurrency.Text;
                    cmbToCurrency.Text = tempCurrency;
                }

            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "Please pick Currency", MessageBoxButton.OK);
            }
        }
        //save button
        private void btnSave_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                if (txtAmount.Text == null || txtAmount.Text.Trim() == "")
                {
                    MessageBox.Show("Please enter amount", "Information", MessageBoxButton.OK, MessageBoxImage.Information);
                    txtAmount.Focus();
                    return;
                }
                else if (txtCurrencyName.Text == null || txtCurrencyName.Text.Trim() == "")
                {
                    MessageBox.Show("Please enter currency name", "Information", MessageBoxButton.OK, MessageBoxImage.Information);
                    txtCurrencyName.Focus();
                    return;
                }
                else
                {
                    bool isUpdate = CurrencyId > 0;
                    if (MessageBox.Show($"Are you sure you want to " + (isUpdate ? "Update" : "Save") + " ?", "Information", MessageBoxButton.YesNo, MessageBoxImage.Question) == MessageBoxResult.Yes)
                    {
                        mycon();

                        SqlCommand cmd;
                        if (isUpdate)
                            cmd = new SqlCommand("UPDATE CurrencyTable SET Amount = @Amount, CurrencyName = @CurrencyName WHERE Id = @Id", con);
                        else
                            cmd = new SqlCommand("INSERT INTO CurrencyTable (Amount, CurrencyName) VALUES(@Amount, @CurrencyName)", con);

                        cmd.CommandType = CommandType.Text;
                        cmd.Parameters.AddWithValue("@Id", CurrencyId);
                        cmd.Parameters.AddWithValue("@Amount", txtAmount.Text);
                        cmd.Parameters.AddWithValue("@CurrencyName", txtCurrencyName.Text);
                        cmd.ExecuteNonQuery();
                        con.Close();
                        MessageBox.Show("Data " + (isUpdate ? "updated" : "saved") + " successfully", "Information", MessageBoxButton.OK, MessageBoxImage.Information);
                    }
                    ClearMaster();
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }
        private void ClearMaster()
        {
            try
            {
                txtAmount.Text = string.Empty;
                txtCurrencyName.Text = string.Empty;
                btnSave.Content = "Save";
                GetData();
                CurrencyId = 0;
                BindCurrencyTable();
                txtAmount.Focus();
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }

        }

        private void GetData()
        {
            mycon();
            DataTable dt = new DataTable();
            cmd = new SqlCommand("Select * from CurrencyTable", con);
            cmd.CommandType = CommandType.Text;
            da = new SqlDataAdapter(cmd);
            da.Fill(dt);
            if (dt != null && dt.Rows.Count > 0)
            {
                dgvCurrency.ItemsSource = dt.DefaultView;
            }
            else
            {
                dgvCurrency.ItemsSource = null;
            }
            con.Close();

        }
        //cancle button
        private void btnCancel_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                ClearMaster();
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }
        //From currency Combobox selection changed event to get the amount of currency on selection change of currency name
        private void cmbFromCurrency_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            try
            {
                if (cmbFromCurrency.SelectedValue != null && int.Parse(cmbFromCurrency.SelectedValue.ToString()) != 0 && cmbFromCurrency.SelectedIndex != 0)
                {
                    int CurrencyFromId = int.Parse(cmbFromCurrency.SelectedValue.ToString());
                    mycon();
                    DataTable dt = new DataTable();
                    cmd = new SqlCommand("SELECT Amount FROM CurrencyTable WHERE Id = @CurrencyFromId", con);
                    cmd.CommandType = CommandType.Text;

                    if (CurrencyFromId != null && CurrencyFromId != 0)
                        cmd.Parameters.AddWithValue("@CurrencyFromId", CurrencyFromId);
                    da = new SqlDataAdapter(cmd);
                    da.Fill(dt);
                    if (dt != null && dt.Rows.Count > 0)
                        FromAmount = double.Parse(dt.Rows[0]["Amount"].ToString());
                    con.Close();
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }


        //To currency Combobox selection changed event to get the amount of currency on selection change of currency name
        private void cmbToCurrency_SelectionChanged(object sender, SelectionChangedEventArgs e)

        {
            try
            {
                if (cmbToCurrency.SelectedValue != null && int.Parse(cmbToCurrency.SelectedValue.ToString()) != 0 && cmbToCurrency.SelectedIndex != 0)
                {
                    //get value frim comboto and sign in currencytoid
                    int CurrencyToId = int.Parse(cmbToCurrency.SelectedValue.ToString());

                    mycon();

                    DataTable dt = new DataTable();
                    cmd = new SqlCommand("SELECT Amount FROM CurrencyTable WHERE Id = @CurrencyToId", con);
                    cmd.CommandType = CommandType.Text;
                    if (CurrencyToId != null && CurrencyToId != 0)
                        cmd.Parameters.AddWithValue("@CurrencyToId", CurrencyToId);
                    da = new SqlDataAdapter(cmd);
                    da.Fill(dt);

                    if (dt != null && dt.Rows.Count > 0)
                        ToAmount = double.Parse(dt.Rows[0]["Amount"].ToString());

                    con.Close();
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }
        //dvg select button 
        private void dgvCurrency_SelectedCellsChanged(object sender, SelectedCellsChangedEventArgs e)
        {
            try
            {
                DataGrid grd = (DataGrid)sender;
                DataRowView row_selected = grd.CurrentItem as DataRowView;

                if (row_selected != null)
                {
                    if (dgvCurrency.Items.Count > 0)
                    {
                        if (grd.SelectedCells.Count > 0)
                        {
                            //get id from selected row
                            CurrencyId = Int32.Parse(row_selected["Id"].ToString());

                            //if id = 0
                            if (grd.SelectedCells[0].Column.DisplayIndex == 0)
                            {
                                txtAmount.Text = row_selected["Amount"].ToString();
                                txtCurrencyName.Text = row_selected["CurrencyName"].ToString();
                                btnSave.Content = "Update";
                            }

                            //if id = 1
                            if (grd.SelectedCells[0].Column.DisplayIndex == 1)
                            {
                                if (MessageBox.Show("Are you sure you want to delete ?", "Information", MessageBoxButton.YesNo, MessageBoxImage.Question) == MessageBoxResult.Yes)
                                {
                                    mycon();
                                    DataTable dt = new DataTable();

                                    cmd = new SqlCommand("DELETE FROM CurrencyTable WHERE Id = @Id", con);
                                    cmd.CommandType = CommandType.Text;

                                    cmd.Parameters.AddWithValue("@Id", CurrencyId);
                                    cmd.ExecuteNonQuery();
                                    con.Close();

                                    MessageBox.Show("Data deleted successfully", "Information", MessageBoxButton.OK, MessageBoxImage.Information);

                                    ClearMaster();
                                }
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }
    }
}
