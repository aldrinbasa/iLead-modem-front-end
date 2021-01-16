using System;
using System.Windows.Forms;
using MySql.Data.MySqlClient;
using System.Data;

namespace RichwayAllNetLoad {
    public partial class RetailersDetailsForm : Form {

        #region Constructors
        private readonly MainForm mainForm;

        private string phoneNumber;
        private string MySQLConnectionString = "datasource=127.0.0.1;port=3306;username=root;password=;database=allload";

        private bool editing = false;

        private int pinFowardPageCounter = 1;
        private int activatedRetailerPageCounter = 1;

        private DateTime currentDate = DateTime.Today;

        private double percentage = 0;
        private double totalIncome = 0;

        public RetailersDetailsForm(string phoneNumber, MainForm mainForm) {
            InitializeComponent();

            this.phoneNumber = phoneNumber;
            this.mainForm = mainForm;
        }
        #endregion

        #region Initializations
        private void RetailersDetailsForm_Load(object sender, EventArgs e) {

            this.ActiveControl = mainLabelName;
            mainTextBoxCPNumber.Text = this.phoneNumber;

            this.LoadDetails();
            this.mainButtonTLC_Click(null, null);

            //TLC
            pageTransactionHistory_DataGridView.DataSource = GetTransactionHistory();
            pageTransactionHistory_DataGridView.Columns[0].AutoSizeMode = DataGridViewAutoSizeColumnMode.AllCells;
            pageTransactionHistory_DataGridView.Columns[1].AutoSizeMode = DataGridViewAutoSizeColumnMode.AllCells;
            pageTransactionHistory_DataGridView.Columns[2].AutoSizeMode = DataGridViewAutoSizeColumnMode.AllCells;
            pageTransactionHistory_DataGridView.Columns[3].AutoSizeMode = DataGridViewAutoSizeColumnMode.AllCells;
            pageTransactionHistory_DataGridView.Columns[4].AutoSizeMode = DataGridViewAutoSizeColumnMode.AllCells;

            showPercentageValuesTLC();

            setCurrentDateTexts();

            GetIncome();

            //Pin Forward Initializations
            pagePinFoward_ComboBoxShowEntries.SelectedIndex = 0;
            pagePinFoward_LabelPageNumber.Text = "Page " + pinFowardPageCounter;

            pagePinFoward_DataGridView.DataSource = GetPinForwardHistory(pinFowardPageCounter, int.Parse(pagePinFoward_ComboBoxShowEntries.Text));

            pagePinFoward_DataGridView.Columns[0].AutoSizeMode = DataGridViewAutoSizeColumnMode.AllCells;
            pagePinFoward_DataGridView.Columns[0].HeaderText = "NUMBER OF PINS";
            pagePinFoward_DataGridView.Columns[1].HeaderText = "RECEIVER NUMBER";
            pagePinFoward_DataGridView.Columns[2].HeaderText = "RECEIVER NAME";
            pagePinFoward_DataGridView.Columns[3].HeaderText = "REMAINING PINS";
            pagePinFoward_DataGridView.Columns[4].HeaderText = "TXN #";
            pagePinFoward_DataGridView.Columns[5].HeaderText = "DATE & TIME";

            //Activated Retailers Initializations
            pageActivatedRetailer_ComboBoxShowEntries.SelectedIndex = 0;
            pageActivatedRetailer_LabelPageNumber.Text = "Page " + activatedRetailerPageCounter;

            pageActivatedRetailer_DataGridView.DataSource = GetActivatedRetailers(activatedRetailerPageCounter, int.Parse(pageActivatedRetailer_ComboBoxShowEntries.Text));
        }

        public void LoadDetails() {

            string searchValue = this.phoneNumber;

            string query = "SELECT tbl_account._PHONE, tbl_users._PASSWORD, tbl_users._BDATE, tbl_users._ADDRESS, tbl_users._FULLNAME FROM tbl_account INNER JOIN tbl_users ON tbl_account._PHONE=tbl_users._PHONE WHERE tbl_users._PHONE = " + searchValue;

            MySqlConnection databaseConnection = new MySqlConnection(this.MySQLConnectionString);
            MySqlCommand commandDatabase = new MySqlCommand(query, databaseConnection);
            databaseConnection.Open();

            MySqlDataReader myReader = commandDatabase.ExecuteReader();

            try {
                myReader.Read();
                mainTextBoxName.Text = myReader["_FULLNAME"].ToString();
                mainTextBoxCPNumber.Text = myReader["_PHONE"].ToString();
                mainTextBoxPassword.Text = myReader["_PASSWORD"].ToString();
                mainTextBoxBirthday.Text = myReader["_BDATE"].ToString();
                mainTextBoxAddress.Text = myReader["_ADDRESS"].ToString();
            }
            catch (Exception e) {
                MessageBox.Show(e.Message);
            }

            databaseConnection.Close();
        }
        #endregion

        #region Tab Controllers
        private void mainButtonPinForward_Click(object sender, EventArgs e) {
            mainTabControl.SelectedTab = mainTabControlPinForwardTab;
            mainLabelTransactionHistory.Text = "PIN FORWARDS";
            this.Text = "Pin Forward";
        }

        private void MainButtonActivatedRetailer_Click(object sender, EventArgs e) {
            mainTabControl.SelectedTab = mainTabControlActivatedRetailerTab;
            mainLabelTransactionHistory.Text = "ACTIVATED RETAILERS";
            this.Text = "Activated Retailers";
        }

        private void mainButtonTLC_Click(object sender, EventArgs e) {
            mainTabControl.SelectedTab = mainTabControlTLCTab;
            mainLabelTransactionHistory.Text = "TLC";
            this.Text = "TLC";
        }
        #endregion

        #region Main Form Functions
        private void mainButtonEditSave_Click(object sender, EventArgs e) {
            editing = !editing;

            if (editing) {
                mainTextBoxName.ReadOnly = false;
                mainTextBoxCPNumber.ReadOnly = false;
                mainTextBoxAddress.ReadOnly = false;
                mainTextBoxBirthday.ReadOnly = false;
                mainTextBoxPassword.ReadOnly = false;

                mainTextBoxName.BorderStyle = BorderStyle.FixedSingle;
                mainTextBoxCPNumber.BorderStyle = BorderStyle.FixedSingle;
                mainTextBoxAddress.BorderStyle = BorderStyle.FixedSingle;
                mainTextBoxBirthday.BorderStyle = BorderStyle.FixedSingle;
                mainTextBoxPassword.BorderStyle = BorderStyle.FixedSingle;

                mainButtonEditSave.Text = "Save";
            }
            else {
                mainTextBoxName.ReadOnly = true;
                mainTextBoxCPNumber.ReadOnly = true;
                mainTextBoxAddress.ReadOnly = true;
                mainTextBoxBirthday.ReadOnly = true;
                mainTextBoxPassword.ReadOnly = true;

                mainTextBoxName.BorderStyle = BorderStyle.None;
                mainTextBoxCPNumber.BorderStyle = BorderStyle.None;
                mainTextBoxAddress.BorderStyle = BorderStyle.None;
                mainTextBoxBirthday.BorderStyle = BorderStyle.None;
                mainTextBoxPassword.BorderStyle = BorderStyle.None;

                mainButtonEditSave.Text = "Edit";

                try {
                    string query = "UPDATE tbl_users SET _FULLNAME = @fullname, _PHONE = @phone, _ADDRESS = @address, _BDATE = @birthday, _PASSWORD = @password WHERE _PHONE = @phone";
                    MySqlConnection databaseConnection = new MySqlConnection(this.MySQLConnectionString);
                    MySqlCommand commandDatabase = new MySqlCommand(query, databaseConnection);

                    commandDatabase.Parameters.AddWithValue("@fullname", mainTextBoxName.Text);
                    commandDatabase.Parameters.AddWithValue("@phone", mainTextBoxCPNumber.Text);
                    commandDatabase.Parameters.AddWithValue("@address", mainTextBoxAddress.Text);
                    commandDatabase.Parameters.AddWithValue("@birthday", mainTextBoxBirthday.Text);
                    commandDatabase.Parameters.AddWithValue("@password", mainTextBoxPassword.Text);

                    databaseConnection.Open();

                    commandDatabase.ExecuteNonQuery();

                    databaseConnection.Close();

                    this.LoadDetails();
                    this.mainForm.reloadData();
                }
                catch (Exception error) {
                    MessageBox.Show(error.Message);
                }
            }
        }
        #endregion

        #region Page - Pin Forward
        private DataTable GetPinForwardHistory(int pageNumber, int rowCount) {

            DataTable dataPinForwardHistory = new DataTable();
            DataSet dataSetPinForwardHistory = new DataSet();

            string query = "SELECT tbl_pin._PIN_AMT, tbl_pin._RECEIVER, tbl_users._FULLNAME, tbl_pin._PINS, tbl_pin._REFNO, tbl_pin._DATETIME FROM tbl_pin INNER JOIN tbl_users ON tbl_users._PHONE=tbl_pin._RECEIVER WHERE tbl_pin._SENDER = " + phoneNumber;

            MySqlConnection databaseConnection = new MySqlConnection(this.MySQLConnectionString);
            MySqlDataAdapter adapterMembersLoadFund = new MySqlDataAdapter(query, databaseConnection);


            try {
                databaseConnection.Open();
                adapterMembersLoadFund.Fill(dataSetPinForwardHistory, (pageNumber - 1) * rowCount, rowCount, "Table");
            }
            catch {

            }

            databaseConnection.Close();

            dataPinForwardHistory = dataSetPinForwardHistory.Tables[0];

            return dataPinForwardHistory;
        }

        private void pagePinFoward_ComboBoxShowEntries_SelectedIndexChanged(object sender, EventArgs e) {
            pagePinFoward_DataGridView.DataSource = GetPinForwardHistory(pinFowardPageCounter, int.Parse(pagePinFoward_ComboBoxShowEntries.Text));
        }

        private void pagePinFoward_ButtonNext_Click(object sender, EventArgs e) {
            if (pagePinFoward_DataGridView.Rows.Count == int.Parse(pagePinFoward_ComboBoxShowEntries.Text)) {
                pinFowardPageCounter = pinFowardPageCounter + 1;

                pagePinFoward_DataGridView.DataSource = GetPinForwardHistory(pinFowardPageCounter, int.Parse(pagePinFoward_ComboBoxShowEntries.Text));
            }

            pagePinFoward_LabelPageNumber.Text = "Page " + pinFowardPageCounter;
        }

        private void pagePinFoward_ButtonPrevious_Click(object sender, EventArgs e) {
            if (pinFowardPageCounter > 1) {
                pinFowardPageCounter = pinFowardPageCounter - 1;

                pagePinFoward_DataGridView.DataSource = GetPinForwardHistory(pinFowardPageCounter, int.Parse(pagePinFoward_ComboBoxShowEntries.Text));
            }

            pagePinFoward_LabelPageNumber.Text = "Page " + pinFowardPageCounter;
        }

        private void pagePinFoward_TextBoxSearch_TextChanged(object sender, EventArgs e) {

            string searchValue = pagePinFoward_TextBoxSearch.Text;
            int rowIndex = -1;

            try {
                foreach (DataGridViewRow row in pagePinFoward_DataGridView.Rows) {
                    if (row.Cells["_REFNO"].Value.ToString().StartsWith(searchValue)) {
                        rowIndex = row.Index;
                        pagePinFoward_DataGridView.CurrentCell = pagePinFoward_DataGridView.Rows[rowIndex].Cells[0];
                        pagePinFoward_DataGridView.Rows[pagePinFoward_DataGridView.CurrentCell.RowIndex].Selected = true;

                        break;
                    }
                }
            }
            catch (Exception exc) {
                MessageBox.Show(exc.Message);
            }
        }
        #endregion

        #region Page - Activater Retailers
        private DataTable GetActivatedRetailers(int pageNumber, int rowCount) {

            DataTable dataActivatedRetailers = new DataTable();
            DataSet dataSetActivatedRetailers = new DataSet();

            string query = "SELECT _NAME, _PHONE, _USERNAME, _PINS, _REFNO FROM tbl_act_ret WHERE _ACTIVATOR_NUMBER = " + phoneNumber;

            MySqlConnection databaseConnection = new MySqlConnection(this.MySQLConnectionString);
            MySqlDataAdapter adapterMembersLoadFund = new MySqlDataAdapter(query, databaseConnection);


            try {
                databaseConnection.Open();
                adapterMembersLoadFund.Fill(dataSetActivatedRetailers, (pageNumber - 1) * rowCount, rowCount, "Table");
            }
            catch {

            }

            databaseConnection.Close();

            dataActivatedRetailers = dataSetActivatedRetailers.Tables[0];

            return dataActivatedRetailers;
        }

        private void pageActivatedRetailer_ComboBoxShowEntries_SelectedIndexChanged(object sender, EventArgs e) {
            pageActivatedRetailer_DataGridView.DataSource = GetActivatedRetailers(activatedRetailerPageCounter, int.Parse(pageActivatedRetailer_ComboBoxShowEntries.Text));
        }

        private void pageActivatedRetailer_ButtonNext_Click(object sender, EventArgs e) {
            if (pageActivatedRetailer_DataGridView.Rows.Count == int.Parse(pageActivatedRetailer_ComboBoxShowEntries.Text)) {
                activatedRetailerPageCounter = activatedRetailerPageCounter + 1;

                pageActivatedRetailer_DataGridView.DataSource = GetActivatedRetailers(activatedRetailerPageCounter, int.Parse(pageActivatedRetailer_ComboBoxShowEntries.Text));
            }

            pageActivatedRetailer_LabelPageNumber.Text = "Page " + activatedRetailerPageCounter;
        }

        private void pageActivatedRetailer_ButtonPrevious_Click(object sender, EventArgs e) {
            if (activatedRetailerPageCounter > 1) {
                activatedRetailerPageCounter = activatedRetailerPageCounter - 1;

                pageActivatedRetailer_DataGridView.DataSource = GetActivatedRetailers(activatedRetailerPageCounter, int.Parse(pageActivatedRetailer_ComboBoxShowEntries.Text));
            }

            pageActivatedRetailer_LabelPageNumber.Text = "Page " + activatedRetailerPageCounter;
        }

        private void pageActivatedRetailer_TextBoxSearch_TextChanged(object sender, EventArgs e) {
            string searchValue = pageActivatedRetailer_TextBoxSearch.Text;
            int rowIndex = -1;

            try {
                foreach (DataGridViewRow row in pageActivatedRetailer_DataGridView.Rows) {
                    if (row.Cells["_REFNO"].Value.ToString().StartsWith(searchValue)) {
                        rowIndex = row.Index;
                        pageActivatedRetailer_DataGridView.CurrentCell = pageActivatedRetailer_DataGridView.Rows[rowIndex].Cells[0];
                        pageActivatedRetailer_DataGridView.Rows[pageActivatedRetailer_DataGridView.CurrentCell.RowIndex].Selected = true;

                        break;
                    }
                }
            }
            catch (Exception exc) {
                MessageBox.Show(exc.Message);
            }
        }
        #endregion

        #region Page - TLC
        private DataTable GetTransactionHistory() {

            DataTable dataMessages = new DataTable();

            string query = "SELECT _SENDER, _SENDER_BAL, _AMOUNT, _RECEIVER, _RECEIVER_BAL, _REFNO, _DATETIME FROM tbl_tlc WHERE _DATETIME LIKE '" + currentDate.ToString("yyyy-MM-dd") + "%' AND _SENDER = '" + this.phoneNumber.ToString() + "'";

            MySqlConnection databaseConnection = new MySqlConnection(this.MySQLConnectionString);
            MySqlCommand commandDatabase = new MySqlCommand(query, databaseConnection);

            try {
                databaseConnection.Open();
                MySqlDataReader myReader = commandDatabase.ExecuteReader();

                dataMessages.Load(myReader);
            }
            catch {

            }

            databaseConnection.Close();

            return dataMessages;
        }

        private void pageTransactionHistory_TextBoxSearch_TextChanged(object sender, EventArgs e) {
            string searchValue = pageTransactionHistory_TextBoxSearch.Text;
            int rowIndex = -1;

            try {
                foreach (DataGridViewRow row in pageTransactionHistory_DataGridView.Rows) {
                    if (row.Cells["_REFNO"].Value.ToString().StartsWith(searchValue)) {
                        rowIndex = row.Index;
                        pageTransactionHistory_DataGridView.CurrentCell = pageTransactionHistory_DataGridView.Rows[rowIndex].Cells[0];
                        pageTransactionHistory_DataGridView.Rows[pageTransactionHistory_DataGridView.CurrentCell.RowIndex].Selected = true;

                        break;
                    }
                }
            }
            catch (Exception exc) {
                MessageBox.Show(exc.Message);
            }
        } 

        private void setCurrentDateTexts() {
            pageTransactionHistory_LabelMonth.Text = currentDate.ToString("MMMM");
            pageTransactionHistory_LabelDay.Text = currentDate.Day.ToString();
            pageTransactionHistory_LabelYear.Text = currentDate.Year.ToString();
        }

        private void pageTransactionHistory_ButtonNext_Click(object sender, EventArgs e) {
            currentDate = currentDate.AddDays(1);
            setCurrentDateTexts();
            pageTransactionHistory_DataGridView.DataSource = GetTransactionHistory();
            showPercentageValuesTLC();
            GetIncome();
        }

        private void pageTransactionHistory_ButtonPrevious_Click(object sender, EventArgs e) {
            currentDate = currentDate.AddDays(-1);
            setCurrentDateTexts();
            pageTransactionHistory_DataGridView.DataSource = GetTransactionHistory();
            showPercentageValuesTLC();
            GetIncome();
        }

        private void GetIncome() {
            double total = 0;

            foreach (DataGridViewRow row in pageTransactionHistory_DataGridView.Rows) {
                total = total + (double.Parse(row.Cells["_AMOUNT"].Value.ToString().Split('(')[0]) - double.Parse(row.Cells["_AMOUNT"].Value.ToString().Split('(')[1].Split(')')[0]));
            }

            if(total > 0) {
                pageTransactionHistory_LabelIncomeOutput.Text = total.ToString();
            }
            else {
                pageTransactionHistory_LabelIncomeOutput.Text = "0";
            }
        }
        #endregion

       private void showPercentageValuesTLC() {

            string userRole = "";

            string query = "SELECT _ROLE FROM tbl_users WHERE _PHONE = '" + this.phoneNumber + "'";

            MySqlConnection databaseConnection = new MySqlConnection(this.MySQLConnectionString);
            MySqlCommand commandDatabase = new MySqlCommand(query, databaseConnection);
            databaseConnection.Open();

            MySqlDataReader myReader = commandDatabase.ExecuteReader();

            try {
                myReader.Read();
                userRole = myReader["_ROLE"].ToString();
            }
            catch (Exception error) {
                MessageBox.Show(error.Message);
            }

            databaseConnection.Close();

            userRole = userRole.Split(' ')[0].Trim();

            switch (userRole) {
                case ("RETAILER"):
                    percentage = 0.02;
                    break;
                case ("DISTRIBUTOR"):
                    percentage = 0.005;
                    break;
                case ("DEALER"):
                    percentage = 0.01;
                    break;
                case ("MOBILE"):
                    percentage = 0.005;
                    break;
                case ("CITY"):
                    percentage = 0.005;
                    break;
                case ("PROVINCIAL"):
                    percentage = 0.005;
                    break;
            }

            foreach (DataGridViewRow row in pageTransactionHistory_DataGridView.Rows) {
                row.Cells["_AMOUNT"].Value = row.Cells["_AMOUNT"].Value.ToString() + "(" + (double.Parse(row.Cells["_AMOUNT"].Value.ToString()) * (1 - percentage)).ToString() + ")";
            }
        }
    }
}
