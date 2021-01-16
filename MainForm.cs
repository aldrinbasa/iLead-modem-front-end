using System;
using System.Globalization;
using System.Data;
using System.Windows.Forms;
using MySql.Data.MySqlClient;

namespace RichwayAllNetLoad {
    public partial class MainForm : Form {


        #region Public Variables
        public string selectedNetwork;
        public string selectedSearchType;
        public string selectedRetailer;
        public string MySQLConnectionString = "datasource=127.0.0.1;port=3306;username=root;password=;database=allload";

        public int membersLoadFundPageCounter = 1;
        public int monthlyTopSellerPageCounter = 1;

        public DateTime today = DateTime.Today;
        public DateTime monthNow = DateTime.Today;
        #endregion

        public MainForm() {
            InitializeComponent();
        }

        public void MainForm_Load(object sender, EventArgs e) {

            //MainForm Initializations
            buttonSmartSim_Click(null, null);

            //Sims Pages Initializations
            simsPage_ComboBoxSearchType.SelectedIndex = 0;
            simsPage_DataGridViewMessages.DataSource = GetMessagesList();

            simsPage_ComboBoxNetwork.SelectedItem = "Smart/TNT";
            simsPage_DataGridViewMessages.Columns["_REFNO"].Visible = false;

            simsPage_DataGridViewMessages.Columns[1].AutoSizeMode = DataGridViewAutoSizeColumnMode.AllCells;

            setCurrentDateTexts();

            //Member's Load Fund Initializations
            membersFundPage_ComboBoxShowEntries.SelectedIndex = 0;
            membersLoadFundPage_DataGridView.DataSource = GetMembersLoadFundList(membersLoadFundPageCounter, int.Parse(membersFundPage_ComboBoxShowEntries.Text));
            membersLoadFundPage_LabelPageNumber.Text = "Page " + membersLoadFundPageCounter;

            //Help Page Initializations
            helpPage_DataGridViewInbox.DataSource = GetHelpMessagesInbox();

            helpPage_DataGridViewInbox.Columns[1].DefaultCellStyle.Format = "MM/dd/yyyy";
            helpPage_DataGridViewInbox.Columns[2].DefaultCellStyle.Format = "HH:mm";

            helpPage_DataGridViewInbox.Columns[1].HeaderText = "DATE";
            helpPage_DataGridViewInbox.Columns[2].HeaderText = "TIME";

            helpPage_DataGridViewInbox.Columns[1].AutoSizeMode = DataGridViewAutoSizeColumnMode.AllCells;
            helpPage_DataGridViewInbox.Columns[2].AutoSizeMode = DataGridViewAutoSizeColumnMode.AllCells;

            //Retailers Pages Initializations
            retailersPage_DataGridView.DataSource = GetRetailersList();

            //Code Generator Page Initializations
            pagGenerateCode_ComboBoxStatus.SelectedIndex = 0;
            pagGenerateCode_DataGridView.DataSource = GetCodes();

            //Monthly Top Seller Initializations
            monthlyTopPage_ComboBoxShow.SelectedIndex = 0;
            monthlyTopSellerPageCounter = 1;
            monthlyTopPage_DataGridView.DataSource = GetMonthlyTopSellers(monthlyTopSellerPageCounter, int.Parse(monthlyTopPage_ComboBoxShow.Text));
            setCurrentMonthTexts();
            monthlyTopPage_LabelPage.Text = "Page" + monthlyTopSellerPageCounter.ToString();
        }

        #region Main Page Buttons/Navigators _Click()

        private void buttonMonthlyTopSeller_Click(object sender, EventArgs e) {
            tabControlMain.SelectedTab = tabPageMonthlyTopSeller;
            this.selectedNetwork = "Smart/TNT";
            simsPage_ComboBoxNetwork.SelectedItem = "Smart/TNT";
            simsPage_LabelMain.Text = "Smart/TNT Sim";
        }
        private void buttonSmartSim_Click(object sender, EventArgs e) {
            tabControlMain.SelectedTab = tabPageSim;
            this.selectedNetwork = "Smart/TNT";
            simsPage_ComboBoxNetwork.SelectedItem = "Smart/TNT";
            simsPage_LabelMain.Text = "Smart/TNT Sim";
        }

        private void buttonGlobeSim_Click(object sender, EventArgs e) {
            tabControlMain.SelectedTab = tabPageSim;
            this.selectedNetwork = "Globe/TM";
            simsPage_ComboBoxNetwork.SelectedItem = "Globe/TM";
            simsPage_LabelMain.Text = "Globe/TM Sim";
        }

        private void buttonSunSim_Click(object sender, EventArgs e) {
            tabControlMain.SelectedTab = tabPageSim;
            this.selectedNetwork = "Sun";
            simsPage_ComboBoxNetwork.SelectedItem = "Sun";
            simsPage_LabelMain.Text = "Sun Sim";
        }

        private void buttonMembersLoadFund_Click(object sender, EventArgs e) {
            tabControlMain.SelectedTab = tabPageMembersLoadFund;
            this.computeTotal();
        }

        private void buttonRetailer_Click(object sender, EventArgs e) {
            tabControlMain.SelectedTab = tabPageSellers;
            retailersPage_LabelTitle.Text = "RETAILER";
            this.selectedRetailer = "RETAILER";
            retailersPage_DataGridView.DataSource = GetRetailersList();
        }

        private void buttonDistributor_Click(object sender, EventArgs e) {
            tabControlMain.SelectedTab = tabPageSellers;
            retailersPage_LabelTitle.Text = "DISTRIBUTOR";
            this.selectedRetailer = "DISTRIBUTOR";
            retailersPage_DataGridView.DataSource = GetRetailersList();
        }

        private void buttonDealer_Click(object sender, EventArgs e) {
            tabControlMain.SelectedTab = tabPageSellers;
            retailersPage_LabelTitle.Text = "DEALER";
            this.selectedRetailer = "DEALER";
            retailersPage_DataGridView.DataSource = GetRetailersList();
        }

        private void buttonMobile_Click(object sender, EventArgs e) {
            tabControlMain.SelectedTab = tabPageSellers;
            retailersPage_LabelTitle.Text = "MOBILE";
            this.selectedRetailer = "MOBILE";
            retailersPage_DataGridView.DataSource = GetRetailersList();
        }

        private void buttonCity_Click(object sender, EventArgs e) {
            tabControlMain.SelectedTab = tabPageSellers;
            retailersPage_LabelTitle.Text = "CITY";
            this.selectedRetailer = "CITY";
            retailersPage_DataGridView.DataSource = GetRetailersList();
        }

        private void buttonProvincial_Click(object sender, EventArgs e) {
            tabControlMain.SelectedTab = tabPageSellers;
            retailersPage_LabelTitle.Text = "PROVINCIAL";
            this.selectedRetailer = "PROVINCIAL";
            retailersPage_DataGridView.DataSource = GetRetailersList();
        }

        private void buttonGenerateCode_Click(object sender, EventArgs e) {
            tabControlMain.SelectedTab = tabPageGenerateCode;
        }

        private void buttonAddDeductLoadWallet_Click(object sender, EventArgs e) {
            tabControlMain.SelectedTab = tabPageAddDeductLoadWallet;
        }

        private void buttonAddDeductRetailerPin_Click(object sender, EventArgs e) {
            tabControlMain.SelectedTab = tabPageAddDeductRetailerPins;
        }

        private void buttonHelp_Click(object sender, EventArgs e) {
            tabControlMain.SelectedTab = tabPageHelp;
        }
        #endregion

        #region Page - Add/Deduct Load Wallet

        private void loadWalletPage_ButtonSearch_Click(object sender, EventArgs e) {

            loadWalletPage_TextBoxAmount.Text = "";
            string searchValue = loadWalletPage_TextBoxSearch.Text;

            string query = "SELECT tbl_account._ID, tbl_account._PHONE, tbl_account._BALANCE, tbl_account._PINS, tbl_users._USERNAME,  tbl_users._FULLNAME, tbl_users._ADDRESS FROM tbl_account INNER JOIN tbl_users ON tbl_account._PHONE=tbl_users._PHONE WHERE tbl_users._PHONE = " + searchValue.ToString();

            MySqlConnection databaseConnection = new MySqlConnection(this.MySQLConnectionString);
            MySqlCommand commandDatabase = new MySqlCommand(query, databaseConnection);
            databaseConnection.Open();

            MySqlDataReader myReader = commandDatabase.ExecuteReader();

            try {
                myReader.Read();
                loadWalletPage_LabelDataName.Text = myReader["_FULLNAME"].ToString();
                loadWalletPage_LabelDataUserName.Text = myReader["_USERNAME"].ToString();
                loadWalletPage_LabelDataAddress.Text = myReader["_ADDRESS"].ToString();
                loadWalletPage_LabelDataBalance.Text = myReader["_BALANCE"].ToString();
            }
            catch {
                MessageBox.Show("Mobile number: " + searchValue.ToString() + " not found.");
            }

            databaseConnection.Close();
        }

        private void loadWalletPage_ButtonAdd_Click(object sender, EventArgs e) {

            decimal balance = decimal.Parse(loadWalletPage_LabelDataBalance.Text);
            decimal addend = decimal.Parse(loadWalletPage_TextBoxAmount.Text);

            string query = "UPDATE tbl_account SET _BALANCE = " + (balance + addend).ToString() + " WHERE _PHONE = '" + loadWalletPage_TextBoxSearch.Text + "'";
            MySqlConnection databaseConnection = new MySqlConnection(this.MySQLConnectionString);
            MySqlCommand commandDatabase = new MySqlCommand(query, databaseConnection);

            databaseConnection.Open();

            commandDatabase.ExecuteNonQuery();
            MessageBox.Show("Successfully added ₱" + loadWalletPage_TextBoxAmount.Text);

            loadWalletPage_ButtonSearch_Click(null, null);

            databaseConnection.Close();
        }

        private void loadWalletPage_ButtonDeduct_Click(object sender, EventArgs e) {

            decimal balance = decimal.Parse(loadWalletPage_LabelDataBalance.Text);
            decimal subtrahend = decimal.Parse(loadWalletPage_TextBoxAmount.Text);

            if (!(subtrahend > balance)) {

                string query = "UPDATE tbl_account SET _BALANCE = " + (balance - subtrahend).ToString() + " WHERE _PHONE = '" + loadWalletPage_TextBoxSearch.Text + "'";
                MySqlConnection databaseConnection = new MySqlConnection(this.MySQLConnectionString);
                MySqlCommand commandDatabase = new MySqlCommand(query, databaseConnection);

                databaseConnection.Open();

                commandDatabase.ExecuteNonQuery();
                MessageBox.Show("Successfully deducted ₱" + subtrahend);

                loadWalletPage_ButtonSearch_Click(null, null);

                databaseConnection.Close();
            }
            else {
                MessageBox.Show("₱" + subtrahend + " can not be deducted from the user's balance of ₱" + balance);
            }
        }

        #endregion

        #region Page - Add/Deduct Pins
        private void retailPinsPage_ButtonSearch_Click(object sender, EventArgs e) {

            retailPinsPage_TextBoxNumberOfPins.Text = "";
            string searchValue = retailPinsPage_TextBoxSearch.Text;

            string query = "SELECT tbl_account._ID, tbl_account._PHONE, tbl_account._BALANCE, tbl_account._PINS, tbl_users._USERNAME,  tbl_users._FULLNAME, tbl_users._ADDRESS FROM tbl_account INNER JOIN tbl_users ON tbl_account._PHONE=tbl_users._PHONE WHERE tbl_users._PHONE = " + searchValue.ToString();

            MySqlConnection databaseConnection = new MySqlConnection(this.MySQLConnectionString);
            MySqlCommand commandDatabase = new MySqlCommand(query, databaseConnection);
            databaseConnection.Open();

            MySqlDataReader myReader = commandDatabase.ExecuteReader();

            try {
                myReader.Read();
                retailPinsPage_LabelDataName.Text = myReader["_FULLNAME"].ToString();
                retailPinsPage_LabelDataUserName.Text = myReader["_USERNAME"].ToString();
                retailPinsPage_LabelDataAddress.Text = myReader["_ADDRESS"].ToString();
                retailPinsPage_LabelDataAvailablePins.Text = myReader["_PINS"].ToString();
            }
            catch (Exception error) {
                MessageBox.Show("Mobile number: " + searchValue.ToString() + " not found." + error.Message);
            }

            databaseConnection.Close();
        }

        private void retailPinsPage_ButtonAdd_Click(object sender, EventArgs e) {
            int availablePins = int.Parse(retailPinsPage_LabelDataAvailablePins.Text);
            int addend = int.Parse(retailPinsPage_TextBoxNumberOfPins.Text);

            string query = "UPDATE tbl_account SET _PINS = " + (availablePins + addend).ToString() + " WHERE _PHONE = '" + retailPinsPage_TextBoxSearch.Text + "'";
            MySqlConnection databaseConnection = new MySqlConnection(this.MySQLConnectionString);
            MySqlCommand commandDatabase = new MySqlCommand(query, databaseConnection);

            databaseConnection.Open();

            commandDatabase.ExecuteNonQuery();
            MessageBox.Show("Successfully added " + retailPinsPage_TextBoxNumberOfPins.Text + " PINS");

            retailPinsPage_ButtonSearch_Click(null, null);

            databaseConnection.Close();
        }

        private void retailPinsPage_ButtonDeduct_Click(object sender, EventArgs e) {

            int availablePins = int.Parse(retailPinsPage_LabelDataAvailablePins.Text);
            int subtrahend = int.Parse(retailPinsPage_TextBoxNumberOfPins.Text);

            if (!(subtrahend > availablePins)) {
                string query = "UPDATE tbl_account SET _PINS = " + (availablePins - subtrahend).ToString() + " WHERE _PHONE = '" + retailPinsPage_TextBoxSearch.Text + "'";
                MySqlConnection databaseConnection = new MySqlConnection(this.MySQLConnectionString);
                MySqlCommand commandDatabase = new MySqlCommand(query, databaseConnection);

                databaseConnection.Open();

                commandDatabase.ExecuteNonQuery();
                MessageBox.Show("Successfully deducted " + subtrahend + " PINS");

                retailPinsPage_ButtonSearch_Click(null, null);

                databaseConnection.Close();
            }
            else {
                MessageBox.Show(subtrahend + " PINS can not be deducted from the user's available PINS of " + availablePins);
            }
        }
        #endregion

        #region Page - Sims
        private void textBoxSearch_TextChanged(object sender, EventArgs e) {
            string searchValue = SimsPage_TextBoxSearch.Text;
            int rowIndex = -1;

            if (selectedSearchType == "TRX") {
                try {
                    foreach (DataGridViewRow row in simsPage_DataGridViewMessages.Rows) {
                        if (row.Cells["_TRCNO"].Value.ToString().StartsWith(searchValue)) {
                            rowIndex = row.Index;
                            simsPage_DataGridViewMessages.CurrentCell = simsPage_DataGridViewMessages.Rows[rowIndex].Cells[0];
                            simsPage_DataGridViewMessages.Rows[simsPage_DataGridViewMessages.CurrentCell.RowIndex].Selected = true;

                            break;
                        }
                    }
                }
                catch (Exception exc) {
                    MessageBox.Show(exc.Message);
                }
            }
            else if (selectedSearchType == "REF No.") {
                try {
                    foreach (DataGridViewRow row in simsPage_DataGridViewMessages.Rows) {
                        if (row.Cells["_REFNO"].Value.ToString().StartsWith(searchValue)) {
                            rowIndex = row.Index;
                            simsPage_DataGridViewMessages.CurrentCell = simsPage_DataGridViewMessages.Rows[rowIndex].Cells[0];
                            simsPage_DataGridViewMessages.Rows[simsPage_DataGridViewMessages.CurrentCell.RowIndex].Selected = true;

                            break;
                        }
                    }
                }
                catch (Exception exc) {
                    MessageBox.Show(exc.Message);
                }
            }
        }

        private void comboBoxNetwork_SelectedIndexChanged(object sender, EventArgs e) {
            this.selectedNetwork = (String)simsPage_ComboBoxNetwork.SelectedItem;
            simsPage_DataGridViewMessages.DataSource = GetMessagesList();
            SimsPage_TextBoxSearch.Text = "";
        }

        private void comboBoxSearchType_SelectedIndexChanged(object sender, EventArgs e) {
            this.selectedSearchType = (String)simsPage_ComboBoxSearchType.SelectedItem;
            SimsPage_TextBoxSearch.Text = "";
        }

        private void membersLoadFundPage_DataGridView_CellContentClick(object sender, DataGridViewCellEventArgs e) {

        }

        private DataTable GetMessagesList() {

            DataTable dataMessages = new DataTable();

            string query = "SELECT _MESSAGE, _TRCNO, _REFNO FROM tbl_messages";

            if (this.selectedNetwork == "Globe/TM") {
                query = "SELECT _MESSAGE, _TRCNO, _REFNO FROM tbl_messages WHERE _CARRIER='Globe/TM' AND _DATETIME LIKE '" + today.ToString("yyyy-MM-dd") + "%'";
            }
            else if (this.selectedNetwork == "Smart/TNT") {
                query = "SELECT _MESSAGE, _TRCNO, _REFNO FROM tbl_messages WHERE _CARRIER='Smart/TNT' AND _DATETIME LIKE '" + today.ToString("yyyy-MM-dd") + "%'";
            }
            else if (this.selectedNetwork == "Sun") {
                query = "SELECT _MESSAGE, _TRCNO, _REFNO FROM tbl_messages WHERE _CARRIER='Sun' AND _DATETIME LIKE '" + today.ToString("yyyy-MM-dd") + "%'";
            }


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

        private void membersLoadFundPage_LabelNextDate_Click(object sender, EventArgs e) {
            today = today.AddDays(1);
            setCurrentDateTexts();
            simsPage_DataGridViewMessages.DataSource = GetMessagesList();

        }

        private void membersLoadFundPage_ButtonPreviousDate_Click(object sender, EventArgs e) {
            today = today.AddDays(-1);
            setCurrentDateTexts();
            simsPage_DataGridViewMessages.DataSource = GetMessagesList();
        }
        #endregion

        #region Page - Member's Load Fund
        private DataTable GetMembersLoadFundList(int pageNumber, int rowCount) {

            DataTable dataMembersLoadFund = new DataTable();
            DataSet dataSetMembersLoadFund = new DataSet();

            string query = "SELECT tbl_account._PHONE, tbl_users._FULLNAME, tbl_users._ADDRESS, tbl_account._BALANCE FROM tbl_account INNER JOIN tbl_users ON tbl_account._PHONE=tbl_users._PHONE";

            MySqlConnection databaseConnection = new MySqlConnection(this.MySQLConnectionString);
            MySqlDataAdapter adapterMembersLoadFund = new MySqlDataAdapter(query, databaseConnection);


            try {
                databaseConnection.Open();
                adapterMembersLoadFund.Fill(dataSetMembersLoadFund, (pageNumber - 1) * rowCount, rowCount, "Table");
            }
            catch {

            }

            databaseConnection.Close();

            dataMembersLoadFund = dataSetMembersLoadFund.Tables[0];

            return dataMembersLoadFund;
        }

        private void membersLoadFundPage_ButtonNext_Click(object sender, EventArgs e) {

            if (membersLoadFundPage_DataGridView.Rows.Count == int.Parse(membersFundPage_ComboBoxShowEntries.Text)) {
                membersLoadFundPageCounter = membersLoadFundPageCounter + 1;

                membersLoadFundPage_DataGridView.DataSource = GetMembersLoadFundList(membersLoadFundPageCounter, int.Parse(membersFundPage_ComboBoxShowEntries.Text));
            }

            membersLoadFundPage_LabelPageNumber.Text = "Page " + membersLoadFundPageCounter;

            computeTotal();
        }

        private void membersLoadFundPage_ButtonPrevious_Click(object sender, EventArgs e) {

            if(membersLoadFundPageCounter > 1) {
                membersLoadFundPageCounter = membersLoadFundPageCounter - 1;

                membersLoadFundPage_DataGridView.DataSource = GetMembersLoadFundList(membersLoadFundPageCounter, int.Parse(membersFundPage_ComboBoxShowEntries.Text));
            }

            membersLoadFundPage_LabelPageNumber.Text = "Page " + membersLoadFundPageCounter;

            computeTotal();
        }

        private void membersFundPage_ComboBoxShowEntries_SelectedIndexChanged(object sender, EventArgs e) {
            membersLoadFundPage_DataGridView.DataSource = GetMembersLoadFundList(membersLoadFundPageCounter, int.Parse(membersFundPage_ComboBoxShowEntries.Text));

            computeTotal();
        }

        private void computeTotal() {
            double total = 0;

            foreach(DataGridViewRow row in membersLoadFundPage_DataGridView.Rows) {
                total = total + double.Parse(row.Cells["_BALANCE"].Value.ToString());
            }

            membersLoadFundPage_TotalFund.Text = total.ToString();
        }
        #endregion

        #region Page- Help
        private DataTable GetHelpMessagesInbox() {

            DataTable dataMessages = new DataTable();

            string query = "SELECT _MESSAGE, _DATETIME, _DATETIME FROM tbl_help";

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
        #endregion

        #region Page - Retailers
        private DataTable GetRetailersList() {

            DataTable dataMessages = new DataTable();
            
            string query = "SELECT tbl_users._FULLNAME, tbl_account._PHONE, tbl_account._BALANCE, tbl_account._PINS FROM tbl_account INNER JOIN tbl_users ON tbl_account._PHONE=tbl_users._PHONE";

            if (this.selectedRetailer == "RETAILER") {
                query = query + " WHERE tbl_users._ROLE = 'RETAILER'";
            }
            else if (this.selectedRetailer == "DISTRIBUTOR") {
                query = query + " WHERE tbl_users._ROLE = 'DISTRIBUTOR'";
            }
            else if (this.selectedRetailer == "DEALER") {
                query = query + " WHERE tbl_users._ROLE = 'DEALER'";
            }
            else if (this.selectedRetailer == "MOBILE") {
                query = query + " WHERE tbl_users._ROLE = 'MOBILE'";
            }
            else if (this.selectedRetailer == "CITY") {
                query = query + " WHERE tbl_users._ROLE = 'CITY'";
            }
            else if (this.selectedRetailer == "PROVINCIAL") {
                query = query + " WHERE tbl_users._ROLE = 'PROVINCIAL'";
            }

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

        private void retailersPage_DataGridView_CellDoubleClick(object sender, DataGridViewCellEventArgs e) {

            int rowIndexSelected = e.RowIndex;
            string phoneNumber = retailersPage_DataGridView.Rows[rowIndexSelected].Cells["_PHONE"].Value.ToString();

            RetailersDetailsForm retailersDetails = new RetailersDetailsForm(phoneNumber, this);
            retailersDetails.ShowDialog();
        }


        #endregion

        #region Page - Generate Code
        private DataTable GetCodes() {

            DataTable dataMessages = new DataTable();

            string query = "SELECT tbl_act_code._CODE, tbl_act_code._USED, tbl_users._PHONE, tbl_users._FULLNAME, tbl_users._ADDRESS, tbl_act_code._TYPE, tbl_act_code._DATETIME FROM tbl_act_code LEFT JOIN tbl_users ON tbl_act_code._USER=tbl_users._USERNAME WHERE _DATETIME LIKE '" + today.ToString("yyyy-MM-dd") + "%'";

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

        private void pagGenerateCode_ButtonSave_Click(object sender, EventArgs e) {
            string code = pagGenerateCode_TextBoxCode.Text;
            string type = pagGenerateCode_ComboBoxStatus.Text;
            DateTime dateTime = new DateTime();

            dateTime = DateTime.Now;

            string query = "INSERT INTO tbl_act_code (_CODE, _TYPE, _USED, _DATETIME) VALUES('" + code + "', '" + type + "', 'NO', '" + dateTime.ToString("yyyy-MM-dd") + "')";

            MySqlConnection databaseConnection = new MySqlConnection(this.MySQLConnectionString);
            MySqlCommand commandDatabase = new MySqlCommand(query, databaseConnection);

            databaseConnection.Open();

            commandDatabase.ExecuteNonQuery();
            MessageBox.Show("Successfully added Code: " + code);

            databaseConnection.Close();
            reloadData();
        }

        private void pagGenerateCode_ButtonNext_Click(object sender, EventArgs e) {
            today = today.AddDays(1);
            setCurrentDateTexts();
            pagGenerateCode_DataGridView.DataSource = GetCodes();

        }

        private void pagGenerateCode_ButtonPrevious_Click(object sender, EventArgs e) {
            today = today.AddDays(-1);
            setCurrentDateTexts();
            pagGenerateCode_DataGridView.DataSource = GetCodes();
        }
        #endregion

        #region Page - Monthly Top Sellers
        private void monthlyTopPage_ComboBoxShow_SelectedIndexChanged(object sender, EventArgs e) {
            monthlyTopPage_DataGridView.DataSource = GetMonthlyTopSellers(monthlyTopSellerPageCounter, int.Parse(monthlyTopPage_ComboBoxShow.Text));
        }

        private DataTable GetMonthlyTopSellers(int pageNumber, int rowCount) {

            DataTable dataMonthlyTopSellers = new DataTable();
            DataSet dataSetMonthlyTopSellers = new DataSet();

            string query = "SELECT * FROM tbl_monthly_top WHERE _UPDATED LIKE '" + monthNow.ToString("yyyy-MM") + "-%'";

            Console.WriteLine(query);

            MySqlConnection databaseConnection = new MySqlConnection(this.MySQLConnectionString);
            MySqlDataAdapter adapterMonthlyTopSellers = new MySqlDataAdapter(query, databaseConnection);


            try {
                databaseConnection.Open();
                adapterMonthlyTopSellers.Fill(dataSetMonthlyTopSellers, (pageNumber - 1) * rowCount, rowCount, "Table");

                databaseConnection.Close();

                dataMonthlyTopSellers = dataSetMonthlyTopSellers.Tables[0];
            }
            catch {

            }
            return dataMonthlyTopSellers;
        }

        private void monthlyTopPage_ButtonNextMonth_Click(object sender, EventArgs e) {
            monthNow = monthNow.AddMonths(1);
            setCurrentMonthTexts();
            monthlyTopPage_DataGridView.DataSource = GetMonthlyTopSellers(monthlyTopSellerPageCounter, int.Parse(monthlyTopPage_ComboBoxShow.Text));
        }

        private void monthlyTopPage_ButtonPreviousMonth_Click(object sender, EventArgs e) {
            monthNow = monthNow.AddMonths(-1);
            setCurrentMonthTexts();
            monthlyTopPage_DataGridView.DataSource = GetMonthlyTopSellers(monthlyTopSellerPageCounter, int.Parse(monthlyTopPage_ComboBoxShow.Text));
        }

        private void monthlyTopPage_ButtonNextPage_Click(object sender, EventArgs e) {
            if (monthlyTopPage_DataGridView.Rows.Count == int.Parse(monthlyTopPage_ComboBoxShow.Text)) {
                monthlyTopSellerPageCounter = monthlyTopSellerPageCounter + 1;

                monthlyTopPage_DataGridView.DataSource = GetMonthlyTopSellers(monthlyTopSellerPageCounter, int.Parse(monthlyTopPage_ComboBoxShow.Text));
            }

            monthlyTopPage_LabelPage.Text = "Page " + monthlyTopSellerPageCounter;
        }

        private void monthlyTopPage_ButtonPreviousPage_Click(object sender, EventArgs e) {
            if (monthlyTopSellerPageCounter > 1) {
                monthlyTopSellerPageCounter = monthlyTopSellerPageCounter - 1;

                monthlyTopPage_DataGridView.DataSource = GetMonthlyTopSellers(monthlyTopSellerPageCounter, int.Parse(monthlyTopPage_ComboBoxShow.Text));
            }

            monthlyTopPage_LabelPage.Text = "Page " + monthlyTopSellerPageCounter;
        }
        #endregion

        #region Public Methods
        public void reloadData() {
            simsPage_DataGridViewMessages.DataSource = GetMessagesList();
            membersLoadFundPage_DataGridView.DataSource = GetMembersLoadFundList(membersLoadFundPageCounter, int.Parse(membersFundPage_ComboBoxShowEntries.Text));

            helpPage_DataGridViewInbox.DataSource = GetHelpMessagesInbox();

            helpPage_DataGridViewInbox.Columns[1].DefaultCellStyle.Format = "MM/dd/yyyy";
            helpPage_DataGridViewInbox.Columns[2].DefaultCellStyle.Format = "HH:mm";

            helpPage_DataGridViewInbox.Columns[1].HeaderText = "DATE";
            helpPage_DataGridViewInbox.Columns[2].HeaderText = "TIME";

            helpPage_DataGridViewInbox.Columns[1].AutoSizeMode = DataGridViewAutoSizeColumnMode.AllCells;
            helpPage_DataGridViewInbox.Columns[2].AutoSizeMode = DataGridViewAutoSizeColumnMode.AllCells;

            retailersPage_DataGridView.DataSource = GetRetailersList();

            pagGenerateCode_DataGridView.DataSource = GetCodes();

            monthlyTopPage_DataGridView.DataSource = GetMonthlyTopSellers(monthlyTopSellerPageCounter, int.Parse(monthlyTopPage_ComboBoxShow.Text));
        }
        
        private void buttonRefresh_Click(object sender, EventArgs e) {
            this.reloadData();
        }

        private void setCurrentDateTexts() {
            membersLoadFundPage_LabelMonth.Text = today.ToString("MMMM");
            membersLoadFundPage_LabelDay.Text = today.Day.ToString();
            membersLoadFundPage_LabelYear.Text = today.Year.ToString();

            pagGenerateCode_LabelMonth.Text = today.ToString("MMMM");
            pagGenerateCode_LabelDay.Text = today.Day.ToString();
            pagGenerateCode_LabelYear.Text = today.Year.ToString();
        }

        private void setCurrentMonthTexts() {
            monthlyTopPage_LabelMonth.Text = monthNow.ToString("MMMM");
            monthlyTopPage_LabelYear.Text = monthNow.ToString("yyyy");
        }


        #endregion
    }
}
