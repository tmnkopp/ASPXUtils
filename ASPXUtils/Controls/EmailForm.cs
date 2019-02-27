using System;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Collections;
using System.ComponentModel;
using System.Net.Mail;
using MSCaptcha; 
using System.Text;
namespace ASPXUtils.Controls
{
    [ToolboxData("<{0}:EmailForm runat=server></{0}:EmailForm>")]
    public class EmailForm : CompositeControl
    {
        public Panel ItemPanel;
        public PlaceHolder ControlHolder;
        public Literal Result;
        public Label litFeedback;
        public Literal HTMLOutput;
        public string Message { get; set; }
        public string MessageAppend { get; set; }
        public string BCC { get; set; } 
        public string FormTitle { get; set; }
        public bool HTMLMessage { get; set; } 
        public string MessageHeader { get; set; }
        public bool Debug { get; set; }
        public bool UseSend { get; set; }
        public bool FormSent { get; set; }
        public bool UseCaptcha { get; set; }
        private int RowCnt;
        public StringBuilder sbMessage;

        private string _from = "contact@" + HttpContext.Current.Request.ServerVariables["SERVER_NAME"].Replace("www.", "");
        public string From
        {
            get { return _from; }
            set { _from = value; }
        }
        private string _ReplyTo;
        public string ReplyTo
        {
            get { return _ReplyTo; }
            set { _ReplyTo = value; }
        }
        private string _subject = HttpContext.Current.Request.ServerVariables["SERVER_NAME"] + " Contact Form ";
        public string Subject
        {
            get { return _subject; }
            set { _subject = value; }
        }

        private string _to = "";
        public string To
        {
            get { return _to; }
            set { _to = value; }
        }
        private string _cc = "";
        public string CC
        {
            get { return _cc; }
            set { _cc = value; }
        }

        private string captchaInvalidMessage = "  The code entered was invalid. Please re-enter. ";
        public string CaptchaInvalidMessage
        {
            get { return captchaInvalidMessage; }
            set { captchaInvalidMessage = value; } 
        }
         private string emailSentMessage = "Your request has been sent. We will be contacting you shortly.";
        public string EmailSentMessage
        {
            get { return emailSentMessage; }
            set { emailSentMessage = value; } 
        }

        private string emailButtonText = "Send Email";
        public string EmailButtonText
        {
            get { return emailButtonText; }
            set { emailButtonText = value; }
        } 

         
        private Button cmdSubmit = new Button() { Text = "Send Email", CssClass="emailform-send-button" };
        public Control ItemTemplateControls {
            get {
                if (this.ControlHolder != null)
                    return this.ControlHolder.Controls[0];
                else 
                    return null;
            }
        }
        public Control GetItemTemplateControl(string name)
        {
            if (this.ControlHolder != null) 
                return this.ControlHolder.Controls[0].FindControl(name); 
            else
                  return null;
        }

        private ITemplate itemTemplate;
        [Browsable(false), Description("Item template"),
        PersistenceMode(PersistenceMode.InnerProperty),
        TemplateContainer(typeof(BasicTemplateContainer))]
        public ITemplate ItemTemplate
        {
            get { return itemTemplate; }
            set { itemTemplate = value; }
        }

        public EmailForm()
            : base()
        {
            ControlHolder = new PlaceHolder { ID = "ControlHolder" };
            
        }
        protected override void OnInit(EventArgs e)
        {
            base.OnInit(e);
            cmdSubmit.Text = this.EmailButtonText; 
            cmdSubmit.Click += new EventHandler(cmdSubmit_Click);
            sbMessage = new StringBuilder();
        }
        protected override void OnPreRender(EventArgs e)
        {
            base.OnPreRender(e);
            PlaceHolder CaptchaPlaceHolder = (PlaceHolder)this.GetItemTemplateControl("CaptchaPlaceHolder");
            if (CaptchaPlaceHolder != null)
            {
                CaptchaPlaceHolder.Visible = this.UseCaptcha;
            }
        }

        protected void cmdSubmit_Click(object source, EventArgs e)
        {
            SendForm();
        }
     
        private void CreateControlHierarchy()
        { 
            Controls.Add(ControlHolder);
            litFeedback = new Label() {CssClass="ok", Visible=false};
            Controls.Add(litFeedback);
            if (ItemTemplate != null)
            {
                BasicTemplateContainer item = new BasicTemplateContainer();
                ItemTemplate.InstantiateIn(item); 
                ControlHolder.Controls.Add(item); 

            }
            if (UseSend)
                Controls.Add(cmdSubmit);

            Result = new Literal() { ID = "Result", Text = "" };
            Controls.Add(Result);

            HTMLOutput = new Literal() { ID = "HTMLOutput", Text="" };
            Controls.Add(HTMLOutput);
            
        }

        public bool CaptchaValidates() {
            bool validate = true;
            if (this.UseCaptcha )
            {
                PlaceHolder CaptchaPlaceHolder = (PlaceHolder)this.GetItemTemplateControl("CaptchaPlaceHolder");
                if (CaptchaPlaceHolder != null)
                {
                    CaptchaControl oCaptchaControl = (CaptchaControl)CaptchaPlaceHolder.FindControl("oCaptchaControl");
                    ExtendedTextBox oCaptchaText = (ExtendedTextBox)CaptchaPlaceHolder.FindControl("oCaptchaText");
                    if (oCaptchaControl != null && oCaptchaText != null)
                    {
                        oCaptchaControl.ValidateCaptcha(oCaptchaText.Text);  
                        if (!oCaptchaControl.UserValidated) 
                        {
                            validate = false;
                            Result.Text = this.CaptchaInvalidMessage;
                        } 
                    } 
                } 
            }
  
            return validate;
        }

        public void SendForm()
        {
            if (this.UseCaptcha && !CaptchaValidates()) 
                return; 
            foreach (Control c in this.ControlHolder.Controls[0].Controls) 
                GetValue(c);
            
            SendMail(this.Subject, this.sbMessage.ToString());
        }
        private void GetValue(Control c)
        {
            if (c is System.Web.UI.WebControls.DropDownList)
            {
                DropDownList ctrl = (DropDownList)c;
                sbMessage.AppendFormat("{0}",FormatRow(CleanName(ctrl.ToolTip), (string)ctrl.SelectedValue)); 
            }
            else if (c is System.Web.UI.WebControls.TextBox)
            {
                TextBox ctrl = (TextBox)c;
                 sbMessage.AppendFormat("{0}",FormatRow(CleanName(ctrl.ToolTip), (string)ctrl.Text));
            }
            else if (c is System.Web.UI.WebControls.CheckBox)
            {
                CheckBox ctrl = (CheckBox)c;
                sbMessage.AppendFormat("{0}",FormatRow(CleanName(ctrl.ToolTip), Convert.ToString(ctrl.Checked)));
            }
            else if (c is ASPXUtils.Controls.ExtendedTextBox)
            {
                ASPXUtils.Controls.ExtendedTextBox ctrl = (ASPXUtils.Controls.ExtendedTextBox)c;
                sbMessage.AppendFormat("{0}",FormatRow(CleanName(ctrl.ToolTip), (string)ctrl.Text));
            }
            else if (c is System.Web.UI.WebControls.RadioButtonList)
            {
                RadioButtonList ctrl = (RadioButtonList)c;
                sbMessage.AppendFormat("{0}",FormatRow(CleanName(ctrl.ToolTip), (string)ctrl.SelectedValue));
            }
        }

        private string FormatRow(string lab, string val)
        {
            RowCnt++;
            string bg = "";
            string _ret = "";
            if (!this.HTMLMessage)
            {
                _ret = "<br>" + lab + ": " + val;
            }
            else
            {
                if (RowCnt % 2 == 0)
                {
                    //bg = "#f7f7f7";
                }
                _ret = "<tr style='background:" + bg + ";'><td style='width:100px;white-space:nowrap;font-weight:bold;'>" + lab + "</td><td> " + val + "</td></tr>";
            }
            return _ret;
        }

        private string CleanName(string nam)
        {
            nam = ASPXUtils.StringUtils.ParseCamel(nam);
            nam = nam.Replace("_", " ");
            nam = nam.Replace("  ", " ");
            return nam;
        }

        private string FormatMessage(string message)
        {
            if (this.HTMLMessage)
            {
                message = "<table style='font-family:tahoma;font-size:12px;'>" + message + "</table>";
            }
            if (this.MessageHeader != "" && this.MessageHeader != null)
            {
                message = "<div style='border:1px solid #000000;padding:6px;'>" + this.MessageHeader + message + "</div>";
            }
            return message;
        }

        public void SendMail(string subject, string message)
        {
            message = FormatMessage(message);

            MailMessage oMailMessage = new MailMessage();
            oMailMessage.From = new MailAddress(this.From);

            if (this.To != "" && this.To != null)
            {
                string[] _to = this.To.Split(';');
                foreach (string to in _to)
                {
                    if (to != "")
                        oMailMessage.To.Add(new MailAddress(to));
                }
            }
            if (this.CC != "" && this.CC != null)
            {
                string[] _cc = this.CC.Split(';');
                foreach (string cc in _cc)
                {
                    if (cc != "")
                        oMailMessage.CC.Add(new MailAddress(cc));
                }
            }
            if (this.BCC != "" && this.BCC != null)
            {
                string[] _bcc = this.BCC.Split(';');
                foreach (string bcc in _bcc)
                {
                    if (bcc != "")
                        oMailMessage.Bcc.Add(new MailAddress(bcc));
                }
            }
            
 
            if (oMailMessage.To.Count <= 0)
            {
                oMailMessage.To.Add(new MailAddress(this.From));
            }

            oMailMessage.IsBodyHtml = true;
            oMailMessage.Subject = subject;
            oMailMessage.Body = message;

            try
            {
                if (this.ReplyTo != null || this.ReplyTo != "")
                {
                    oMailMessage.ReplyTo = new MailAddress(this.ReplyTo);
                } 
            }
            catch (Exception ex)
            {
            }
            
 
            SmtpClient client = new SmtpClient();

            if (this.Debug)
            {
                StringBuilder SBDebug = new StringBuilder();
                SBDebug.AppendFormat("<br>From: {0}", this.From);
                SBDebug.AppendFormat("<br>To: {0}", this.To);
                SBDebug.AppendFormat("<br>CC: {0}", this.CC);
                SBDebug.AppendFormat("<br>Message: {0}", message);
                SBDebug.AppendFormat("<br>Reply To: {0}", oMailMessage.ReplyTo.ToString());
                HTMLOutput.Text = SBDebug.ToString();
            }
            try
            {
             
                client.Send(oMailMessage);
                if (litFeedback.Text != "")
                    litFeedback.Visible = true;
                Result.Text = "Email Sent";
                this.FormSent = true;
            }
            catch (Exception ex)
            {
                Result.Text =  ex.Message + " " + this.ToString(); 
            }
            
        }



        public class BasicTemplateContainer : WebControl, INamingContainer
        {
            public BasicTemplateContainer()
                : base(HtmlTextWriterTag.Span)
            { 
            }
        }
        override protected void CreateChildControls()
        {
            Controls.Clear();
            CreateControlHierarchy();
        }
        public override ControlCollection Controls
        {
            get { EnsureChildControls(); return base.Controls; }
        }
        public override void DataBind()
        {
            CreateChildControls();
            ChildControlsCreated = true; 
            base.DataBind();
        }

    }
}
