using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using AjaxControlToolkit;
namespace ASPXUtils.Controls
{ 
    public class FeetInches : TextBox
    {    
        private bool _required = false;
        public bool Required
        {
            get { return _required; }
            set { _required = value; }
        } 
        public bool HideTic { get; set; }
        public bool HideFeetInchLabel { get; set; }
        public override string Text
        {
            get
            {
                double feet = 0;
                double inches = 0;
                if (Feet.Text != "" && StringUtils.IsDecimal(Feet.Text))
                    feet = Convert.ToDouble(Feet.Text);
                if (Inches.Text != "" && StringUtils.IsDecimal(Inches.Text))
                    inches = Convert.ToDouble(Inches.Text);

                double total = feet * 12 + inches;
                return Convert.ToString(total);
            }
            set
            {
                double val = 0;
                if(StringUtils.IsDecimal(value))
                    val = Convert.ToDouble(value);
                 
                double feet = Math.Floor(val / 12);
                double inches = val - (feet*12);
                Feet.Text = Convert.ToString(feet);
                Inches.Text = Convert.ToString(inches);  
            }
        }
         
        public TextBox Feet;
        public TextBox Inches;
        Literal litTic;
        Literal litTicIn;
        Literal label;
        RegularExpressionValidator oFeetVal;
        RegularExpressionValidator oIncVal;
        protected override void OnInit(EventArgs e)
        {

            Feet = new TextBox() { CssClass="feet", ID=this.ID+"_feet" };
            Inches = new TextBox() { CssClass = "inches", ID = this.ID + "_inches" };
            litTic = new Literal() { Text = "<span class='tic'> &nbsp; ' &nbsp; </span>", ID = this.ID + "_tec" };
            litTicIn = new Literal() { Text = "<span class='tic'> &nbsp; '' &nbsp; </span>", ID = this.ID + "_tecin" };
            label = new Literal() { Text = "<span class='feet-inch-label'> Feet / Inches </span><br style='clear:both;'>", ID = this.ID + "_filabel" };
            Controls.Add(Feet);
            if (!HideTic)
            {
                Controls.Add(litTic);
                Controls.Add(litTicIn); 
            }
            if (!HideFeetInchLabel)
                Controls.Add(label); 
            Controls.Add(Inches);
            base.OnInit(e);
             
            oFeetVal = new RegularExpressionValidator(); 
            oFeetVal.ControlToValidate = Inches.ID;
            oFeetVal.ID = "rfv_DecimalNumber" + Inches.ID;
            oFeetVal.CssClass = "errormsg";
            oFeetVal.ValidationExpression = @"^\d*[0-9](\.\d*[0-9])?$";
            oFeetVal.ErrorMessage = "Numeric Required (ex. 12.25)";
            Controls.Add(oFeetVal);
             
            oIncVal = new RegularExpressionValidator();
            oIncVal.ControlToValidate = Feet.ID;
            oIncVal.ID = "rfv_int" + Feet.ID;
            oIncVal.CssClass = "errormsg";
            oIncVal.ValidationExpression = @"^0|[0-9][0-9]*$";
            oIncVal.ErrorMessage = "Numeric Required";
            Controls.Add(oIncVal);
  
        } 
        protected override void OnLoad(EventArgs e)
        { 
            base.OnLoad(e); 
        }
        protected override void Render(HtmlTextWriter writer)
        {
            //label.RenderControl(writer);
            Feet.RenderControl(writer);
            litTic.RenderControl(writer); 
            Inches.RenderControl(writer);
            litTicIn.RenderControl(writer);
            oFeetVal.RenderControl(writer);
            oIncVal.RenderControl(writer); 
        } 
    }
}
